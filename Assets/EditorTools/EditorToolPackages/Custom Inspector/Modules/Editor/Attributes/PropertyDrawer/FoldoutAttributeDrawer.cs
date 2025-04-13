using CustomInspector.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace CustomInspector.Editor
{
    [CustomPropertyDrawer(typeof(FoldoutAttribute))]
    public class FoldoutAttributeDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            if (property.propertyType == SerializedPropertyType.Generic) //already has a foldout
            {
                DrawProperties.PropertyField(position, label, property);
                return;
            }
            else if (property.propertyType != SerializedPropertyType.ObjectReference)
            {
                DrawProperties.DrawPropertyWithMessage(position, label, property,
                    $"{nameof(FoldoutAttribute)}'s supported type is only ObjectReference and not {property.propertyType}", MessageType.Error);
                return;
            }

            if (!typeof(Object).IsAssignableFrom(fieldInfo.FieldType))
            {
                DrawProperties.DrawPropertyWithMessage(position, label, property,
                        $"{nameof(FoldoutAttribute)} is only available on UnityEngine.Object 's", MessageType.Error);
                return;
            }

            //Draw current
            Rect holdersRect = new(position)
            {
                height = DrawProperties.GetPropertyHeight(label, property)
            };

            Object value = property.objectReferenceValue;

            if (value == null) //nothing to foldout, bec its null
            {
                DrawProperties.PropertyField(position, label, property);
                property.isExpanded = false;
                return;
            }

            DrawProperties.PropertyFieldWithFoldout(holdersRect, label, property);

            //Draw Members
            using (new EditorGUI.IndentLevelScope(1))
            {
                Rect membersRect = EditorGUI.IndentedRect(position);
                using (new NewIndentLevel(0))
                {
                    membersRect.y = holdersRect.y + holdersRect.height + EditorGUIUtility.standardVerticalSpacing;
                    membersRect.height = position.height - holdersRect.height - EditorGUIUtility.standardVerticalSpacing;

                    if (property.isExpanded)
                    {
                        DrawMembers(membersRect, value);
                    }
                }
            }

            void DrawMembers(Rect position, Object displayedObject)
            {
                Debug.Assert(displayedObject != null, "No Object found to draw members on.");
                using (SerializedObject serializedObject = new(displayedObject))
                {
                    List<SerializedProperty> props = serializedObject.GetAllVisibleProperties(true).ToList();
                    if (props.Count <= 0)
                    {
                        Debug.LogWarning(NoPropsWarning(displayedObject));
                        property.isExpanded = false;
                        return;
                    }
                    EditorGUI.BeginChangeCheck();
                    foreach (SerializedProperty p in props)
                    {
                        position.height = DrawProperties.GetPropertyHeight(new GUIContent(p.name, p.tooltip), p);
                        DrawProperties.PropertyField(position, new GUIContent(p.name, p.tooltip), p);
                        position.y += position.height + EditorGUIUtility.standardVerticalSpacing;
                    }
                    if (EditorGUI.EndChangeCheck())
                        serializedObject.ApplyModifiedProperties();
                }
            }
        }
        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            if (property.propertyType == SerializedPropertyType.Generic) //already has a foldout
                return DrawProperties.GetPropertyHeight(label, property);
            else if (property.propertyType != SerializedPropertyType.ObjectReference
                     || !typeof(Object).IsAssignableFrom(fieldInfo.FieldType))
                return DrawProperties.GetPropertyWithMessageHeight(label, property);

            float currentHeight = DrawProperties.GetPropertyHeight(label, property);

            if (property.isExpanded && property.objectReferenceValue != null)
                currentHeight += GetMembersHeight(property.objectReferenceValue);

            return currentHeight;


            float GetMembersHeight(Object displayedObject)
            {
                Debug.Assert(displayedObject != null, "No Object found to search members on.");
                using (SerializedObject serializedObject = new(displayedObject))
                {
                    List<SerializedProperty> props = serializedObject.GetAllVisibleProperties(true).ToList();
                    if (props.Count <= 0)
                    {
                        Debug.LogWarning(NoPropsWarning(displayedObject));
                        property.isExpanded = false;
                        return 0;
                    }
                    return props.Select(p => DrawProperties.GetPropertyHeight(new GUIContent(p.name, p.tooltip), p))
                                .Sum(x => x + EditorGUIUtility.standardVerticalSpacing);
                }
            }
        }
        static string NoPropsWarning(Object target)
        {
            Type type = target.GetType();
            return nameof(FoldoutAttribute) + $": No properties found on {target.name} -> {type}." +
                                    $"\nPlease open the '{type}' script and make sure properties are public and serializable." +
                                    "\nPrivates can be serialized with the [SerializeField] attribute.";
        }
    }
}
