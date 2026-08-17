using UnityEditor;
using UnityEngine;

namespace Editor.DataEditor
{
    /// <summary>
    /// Small drawing primitives shared by the window and the scheme inspector.
    /// </summary>
    public static class PrivateDataGUI
    {
        public static void DrawSeparator(PrivateDataSkin skin)
        {
            var rect = GUILayoutUtility.GetRect(1f, 1f, GUILayout.ExpandWidth(true));

            if (Event.current.type == EventType.Repaint)
                EditorGUI.DrawRect(rect, skin.Separator);
        }

        public static void DrawEmptyState(PrivateDataSkin skin, string message)
        {
            GUILayout.Space(16f);
            GUILayout.Label(message, skin.EmptyLabel);
        }
    }
}
