using System;
using UnityEditor;
using UnityEngine;
using static UnityUtilities.Attributes.ShowIfAttribute;

namespace UnityUtilities.Attributes
{
    public class ShowIfComponentsAttribute : PropertyAttribute
    {
        public bool HideInInspector;
        public Type[] ComponentTypes;
        public bool ExpectMissingComponents;
        public EvaluationModeType EvaluationMode;
        public ShowIfComponentsAttribute(bool expectMissingComponents = false, bool hideInInspector = false, EvaluationModeType evaluationMode = EvaluationModeType.AND, params Type[] componentTypes)
        {
            this.ExpectMissingComponents = expectMissingComponents;
            this.HideInInspector = hideInInspector;
            this.ComponentTypes = componentTypes;
            this.EvaluationMode = evaluationMode;
        }
        public ShowIfComponentsAttribute(params Type[] componentTypes) : this(false, false, EvaluationModeType.AND, componentTypes)
        {
        }
    }
    [CustomPropertyDrawer(typeof(ShowIfComponentsAttribute))]
    public class ConditionalComponentsHidePropertyDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
#if UNITY_EDITOR
            ShowIfComponentsAttribute condHAtt = (ShowIfComponentsAttribute)attribute;
            bool enabled = GetConditionalSourceField(property, condHAtt);
            bool wasEnabled = GUI.enabled;
            GUI.enabled = enabled;

            // if is enable draw the label
            if (enabled)
                EditorGUI.PropertyField(position, property, label, true);
            // if is not enabled but we want not hide it, then draw it disabled
            else if (!condHAtt.HideInInspector)
                EditorGUI.PropertyField(position, property, label, false);
            // else hide it ,dont draw it

            GUI.enabled = wasEnabled;
#endif
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
#if UNITY_EDITOR
            ShowIfComponentsAttribute condHAtt = (ShowIfComponentsAttribute)attribute;
            bool enabled = GetConditionalSourceField(property, condHAtt);

            // if is enable draw the label
            if (enabled)
            {
                return EditorGUI.GetPropertyHeight(property, label, true);
            }
            // if is not enabled but we want not hide it, then draw it disabled
            else
            {
                if (!condHAtt.HideInInspector)
                    return EditorGUI.GetPropertyHeight(property, label, false);
                // else hide it
                else
                    return -EditorGUIUtility.standardVerticalSpacing;
            }
#else
        return 0f;
#endif
        }
        private bool GetConditionalSourceField(SerializedProperty property, ShowIfComponentsAttribute condAtt)
        {
#if UNITY_EDITOR
            var targetObject = property?.serializedObject?.targetObject;
            if (targetObject is Component component)
            {
                switch (condAtt.EvaluationMode)
                {
                    case EvaluationModeType.AND:
                        {
                            if (condAtt.ExpectMissingComponents)
                            {
                                foreach (var t in condAtt.ComponentTypes)
                                {
                                    if (component.GetComponent(t) != null)
                                        return false;
                                }
                            }
                            else
                            {
                                foreach (var t in condAtt.ComponentTypes)
                                {
                                    if (component.GetComponent(t) == null)
                                        return false;
                                }
                            }
                        }
                        return true;
                    case EvaluationModeType.OR:
                        {
                            if (condAtt.ExpectMissingComponents)
                            {
                                foreach (var t in condAtt.ComponentTypes)
                                {
                                    if (component.GetComponent(t) == null)
                                        return true;
                                }
                            }
                            else
                            {
                                foreach (var t in condAtt.ComponentTypes)
                                {
                                    if (component.GetComponent(t) != null)
                                        return true;
                                }
                            }
                        }
                        return false;
                    default:
                        return true;
                }
            }
            else
            {
                Debug.LogWarning($"{nameof(ShowIfComponentsAttribute)} only usable for fields in {nameof(Component)} scripts.");
            }
            return true;
#else
        return false;
#endif
        }
    }
}
