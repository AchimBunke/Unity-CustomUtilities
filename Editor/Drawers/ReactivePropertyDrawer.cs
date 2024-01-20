using System.Collections;
using System.Collections.Generic;
using UnityEditor.UIElements;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

#if USE_UNIRX_7_1
using UniRx;


namespace UnityUtilities.Drawers
{
    /// <summary>
    /// Draws a PropertyDrawer for a <see cref="ReactiveProperty{T}"/> which does not need to be expanded to edit the value.
    /// </summary>
    [CustomPropertyDrawer(typeof(ReactiveProperty<>))]
    public class ReactivePropertyDrawer : PropertyDrawer
    {
        public override VisualElement CreatePropertyGUI(SerializedProperty property)
        {
            return new PropertyField(property.FindPropertyRelative("value"), property.displayName);
        }
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginProperty(position, label, property);
            EditorGUI.PropertyField(position, property.FindPropertyRelative("value"), new GUIContent(property.displayName), true);
            EditorGUI.EndProperty();
        }
    }
}
#endif
