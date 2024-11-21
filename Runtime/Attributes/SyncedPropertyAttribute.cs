using System;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace UnityUtilities.Attributes
{
    /// <summary>
    /// Syncs a property to another property.
    /// Ref: https://www.reddit.com/r/unity_tutorials/comments/zw4wfh/sync_inspector_fields_with_c_properties_using/
    /// </summary>
    [AttributeUsage(AttributeTargets.Field)]
    public class SyncedPropertyAttribute : PropertyAttribute
    {
        /// <summary>
        /// Name of Property to sync to.
        /// </summary>
        public string PropertyName;
        /// <summary>
        /// Component that owns the property.
        /// </summary>
        public Type ComponentType;
        /// <summary>
        /// Changes reflect in synced property.
        /// </summary>
        public bool TwoWay;
        /// <summary>
        /// Disabled property is using one-way sync mode.
        /// </summary>
        public bool OneWayDisabled;
        /// <summary>
        /// Disregards any errors while trying to sync providing default property behavior.
        /// </summary>
        public bool OptionalSync;
        public SyncedPropertyAttribute(string propertyName, Type componentType = null, bool twoWay = false, bool oneWayDisabled = true, bool optionalSync = false)
        {
            this.PropertyName = propertyName;
            this.ComponentType = componentType;
            this.TwoWay = twoWay;
            this.OneWayDisabled = oneWayDisabled;
            this.OptionalSync = optionalSync;
        }
    }

    [CustomPropertyDrawer(typeof(SyncedPropertyAttribute))]
    public class SyncedPropertyPropertyDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            var syncAtt = (attribute as SyncedPropertyAttribute);
            var propertyName = syncAtt.PropertyName;
            var componentType = syncAtt.ComponentType;
            var twoWay = syncAtt.TwoWay;
            var optionalSync = syncAtt.OptionalSync;

            var target = property.serializedObject.targetObject;
            UnityEngine.Object propertyObject = target;
            bool validSync = true;
            if(componentType != null)
            {
                if(componentType != target.GetType())
                {
                    if(target is Component component)
                    {
                        var targetComponent = component.GetComponent(componentType);
                        if (targetComponent != null)
                            propertyObject = targetComponent;
                        else
                        {
                            validSync = false;
                            if (!optionalSync)
                            {
                                var rect = GUILayoutUtility.GetRect(position.width, 30);
                                EditorGUI.HelpBox(rect, $"{componentType} could not be found.", MessageType.Error);
                            }
                        }
                    }
                    else
                    {

                        validSync = false;
                        if (!optionalSync)
                        {
                            var rect = GUILayoutUtility.GetRect(position.width, 30);
                            EditorGUI.HelpBox(rect, $"{nameof(SyncedPropertyAttribute.ComponentType)} is only valid for {nameof(Component)} types.", MessageType.Error);
                        }
                    }
                }
            }

            var flags = BindingFlags.Public | BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.GetProperty | BindingFlags.SetProperty | BindingFlags.Static;

            PropertyInfo propertyInfo = null;
            bool isValid = false;
            if (validSync)
            {
                propertyInfo = propertyObject.GetType().GetProperty(propertyName, flags);
                isValid = propertyInfo != null && propertyInfo.PropertyType.Equals(fieldInfo.FieldType);
                if (isValid)
                {
                    fieldInfo.SetValue(target, propertyInfo.GetValue(propertyObject));
                }
                else
                {
                    if (!optionalSync)
                    {
                        var rect = GUILayoutUtility.GetRect(position.width, 30);
                        EditorGUI.HelpBox(rect, $"Invalid Sync Property: {propertyName}", MessageType.Error);
                    }
                }
            }

            EditorGUI.BeginChangeCheck();
            EditorGUI.BeginDisabledGroup(!twoWay && syncAtt.OneWayDisabled && isValid);
            EditorGUI.PropertyField(position, property, label);
            EditorGUI.EndDisabledGroup();
            if (EditorGUI.EndChangeCheck())
            {
                property.serializedObject.ApplyModifiedProperties();
                if(twoWay && isValid)
                    propertyInfo.SetValue(propertyObject, fieldInfo.GetValue(target));

                EditorUtility.SetDirty(target);
            }
        }
    }
}
