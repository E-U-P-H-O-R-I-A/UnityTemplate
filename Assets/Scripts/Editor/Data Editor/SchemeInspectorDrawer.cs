using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Data;
using UnityEditor;
using UnityEngine;

namespace Editor.Data_Editor
{
    public class SchemeInspectorDrawer
    {
        private const float SECTION_HEADER_HEIGHT = 20f;

        private readonly Dictionary<string, bool> foldouts = new();

        private DataEditorSkin skin;
        private int rowIndex;
        private bool changed;

        public void ResetFoldouts()
        {
            foldouts.Clear();
        }

        public bool Draw(object model, string ownerKey, DataEditorSkin currentSkin, float labelWidth)
        {
            skin = currentSkin;
            changed = false;
            rowIndex = 0;

            var schemes = SchemeReflection.GetSchemes(model).ToList();

            if (schemes.Count == 0)
            {
                DataEditorGUI.DrawEmptyState(skin, "This save has no serialized schemes");
                return false;
            }

            float previousLabelWidth = EditorGUIUtility.labelWidth;
            EditorGUIUtility.labelWidth = labelWidth;

            if (SchemeReflection.IsCollection(model))
                DrawCollection(schemes, ownerKey);
            else
                DrawSingle(schemes[0], ownerKey);

            EditorGUIUtility.labelWidth = previousLabelWidth;

            return changed;
        }

        private void DrawCollection(List<IScheme> schemes, string ownerKey)
        {
            for (int i = 0; i < schemes.Count; i++)
            {
                var scheme = schemes[i];
                string key = $"{ownerKey}:{i}";

                using (new EditorGUILayout.VerticalScope(skin.Section))
                {
                    if (!DrawSectionHeader(scheme.ID, key))
                        continue;

                    GUILayout.Space(2f);
                    DrawSerializableFields(scheme, key);
                }
            }
        }

        private void DrawSingle(IScheme scheme, string ownerKey)
        {
            using (new EditorGUILayout.VerticalScope(skin.Section))
            using (new EditorGUILayout.VerticalScope(skin.SectionContent))
                DrawSerializableFields(scheme, ownerKey);
        }

        private bool DrawSectionHeader(string label, string key)
        {
            foldouts.TryGetValue(key, out bool isExpanded);

            var rect = GUILayoutUtility.GetRect(0f, SECTION_HEADER_HEIGHT, GUILayout.ExpandWidth(true));

            if (Event.current.type == EventType.Repaint && rect.Contains(Event.current.mousePosition))
                EditorGUI.DrawRect(rect, skin.RowHover);

            isExpanded = EditorGUI.Foldout(
                new Rect(rect.x + 2f, rect.y + 2f, Mathf.Max(rect.width - 4f, 20f), 16f),
                isExpanded,
                string.IsNullOrEmpty(label) ? "(no id)" : label,
                true,
                skin.SectionFoldout);

            foldouts[key] = isExpanded;

            return isExpanded;
        }

        private void DrawSerializableFields(object target, string path)
        {
            foreach (var field in SchemeReflection.GetSerializableFields(target.GetType()))
            {
                DrawField(target, field, $"{path}.{field.Name}");
            }
        }

        private void DrawField(object target, FieldInfo field, string path)
        {
            object currentValue = field.GetValue(target);
            string label = ObjectNames.NicifyVariableName(field.Name);

            EditorGUI.BeginChangeCheck();

            object nextValue = DrawTypedValue(field, label, currentValue, path);

            if (!EditorGUI.EndChangeCheck())
                return;

            field.SetValue(target, nextValue);
            changed = true;
        }

        private object DrawTypedValue(FieldInfo field, string label, object currentValue, string path)
        {
            if (IsInlineType(field.FieldType))
            {
                using (new EditorGUILayout.VerticalScope(NextRowStyle()))
                    return DrawValue(label, field.FieldType, currentValue, path);
            }

            if (typeof(IList).IsAssignableFrom(field.FieldType))
            {
                DrawList(label, currentValue as IList, path);
                return currentValue;
            }

            return DrawValue(label, field.FieldType, currentValue, path);
        }

        private object DrawValue(string label, Type type, object value, string path)
        {
            if (type == typeof(bool))
                return EditorGUILayout.Toggle(label, value is true);

            if (type == typeof(int))
                return EditorGUILayout.IntField(label, value is int intValue ? intValue : 0);

            if (type == typeof(float))
                return EditorGUILayout.FloatField(label, value is float floatValue ? floatValue : 0f);

            if (type == typeof(double))
                return EditorGUILayout.DoubleField(label, value is double doubleValue ? doubleValue : 0d);

            if (type == typeof(long))
                return EditorGUILayout.LongField(label, value is long longValue ? longValue : 0L);

            if (type == typeof(string))
                return EditorGUILayout.TextField(label, value as string ?? string.Empty);

            if (type.IsEnum)
            {
                var enumValue = value as Enum ?? (Enum)Enum.GetValues(type).GetValue(0);
                return EditorGUILayout.EnumPopup(label, enumValue);
            }

            if (type == typeof(Vector2))
                return EditorGUILayout.Vector2Field(label, value is Vector2 vector ? vector : Vector2.zero);

            if (type == typeof(Vector3))
                return EditorGUILayout.Vector3Field(label, value is Vector3 vector ? vector : Vector3.zero);

            if (type == typeof(Color))
                return EditorGUILayout.ColorField(label, value is Color color ? color : Color.white);

            if (value == null)
            {
                using (new EditorGUILayout.VerticalScope(NextRowStyle()))
                    EditorGUILayout.LabelField(label, "null");

                return null;
            }

            if (DrawNestedFoldout(label, path))
            {
                EditorGUI.indentLevel++;
                DrawSerializableFields(value, path);
                EditorGUI.indentLevel--;
            }

            return value;
        }

        private void DrawList(string label, IList list, string path)
        {
            if (!DrawNestedFoldout($"{label} ({list?.Count ?? 0})", path) || list == null)
                return;

            EditorGUI.indentLevel++;

            if (list.Count == 0)
                EditorGUILayout.LabelField(" ", "Empty", skin.InlineHint);

            for (int i = 0; i < list.Count; i++)
            {
                object element = list[i];
                string elementPath = $"{path}[{i}]";
                var type = element?.GetType();

                if (type == null)
                {
                    EditorGUILayout.LabelField($"Element {i}", "null");
                    continue;
                }

                EditorGUI.BeginChangeCheck();
                object nextValue = DrawValue($"Element {i}", type, element, elementPath);

                if (!EditorGUI.EndChangeCheck())
                    continue;

                list[i] = nextValue;
                changed = true;
            }

            EditorGUI.indentLevel--;
        }

        private bool DrawNestedFoldout(string label, string path)
        {
            foldouts.TryGetValue(path, out bool isExpanded);

            GUILayout.Space(2f);
            isExpanded = EditorGUILayout.Foldout(isExpanded, label, true, skin.NestedFoldout);
            foldouts[path] = isExpanded;

            return isExpanded;
        }

        private GUIStyle NextRowStyle() =>
            rowIndex++ % 2 == 0 ? skin.RowEven : skin.RowOdd;

        private static bool IsInlineType(Type type)
        {
            return type == typeof(bool)
                   || type == typeof(int)
                   || type == typeof(float)
                   || type == typeof(double)
                   || type == typeof(long)
                   || type == typeof(string)
                   || type == typeof(Vector2)
                   || type == typeof(Vector3)
                   || type == typeof(Color)
                   || type.IsEnum;
        }
    }
}
