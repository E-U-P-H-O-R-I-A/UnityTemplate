using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

namespace Editor.Data_Editor
{
    public class RecordPropertyInspector
    {
        private const float SECTION_HEADER_HEIGHT = 20f;
        private const float SMALL_BUTTON_WIDTH = 22f;
        private const string RECORDS_PROPERTY = "records";
        private const string SINGLE_PROPERTY = "record";
        private const string ID_PROPERTY = "id";
        private const string NEW_RECORD_ID = "NewRecord";
        private const float LIST_HEADER_HEIGHT = 20f;
        private const float ELEMENT_PADDING = 3f;
        private const float ROW_SPACING = 2f;
        private const float ELEMENT_TITLE_HEIGHT = 18f;
        private const float MIN_LABEL_WIDTH = 60f;

        private readonly Dictionary<string, bool> foldouts = new();
        private readonly Dictionary<string, ReorderableList> lists = new();

        private DataEditorSkin skin;
        private Action pending;
        private float contentLeft;

        public void ResetFoldouts()
        {
            foldouts.Clear();
            lists.Clear();
        }

        public bool TryTakePendingAction(out Action action)
        {
            action = pending;
            pending = null;

            return action != null;
        }

        public bool Draw(SerializedObject serializedObject, DataEditorSkin currentSkin, float labelWidth)
        {
            skin = currentSkin;

            serializedObject.Update();

            var records = serializedObject.FindProperty(RECORDS_PROPERTY);
            var single = serializedObject.FindProperty(SINGLE_PROPERTY);

            if (records == null && single == null)
            {
                DataEditorGUI.DrawEmptyState(skin, "This container has no records");
                return false;
            }

            float previousLabelWidth = EditorGUIUtility.labelWidth;
            EditorGUIUtility.labelWidth = labelWidth;

            if (records != null && records.isArray)
                DrawCollection(serializedObject, records);
            else
                DrawSingle(single);

            EditorGUIUtility.labelWidth = previousLabelWidth;

            return serializedObject.ApplyModifiedProperties();
        }

        private void DrawCollection(SerializedObject serializedObject, SerializedProperty records)
        {
            DrawAddRecordRow(serializedObject);

            if (records.arraySize == 0)
                DataEditorGUI.DrawEmptyState(skin, "No records yet, add the first one");

            for (int i = 0; i < records.arraySize; i++)
                DrawRecord(serializedObject, records, i);
        }

        private void DrawSingle(SerializedProperty record)
        {
            using (new EditorGUILayout.VerticalScope(skin.Section))
            using (new EditorGUILayout.VerticalScope(skin.SectionContent))
                DrawRecordFields(record);
        }

        private void DrawAddRecordRow(SerializedObject serializedObject)
        {
            using (new EditorGUILayout.HorizontalScope())
            {
                GUILayout.FlexibleSpace();

                if (GUILayout.Button("Add Record", skin.SmallButton))
                    pending = () => AddRecord(serializedObject);
            }

            GUILayout.Space(2f);
        }

        private void DrawRecord(SerializedObject serializedObject, SerializedProperty records, int index)
        {
            var element = records.GetArrayElementAtIndex(index);
            var idProperty = element.FindPropertyRelative(ID_PROPERTY);

            string label = string.IsNullOrEmpty(idProperty?.stringValue) ? "(no id)" : idProperty.stringValue;
            string key = $"{serializedObject.targetObject.GetInstanceID()}:{index}";

            using (new EditorGUILayout.VerticalScope(skin.Section))
            {
                if (!DrawSectionHeader(label, key, serializedObject, index))
                    return;

                GUILayout.Space(2f);

                using (new EditorGUILayout.VerticalScope(skin.SectionContent))
                {
                    var probe = GUILayoutUtility.GetRect(0f, 0f);

                    if (probe.width > 1f)
                        contentLeft = probe.x;

                    DrawRecordFields(element);
                }
            }
        }

        private void DrawRecordFields(SerializedProperty record)
        {
            var iterator = record.Copy();
            var end = record.GetEndProperty();
            bool enterChildren = true;

            while (iterator.NextVisible(enterChildren) && !SerializedProperty.EqualContents(iterator, end))
            {
                DrawProperty(iterator);
                enterChildren = false;
            }
        }

        private void DrawProperty(SerializedProperty property)
        {
            if (TryGetSingleChild(property, out var child))
            {
                DrawNamed(child, property.displayName);
                return;
            }

            DrawNamed(property, property.displayName);
        }

        private void DrawNamed(SerializedProperty property, string label)
        {
            if (property.isArray && property.propertyType == SerializedPropertyType.Generic)
            {
                DrawList(property, label);
                return;
            }

            EditorGUILayout.PropertyField(property, new GUIContent(label), true);
        }

        private void DrawList(SerializedProperty property, string label)
        {
            var rect = GUILayoutUtility.GetRect(0f, LIST_HEADER_HEIGHT, GUILayout.ExpandWidth(true));

            float buttonX = rect.xMax - SMALL_BUTTON_WIDTH;

            property.isExpanded = EditorGUI.Foldout(
                new Rect(rect.x, rect.y + 2f, Mathf.Max(buttonX - rect.x - 4f, 20f), 16f),
                property.isExpanded,
                label,
                true,
                skin.NestedFoldout);

            if (GUI.Button(new Rect(buttonX, rect.y + 2f, SMALL_BUTTON_WIDTH, 16f), skin.AddIcon, skin.SmallButton))
                RequestResize(property, property.arraySize + 1);

            if (!property.isExpanded)
                return;

            GetList(property).DoLayoutList();
        }

        private ReorderableList GetList(SerializedProperty property)
        {
            string key = ListKey(property);

            if (lists.TryGetValue(key, out var cached))
                return cached;

            var list = new ReorderableList(property.serializedObject, property.Copy(), true, false, false, false)
            {
                headerHeight = 0f,
                footerHeight = 0f,
                showDefaultBackground = false
            };

            list.elementHeightCallback = index => ElementHeight(list, index);
            list.drawElementCallback = (rect, index, _, _) => DrawElement(list, rect, index);
            list.drawElementBackgroundCallback = (rect, _, active, _) => DrawElementBackground(rect, active);

            lists[key] = list;

            return list;
        }

        private static float ElementHeight(ReorderableList list, int index)
        {
            if (index < 0 || index >= list.serializedProperty.arraySize)
                return EditorGUIUtility.singleLineHeight;

            var element = list.serializedProperty.GetArrayElementAtIndex(index);
            float height = ELEMENT_TITLE_HEIGHT + ROW_SPACING + ELEMENT_PADDING * 2f;

            if (!IsFlattened(element))
                return height + EditorGUI.GetPropertyHeight(element, true);

            foreach (var child in Children(element))
                height += EditorGUI.GetPropertyHeight(child, true) + ROW_SPACING;

            return height - ROW_SPACING;
        }

        private void DrawElement(ReorderableList list, Rect rect, int index)
        {
            if (index < 0 || index >= list.serializedProperty.arraySize)
                return;

            var element = list.serializedProperty.GetArrayElementAtIndex(index);

            float buttonX = rect.xMax - SMALL_BUTTON_WIDTH;
            float width = Mathf.Max(buttonX - rect.x - 6f, 20f);
            float top = rect.y + ELEMENT_PADDING;

            GUI.Label(new Rect(rect.x, top, width, ELEMENT_TITLE_HEIGHT), $"Element {index}", skin.RowTitle);

            if (GUI.Button(new Rect(buttonX, top + 1f, SMALL_BUTTON_WIDTH, 16f), skin.RemoveIcon, skin.SmallButton))
                RequestDelete(list.serializedProperty, index);

            var content = new Rect(rect.x, top + ELEMENT_TITLE_HEIGHT + ROW_SPACING, width,
                rect.height - ELEMENT_TITLE_HEIGHT - ROW_SPACING - ELEMENT_PADDING * 2f);

            float previousLabelWidth = EditorGUIUtility.labelWidth;
            EditorGUIUtility.labelWidth = Mathf.Max(previousLabelWidth - (rect.x - contentLeft), MIN_LABEL_WIDTH);

            if (IsFlattened(element))
                DrawElementRows(element, content);
            else
                EditorGUI.PropertyField(content, element, GUIContent.none, true);

            EditorGUIUtility.labelWidth = previousLabelWidth;
        }

        private static void DrawElementRows(SerializedProperty element, Rect rect)
        {
            float y = rect.y;

            foreach (var child in Children(element))
            {
                float height = EditorGUI.GetPropertyHeight(child, true);

                EditorGUI.PropertyField(new Rect(rect.x, y, rect.width, height), child, true);

                y += height + ROW_SPACING;
            }
        }

        private static bool IsFlattened(SerializedProperty element) =>
            element.propertyType == SerializedPropertyType.Generic && !element.isArray && element.hasVisibleChildren;

        private static IEnumerable<SerializedProperty> Children(SerializedProperty property)
        {
            var iterator = property.Copy();
            var end = property.GetEndProperty();
            bool enterChildren = true;

            while (iterator.NextVisible(enterChildren) && !SerializedProperty.EqualContents(iterator, end))
            {
                enterChildren = false;

                yield return iterator.Copy();
            }
        }

        private void DrawElementBackground(Rect rect, bool active)
        {
            if (Event.current.type != EventType.Repaint)
                return;

            var card = new Rect(rect.x, rect.y, rect.width - 2f, rect.height - 2f);

            if (active)
                skin.RowSelectedCard.Draw(card, false, false, false, false);
            else if (card.Contains(Event.current.mousePosition))
                skin.RowHoverCard.Draw(card, false, false, false, false);
        }

        private void RequestResize(SerializedProperty property, int size)
        {
            var target = property.serializedObject;
            string path = property.propertyPath;

            pending = () =>
            {
                var list = target.FindProperty(path);

                if (list == null)
                    return;

                list.arraySize = size;
                target.ApplyModifiedProperties();
            };
        }

        private void RequestDelete(SerializedProperty property, int index)
        {
            var target = property.serializedObject;
            string path = property.propertyPath;

            pending = () =>
            {
                var list = target.FindProperty(path);

                if (list == null || index < 0 || index >= list.arraySize)
                    return;

                list.DeleteArrayElementAtIndex(index);
                target.ApplyModifiedProperties();
            };
        }

        private static string ListKey(SerializedProperty property) =>
            $"{property.serializedObject.targetObject.GetInstanceID()}:{property.propertyPath}";

        private static bool TryGetSingleChild(SerializedProperty property, out SerializedProperty child)
        {
            child = null;

            if (!IsFlattened(property))
                return false;

            int count = 0;

            foreach (var candidate in Children(property))
            {
                count++;

                if (count > 1)
                    return false;

                child = candidate;
            }

            return count == 1;
        }

        private bool DrawSectionHeader(string label, string key, SerializedObject serializedObject, int index)
        {
            foldouts.TryGetValue(key, out bool isExpanded);

            var rect = GUILayoutUtility.GetRect(0f, SECTION_HEADER_HEIGHT, GUILayout.ExpandWidth(true));

            if (Event.current.type == EventType.Repaint && rect.Contains(Event.current.mousePosition))
                EditorGUI.DrawRect(rect, skin.RowHover);

            isExpanded = EditorGUI.Foldout(
                new Rect(rect.x + 2f, rect.y + 2f, Mathf.Max(rect.width - 4f - SMALL_BUTTON_WIDTH, 20f), 16f),
                isExpanded,
                label,
                true,
                skin.SectionFoldout);

            foldouts[key] = isExpanded;

            var buttonRect = new Rect(rect.xMax - SMALL_BUTTON_WIDTH - 2f, rect.y + 2f, SMALL_BUTTON_WIDTH, 16f);

            if (GUI.Button(buttonRect, skin.RemoveIcon, skin.SmallButton))
                pending = () => RemoveRecord(serializedObject, index);

            return isExpanded;
        }

        private static void AddRecord(SerializedObject serializedObject)
        {
            var records = serializedObject.FindProperty(RECORDS_PROPERTY);
            if (records == null)
                return;

            records.arraySize++;

            var added = records.GetArrayElementAtIndex(records.arraySize - 1);
            var idProperty = added.FindPropertyRelative(ID_PROPERTY);

            if (idProperty != null)
                idProperty.stringValue = GetFreeRecordId(records);

            serializedObject.ApplyModifiedProperties();
        }

        private static void RemoveRecord(SerializedObject serializedObject, int index)
        {
            var records = serializedObject.FindProperty(RECORDS_PROPERTY);

            if (records == null || index < 0 || index >= records.arraySize)
                return;

            records.DeleteArrayElementAtIndex(index);
            serializedObject.ApplyModifiedProperties();
        }

        private static string GetFreeRecordId(SerializedProperty records)
        {
            var taken = new HashSet<string>();

            for (int i = 0; i < records.arraySize; i++)
            {
                var idProperty = records.GetArrayElementAtIndex(i).FindPropertyRelative(ID_PROPERTY);

                if (idProperty != null)
                    taken.Add(idProperty.stringValue);
            }

            if (!taken.Contains(NEW_RECORD_ID))
                return NEW_RECORD_ID;

            for (int index = 1; index < int.MaxValue; index++)
            {
                string candidate = $"{NEW_RECORD_ID}{index}";

                if (!taken.Contains(candidate))
                    return candidate;
            }

            return NEW_RECORD_ID;
        }
    }
}
