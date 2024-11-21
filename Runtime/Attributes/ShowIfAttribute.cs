using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace UnityUtilities.Attributes
{
    public class ShowIfAttribute : PropertyAttribute
    {
        public struct Data
        {
            public string sourceName;
            public string evaluationFunction;
        }
        public enum EvaluationModeType
        {
            AND, OR
        }
        public string[] EvaluationFunctions;
        public bool HideInInspector;
        public EvaluationModeType EvaluationMode;
        public ShowIfAttribute(
            EvaluationModeType evaluationMode = ShowIfAttribute.EvaluationModeType.AND,
            bool hideInInspector = true,
            params string[] evaluationFunctions)
        {
            this.EvaluationFunctions = evaluationFunctions;
            this.HideInInspector = hideInInspector;
            this.EvaluationMode = evaluationMode;
        }
    }

    [CustomPropertyDrawer(typeof(ShowIfAttribute))]
    public class ConditionalHidePropertyDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
#if UNITY_EDITOR
            ShowIfAttribute condHAtt = (ShowIfAttribute)attribute;
            bool enabled = EvaluateFunctions(property, condHAtt);
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
            ShowIfAttribute condHAtt = (ShowIfAttribute)attribute;
            bool enabled = EvaluateFunctions(property, condHAtt);

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
        private bool FindAndEvaluateFunction(string functionName, IEnumerable<string> instancePath, object instance)
        {
            Type declaringType = fieldInfo.DeclaringType;
            if (declaringType != null)
            {
                instance = ResolveObjectPath(instancePath.ToArray(), instance);
                // Get the evaluation function method with the given name
                MethodInfo evaluationFunction = declaringType.GetMethod(functionName, BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);

                // Check if the evaluation function exists and is compatible with the expected signature
                if (evaluationFunction != null && evaluationFunction.ReturnType == typeof(bool))
                {
                    // Invoke the evaluation function with the provided parameter
                    return (bool)evaluationFunction.Invoke(instance, new object[] { });
                }
            }
            Debug.LogWarning("Error finding or evaluating ShowIf EvaluationFunction");

            // If the evaluation function doesn't exist or has an incompatible signature, return false
            return false;
        }
        private object ResolveObjectPath(string[] objectPath, object startInstance)
        {
            object currentInstance = startInstance;
            for (int i = 0; i < objectPath.Length; ++i)
            {
                PropertyInfo property = currentInstance.GetType().GetProperty(objectPath[i], BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);
                if (property != null)
                {
                    // Get the struct instance from the property
                    currentInstance = property.GetValue(currentInstance);
                    continue;
                }
                FieldInfo field = currentInstance.GetType().GetField(objectPath[i], BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);
                if (field != null)
                {
                    // Get the struct instance from the property
                    currentInstance = field.GetValue(currentInstance);
                    continue;
                }
            }
            return currentInstance;
        }
        private bool EvaluateFunctions(SerializedProperty property, ShowIfAttribute condHAtt)
        {
#if UNITY_EDITOR
            string propertyPath = property.propertyPath;
            var instancePath = propertyPath.Split(".").SkipLast(1);
            switch (condHAtt.EvaluationMode)
            {
                case ShowIfAttribute.EvaluationModeType.AND:
                    {
                        foreach (var ef in condHAtt.EvaluationFunctions)
                        {

                            //string conditionPath = propertyPath.Replace(property.name, ef.sourceName);
                            //SerializedProperty sourcePropertyValue = property.serializedObject.FindProperty(conditionPath);
                            if (!FindAndEvaluateFunction(ef, instancePath, property.serializedObject.targetObject))
                                return false;
                        }
                    }
                    return true;
                case ShowIfAttribute.EvaluationModeType.OR:
                    {
                        foreach (var ef in condHAtt.EvaluationFunctions)
                        {
                            if (FindAndEvaluateFunction(ef, instancePath, property.serializedObject.targetObject))
                                return true;
                        }
                    }
                    return false;
                default:
                    return false;
            }
#else
return false;
#endif

        }

    }
}