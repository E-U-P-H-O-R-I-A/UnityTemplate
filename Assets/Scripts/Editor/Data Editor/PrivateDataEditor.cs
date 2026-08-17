using System;
using UnityEditor;
using UnityEngine;

namespace Editor.DataEditor
{
    /// <summary>
    /// Window layout only: everything it shows comes from PrivateDataService,
    /// everything it draws inside the inspector comes from SchemeInspectorDrawer.
    /// </summary>
    public class PrivateDataEditor : EditorWindow
    {
        private const float SIDEBAR_WIDTH = 250f;
        private const float ROW_HEIGHT = 36f;
        private const float HEADER_HEIGHT = 34f;
        private const float SPACING = 8f;

        private readonly PrivateDataService service = new();
        private readonly SchemeInspectorDrawer inspector = new();

        private string searchQuery = string.Empty;
        private Vector2 fileScroll;
        private Vector2 inspectorScroll;
        private Action pendingAction;
        private PrivateDataSkin skin;

        [MenuItem("Tools/Data Utility")]
        public static void Open()
        {
            foreach (var opened in Resources.FindObjectsOfTypeAll<PrivateDataEditor>())
                opened.Close();

            var window = CreateInstance<PrivateDataEditor>();
            window.titleContent = new GUIContent("Data Utility", GetWindowIcon());
            window.minSize = new Vector2(780f, 520f);
            window.service.Refresh();
            window.ShowUtility();
        }

        private void OnEnable()
        {
            wantsMouseMove = true;
        }

        private void OnDisable()
        {
            skin?.Dispose();
            skin = null;
        }

        private void OnFocus()
        {
            if (!service.HasPendingChanges)
                service.Refresh();
        }

        private void OnGUI()
        {
            EnsureSkin();

            if (Event.current.type == EventType.Repaint)
                EditorGUI.DrawRect(new Rect(0f, 0f, position.width, position.height), skin.WindowBackground);

            DrawToolbar();

            using (new EditorGUILayout.HorizontalScope(skin.Body, GUILayout.ExpandHeight(true)))
            {
                DrawModelList();
                GUILayout.Space(SPACING);
                DrawInspectorPanel();
            }

            DrawFooter();

            if (Event.current.type == EventType.MouseMove)
                Repaint();

            FlushPendingAction();
        }

        private void DrawToolbar()
        {
            using (new EditorGUILayout.HorizontalScope(skin.Toolbar))
            {
                if (Event.current.type == EventType.Repaint)
                {
                    EditorGUI.DrawRect(
                        new Rect(0f, 0f, position.width, PrivateDataSkin.TOOLBAR_HEIGHT),
                        skin.ToolbarBackground);

                    EditorGUI.DrawRect(
                        new Rect(0f, PrivateDataSkin.TOOLBAR_HEIGHT - 1f, position.width, 1f),
                        skin.Border);
                }

                GUILayout.Label("Private Models", skin.ToolbarTitle, GUILayout.Height(22f));

                GUILayout.FlexibleSpace();

                using (new EditorGUILayout.VerticalScope(GUILayout.Height(22f)))
                {
                    GUILayout.Space(2f);
                    searchQuery = EditorGUILayout.TextField(
                        searchQuery,
                        EditorStyles.toolbarSearchField,
                        GUILayout.Width(200f));
                }
            }
        }

        private void DrawModelList()
        {
            var files = service.Filter(searchQuery);

            using (new EditorGUILayout.VerticalScope(skin.Panel, GUILayout.Width(SIDEBAR_WIDTH),
                       GUILayout.ExpandHeight(true)))
            {
                DrawPanelHeader("Models", null, DrawListActions);

                fileScroll = EditorGUILayout.BeginScrollView(fileScroll, GUIStyle.none, GUI.skin.verticalScrollbar);

                using (new EditorGUILayout.VerticalScope(skin.ScrollContent))
                {
                    if (files.Length == 0)
                    {
                        PrivateDataGUI.DrawEmptyState(skin, service.FileCount == 0
                            ? "No models yet"
                            : "Nothing matches the search");
                    }

                    foreach (var file in files)
                        DrawModelRow(file);
                }

                EditorGUILayout.EndScrollView();
            }
        }

        private void DrawModelRow(string file)
        {
            bool isSelected = file == service.SelectedPath;
            var rect = GUILayoutUtility.GetRect(0f, ROW_HEIGHT, GUILayout.ExpandWidth(true));
            bool isHovered = rect.Contains(Event.current.mousePosition);

            if (Event.current.type == EventType.Repaint)
            {
                if (isSelected)
                    skin.RowSelectedCard.Draw(rect, false, false, false, false);
                else if (isHovered)
                    skin.RowHoverCard.Draw(rect, false, false, false, false);

                float textX = rect.x + 12f;
                float textWidth = Mathf.Max(rect.xMax - 16f - textX, 20f);

                GUI.Label(new Rect(textX, rect.y + 4f, textWidth, 16f),
                    ModelNaming.GetDisplayName(file),
                    isSelected ? skin.RowTitleActive : skin.RowTitle);

                GUI.Label(new Rect(textX, rect.y + 19f, textWidth, 13f),
                    service.GetFileMeta(file),
                    skin.RowSubtitle);

                if (isSelected && service.HasPendingChanges)
                {
                    GUI.DrawTexture(
                        new Rect(rect.xMax - 15f, rect.y + rect.height * 0.5f - 3.5f, 7f, 7f),
                        skin.Dot);
                }
            }

            if (Event.current.type != EventType.MouseDown || Event.current.button != 0 || !isHovered)
                return;

            GUI.FocusControl(null);
            pendingAction = () => SelectModel(file);
            Event.current.Use();
        }

        private void DrawInspectorPanel()
        {
            using (new EditorGUILayout.VerticalScope(skin.Panel, GUILayout.ExpandHeight(true)))
            {
                DrawPanelHeader(GetInspectorTitle(), service.GetFileMeta(service.SelectedPath), DrawModelActions);

                inspectorScroll = EditorGUILayout.BeginScrollView(
                    inspectorScroll, GUIStyle.none, GUI.skin.verticalScrollbar);

                using (new EditorGUILayout.VerticalScope(skin.ScrollContent))
                {
                    if (!string.IsNullOrEmpty(service.LoadError))
                        EditorGUILayout.HelpBox(service.LoadError, MessageType.Error);
                    else if (service.SelectedModel == null)
                        PrivateDataGUI.DrawEmptyState(skin, "Select a model");
                    else
                        DrawSelectedModel();
                }

                EditorGUILayout.EndScrollView();
            }
        }

        private void DrawSelectedModel()
        {
            float labelWidth = Mathf.Clamp(position.width * 0.24f, 140f, 260f);

            if (inspector.Draw(service.SelectedModel, service.SelectedPath, skin, labelWidth))
                service.HasPendingChanges = true;
        }

        private void DrawListActions()
        {
            if (GUILayout.Button(skin.RefreshIcon, skin.IconButton))
                pendingAction = service.Refresh;

            if (GUILayout.Button(skin.FolderIcon, skin.IconButton))
                pendingAction = () => EditorUtility.RevealInFinder(PrivateDataService.FolderPath);

            using (new EditorGUI.DisabledScope(service.FileCount == 0))
            {
                if (GUILayout.Button(skin.DeleteAllIcon, skin.DangerIconButton))
                    pendingAction = DeleteAllModels;
            }
        }

        private void DrawModelActions()
        {
            using (new EditorGUI.DisabledScope(service.SelectedModelType == null))
            {
                if (GUILayout.Button(skin.ImportIcon, skin.IconButton))
                    pendingAction = ImportFromClipboard;
            }

            using (new EditorGUI.DisabledScope(service.SelectedModel == null))
            {
                if (GUILayout.Button(skin.ExportIcon, skin.IconButton))
                    pendingAction = ExportToClipboard;
            }

            using (new EditorGUI.DisabledScope(!service.HasSelection))
            {
                var saveStyle = service.HasPendingChanges ? skin.PrimaryIconButton : skin.IconButton;

                if (GUILayout.Button(skin.SaveIcon, saveStyle))
                    pendingAction = service.Save;

                if (GUILayout.Button(skin.DeleteIcon, skin.DangerIconButton))
                    pendingAction = DeleteSelectedModel;
            }
        }

        private void DrawPanelHeader(string title, string meta, Action drawActions = null)
        {
            using (new EditorGUILayout.HorizontalScope(skin.PanelHeader, GUILayout.Height(HEADER_HEIGHT)))
            {
                using (new EditorGUILayout.VerticalScope())
                {
                    GUILayout.FlexibleSpace();
                    GUILayout.Label(title, skin.PanelTitle);

                    if (!string.IsNullOrEmpty(meta))
                        GUILayout.Label(meta, skin.MetaLabel);

                    GUILayout.FlexibleSpace();
                }

                GUILayout.FlexibleSpace();

                using (new EditorGUILayout.VerticalScope(GUILayout.Height(HEADER_HEIGHT)))
                {
                    GUILayout.FlexibleSpace();

                    using (new EditorGUILayout.HorizontalScope())
                        drawActions?.Invoke();

                    GUILayout.FlexibleSpace();
                }
            }

            PrivateDataGUI.DrawSeparator(skin);
        }

        private void DrawFooter()
        {
            using (new EditorGUILayout.HorizontalScope(skin.Footer))
            {
                if (Event.current.type == EventType.Repaint)
                {
                    var background = new Rect(0f, position.height - PrivateDataSkin.FOOTER_HEIGHT,
                        position.width, PrivateDataSkin.FOOTER_HEIGHT);

                    EditorGUI.DrawRect(background, skin.ToolbarBackground);
                    EditorGUI.DrawRect(new Rect(background.x, background.y, background.width, 1f), skin.Border);
                }

                GUILayout.Label(PrivateDataService.FolderPath, skin.FooterLabel);
                GUILayout.FlexibleSpace();
                GUILayout.Label(
                    service.HasPendingChanges ? "Unsaved changes" : $"{service.FileCount} file(s)",
                    service.HasPendingChanges ? skin.FooterAccentLabel : skin.FooterLabel);
            }
        }

        private string GetInspectorTitle()
        {
            if (service.SelectedModelType != null)
                return ModelNaming.FormatTypeName(service.SelectedModelType.Name);

            return service.HasSelection
                ? ModelNaming.GetDisplayName(service.SelectedPath)
                : "Inspector";
        }

        private void SelectModel(string path)
        {
            service.Select(path);
            inspector.ResetFoldouts();
            inspectorScroll = Vector2.zero;
        }

        private void DeleteSelectedModel()
        {
            string fileName = ModelNaming.GetDisplayName(service.SelectedPath);

            bool confirmed = EditorUtility.DisplayDialog(
                "Delete Save",
                $"Delete save file of \"{fileName}\"?",
                "Delete",
                "Cancel");

            if (!confirmed)
                return;

            service.DeleteSelected();
            inspector.ResetFoldouts();
        }

        private void DeleteAllModels()
        {
            bool confirmed = EditorUtility.DisplayDialog(
                "Delete All Saves",
                "Delete all private save files?",
                "Delete All",
                "Cancel");

            if (!confirmed)
                return;

            service.DeleteAll();
            inspector.ResetFoldouts();
        }

        private void ImportFromClipboard()
        {
            string json = EditorGUIUtility.systemCopyBuffer;

            if (string.IsNullOrWhiteSpace(json))
            {
                ShowNotification(new GUIContent("Clipboard is empty"));
                return;
            }

            if (!service.TryImport(json, out string error))
            {
                EditorUtility.DisplayDialog("Import failed", error, "OK");
                return;
            }

            inspector.ResetFoldouts();
            ShowNotification(new GUIContent("JSON imported"));
        }

        private void ExportToClipboard()
        {
            string json = service.Export();

            if (string.IsNullOrEmpty(json))
                return;

            EditorGUIUtility.systemCopyBuffer = json;
            ShowNotification(new GUIContent("JSON copied"));
        }

        private void FlushPendingAction()
        {
            if (pendingAction == null)
                return;

            var action = pendingAction;
            pendingAction = null;

            action.Invoke();
            Repaint();
        }

        private void EnsureSkin()
        {
            if (skin == null || !skin.IsValid)
                skin = new PrivateDataSkin();
        }

        private static Texture GetWindowIcon() =>
            EditorGUIUtility.ObjectContent(null, typeof(TextAsset)).image;
    }
}
