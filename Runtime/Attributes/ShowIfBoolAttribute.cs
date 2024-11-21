using UnityEditor;
using UnityEngine;


namespace UnityUtilities.Attributes
{
    /// <summary>
    /// https://discussions.unity.com/t/custom-inspector-if-bool-is-true-then-show-variable/178698/3
    /// </summary>
    public class ShowIfBoolAttribute : PropertyAttribute
    {
        public string ConditionalSourceField;
        public bool ExpectedValue;
        public bool HideInInspector;
        public ShowIfBoolAttribute(string conditionalSourceField, bool expectedValue = true, bool hideInInspector = true)
        {
            this.ConditionalSourceField = conditionalSourceField;
            this.ExpectedValue = expectedValue;
            this.HideInInspector = hideInInspector;
        }
    }

    [CustomPropertyDrawer(typeof(ShowIfBoolAttribute))]
    public class ConditionalBoolHidePropertyDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
#if UNITY_EDITOR
            ShowIfBoolAttribute condHAtt = (ShowIfBoolAttribute)attribute;
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
            ShowIfBoolAttribute condHAtt = (ShowIfBoolAttribute)attribute;
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
        private bool GetConditionalSourceField(SerializedProperty property, ShowIfBoolAttribute condHAtt)
        {
#if UNITY_EDITOR
            bool enabled = false;
            string propertyPath = property.propertyPath;
            string conditionPath = propertyPath.Replace(property.name, condHAtt.ConditionalSourceField);
            SerializedProperty sourcePropertyValue = property.serializedObject.FindProperty(conditionPath);

            if (sourcePropertyValue != null)
            {
                enabled = sourcePropertyValue.boolValue;
                if (enabled == condHAtt.ExpectedValue) enabled = true;
                else enabled = false;
            }
            else
            {
                string warning = "ConditionalHideAttribute: No conditional field [" + condHAtt.ConditionalSourceField + "] not found in " + property.propertyPath;
                Debug.LogWarning(warning);
            }

            return enabled;
#else
        return false;
#endif
        }
    }
}