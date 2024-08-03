using UnityEditor;
using UnityEngine;


namespace UnityUtilities.Attributes
{
    public class ShowIfEnumAttribute : PropertyAttribute
    {
        public string EnumFieldName;
        public int[] EnumValues;
        public bool HideInInspector;
        public ShowIfEnumAttribute(string enumFieldName, bool hideInInspector = true, params int[] enumValues)
        {
            this.EnumFieldName = enumFieldName;
            this.EnumValues = enumValues;
            this.HideInInspector = hideInInspector;
        }
    }

    [CustomPropertyDrawer(typeof(ShowIfEnumAttribute))]
    public class ConditionalEnumHidePropertyDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
#if UNITY_EDITOR
            ShowIfEnumAttribute condHAtt = (ShowIfEnumAttribute)attribute;
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
            ShowIfEnumAttribute condHAtt = (ShowIfEnumAttribute)attribute;
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
        private bool GetConditionalSourceField(SerializedProperty property, ShowIfEnumAttribute condHAtt)
        {
#if UNITY_EDITOR
            bool enabled = false;
            string propertyPath = property.propertyPath;
            string conditionPath = propertyPath.Replace(property.name, condHAtt.EnumFieldName);
            SerializedProperty sourcePropertyValue = property.serializedObject.FindProperty(conditionPath);

            if (sourcePropertyValue != null)
            {
                if (sourcePropertyValue.propertyType == SerializedPropertyType.Enum)
                {
                    int enumValue = sourcePropertyValue.enumValueIndex;
                    foreach (int value in condHAtt.EnumValues)
                    {
                        if (enumValue == value)
                        {
                            return true;
                        }
                    }
                }
                else if (sourcePropertyValue.propertyType == SerializedPropertyType.Integer)
                {
                    int flagValue = sourcePropertyValue.intValue;
                    foreach (int value in condHAtt.EnumValues)
                    {
                        if ((flagValue & value) != 0)
                        {
                            return true;
                        }
                    }
                }
            }
            else
            {
                string warning = "ConditionalHideAttribute: Conditional field [" + condHAtt.EnumFieldName + "] not found in " + property.propertyPath;
                Debug.LogWarning(warning);
            }

            return enabled;
#else
        return false;
#endif
        }
    }

}