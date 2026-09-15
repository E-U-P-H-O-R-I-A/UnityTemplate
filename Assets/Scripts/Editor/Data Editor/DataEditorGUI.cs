using UnityEditor;
using UnityEngine;

namespace EditorTools.DataEditor
{
    public static class DataEditorGUI
    {
        public static void DrawSeparator(DataEditorSkin skin)
        {
            var rect = GUILayoutUtility.GetRect(1f, 1f, GUILayout.ExpandWidth(true));

            if (Event.current.type == EventType.Repaint)
                EditorGUI.DrawRect(rect, skin.Separator);
        }

        public static void DrawEmptyState(DataEditorSkin skin, string message)
        {
            GUILayout.Space(16f);
            GUILayout.Label(message, skin.EmptyLabel);
        }
    }
}
