using System;
using UnityEditor;
using UnityEngine;

namespace Editor.Data_Editor
{
    public class DataUtilityWindow : EditorWindow
    {
        private const float SIDEBAR_WIDTH = 250f;
        private const float ROW_HEIGHT = 36f;
        private const float HEADER_HEIGHT = 34f;
        private const float SPACING = 8f;
        private const float TAB_WIDTH = 84f;
        private const float TAB_LEFT = 10f;
        private const float TAB_TOP = 5f;
        private const float TAB_SPACING = 2f;
        private const float SEARCH_WIDTH = 200f;

        private readonly DataContainerService[] services =
        {
            new PublicDataService(),
            new PrivateDataService()
        };

        private readonly RecordInspectorDrawer inspector = new();
        private readonly RecordPropertyInspector propertyInspector = new();

        private string searchQuery = string.Empty;
        private Vector2 fileScroll;
        private Vector2 inspectorScroll;
        private Action pendingAction;
        private DataEditorSkin skin;
        private int modeIndex;

        private DataContainerService Service => services[modeIndex];

        [MenuItem("Tools/Data Utility")]
        public static void Open()
        {
            var window = GetWindow<DataUtilityWindow>(true, "Data Utility", true);
            window.titleContent = new GUIContent("Data Utility");
            window.minSize = new Vector2(780f, 520f);
            window.Service.Refresh();
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
            if (!Service.HasPendingChanges)
                Service.Refresh();
        }

        private void OnGUI()
        {
            EnsureSkin();

            if (Event.current.type == EventType.Repaint)
                EditorGUI.DrawRect(new Rect(0f, 0f, position.width, position.height), skin.WindowBackground);

            DrawToolbar();

            using (new EditorGUILayout.HorizontalScope(skin.Body, GUILayout.ExpandHeight(true)))
            {
                DrawContainerList();
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
            var toolbar = GUILayoutUtility.GetRect(0f, DataEditorSkin.TOOLBAR_HEIGHT, GUILayout.ExpandWidth(true));

            if (Event.current.type == EventType.Repaint)
                EditorGUI.DrawRect(toolbar, skin.ToolbarBackground);

            DrawTabs(toolbar);

            var searchRect = new Rect(toolbar.xMax - SEARCH_WIDTH - TAB_LEFT, toolbar.y + 6f, SEARCH_WIDTH, 20f);
            searchQuery = EditorGUI.TextField(searchRect, searchQuery, EditorStyles.toolbarSearchField);
        }

        private void DrawTabs(Rect toolbar)
        {
            float x = toolbar.x + TAB_LEFT;
            var active = Rect.zero;

            for (int index = 0; index < services.Length; index++)
            {
                var tab = new Rect(x, toolbar.y + TAB_TOP, TAB_WIDTH, toolbar.height - TAB_TOP);
                bool isActive = index == modeIndex;

                if (isActive)
                    active = tab;

                DrawTab(tab, services[index].ShortTitle, isActive);
                HandleTabClick(tab, index, isActive);

                x += TAB_WIDTH + TAB_SPACING;
            }

            if (Event.current.type != EventType.Repaint)
                return;

            float borderY = toolbar.yMax - 1f;

            EditorGUI.DrawRect(new Rect(toolbar.x, borderY, active.x - toolbar.x, 1f), skin.Border);
            EditorGUI.DrawRect(new Rect(active.xMax, borderY, toolbar.xMax - active.xMax, 1f), skin.Border);
            EditorGUI.DrawRect(new Rect(active.x + 1f, borderY, active.width - 2f, 1f), skin.PanelFill);
        }

        private void DrawTab(Rect tab, string title, bool isActive)
        {
            if (Event.current.type != EventType.Repaint)
                return;

            var style = isActive
                ? skin.TabActive
                : tab.Contains(Event.current.mousePosition)
                    ? skin.TabHover
                    : skin.Tab;

            style.Draw(tab, title, false, false, false, false);
        }

        private void HandleTabClick(Rect tab, int index, bool isActive)
        {
            if (isActive || Event.current.type != EventType.MouseDown || Event.current.button != 0)
                return;

            if (!tab.Contains(Event.current.mousePosition))
                return;

            GUI.FocusControl(null);
            pendingAction = () => SwitchMode(index);
            Event.current.Use();
        }

        private void DrawContainerList()
        {
            var files = Service.Filter(searchQuery);

            using (new EditorGUILayout.VerticalScope(skin.Panel, GUILayout.Width(SIDEBAR_WIDTH),
                       GUILayout.ExpandHeight(true)))
            {
                DrawPanelHeader("Containers", null, DrawListActions);

                fileScroll = EditorGUILayout.BeginScrollView(fileScroll, GUIStyle.none, GUI.skin.verticalScrollbar);

                using (new EditorGUILayout.VerticalScope(skin.ScrollContent))
                {
                    if (files.Length == 0)
                    {
                        DataEditorGUI.DrawEmptyState(skin, Service.FileCount == 0
                            ? "No containers yet"
                            : "Nothing matches the search");
                    }

                    foreach (var file in files)
                        DrawContainerRow(file);
                }

                EditorGUILayout.EndScrollView();
            }
        }

        private void DrawContainerRow(string file)
        {
            bool isSelected = file == Service.SelectedPath;
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
                    ContainerNaming.GetDisplayName(file),
                    isSelected ? skin.RowTitleActive : skin.RowTitle);

                GUI.Label(new Rect(textX, rect.y + 19f, textWidth, 13f),
                    Service.GetFileMeta(file),
                    skin.RowSubtitle);

                if (isSelected && Service.HasPendingChanges)
                {
                    GUI.DrawTexture(
                        new Rect(rect.xMax - 15f, rect.y + rect.height * 0.5f - 3.5f, 7f, 7f),
                        skin.Dot);
                }
            }

            if (Event.current.type != EventType.MouseDown || Event.current.button != 0 || !isHovered)
                return;

            GUI.FocusControl(null);
            pendingAction = () => SelectContainer(file);
            Event.current.Use();
        }

        private void DrawInspectorPanel()
        {
            using (new EditorGUILayout.VerticalScope(skin.Panel, GUILayout.ExpandHeight(true)))
            {
                DrawPanelHeader(GetInspectorTitle(), Service.GetFileMeta(Service.SelectedPath), DrawContainerActions);

                inspectorScroll = EditorGUILayout.BeginScrollView(
                    inspectorScroll, GUIStyle.none, GUI.skin.verticalScrollbar);

                using (new EditorGUILayout.VerticalScope(skin.ScrollContent))
                {
                    if (!string.IsNullOrEmpty(Service.LoadError))
                        EditorGUILayout.HelpBox(Service.LoadError, MessageType.Error);
                    else if (Service.SelectedContainer == null)
                        DataEditorGUI.DrawEmptyState(skin, "Select a container");
                    else
                        DrawSelectedContainer();
                }

                EditorGUILayout.EndScrollView();
            }
        }

        private void DrawSelectedContainer()
        {
            float labelWidth = Mathf.Clamp(position.width * 0.24f, 140f, 260f);
            var service = Service;

            if (service.SerializedContainer != null)
            {
                if (propertyInspector.Draw(service.SerializedContainer, skin, labelWidth))
                    service.HasPendingChanges = true;

                if (propertyInspector.TryTakePendingAction(out var action))
                    pendingAction = () => RunRecordAction(service, action);

                return;
            }

            if (inspector.Draw(service.SelectedContainer, service.SelectedPath, skin, labelWidth))
                service.HasPendingChanges = true;
        }

        private void DrawListActions()
        {
            if (GUILayout.Button(skin.RefreshIcon, skin.IconButton))
                pendingAction = Service.Refresh;

            if (GUILayout.Button(skin.FolderIcon, skin.IconButton))
            {
                string folder = Service.FolderPath;
                pendingAction = () => EditorUtility.RevealInFinder(folder);
            }

            using (new EditorGUI.DisabledScope(Service.FileCount == 0))
            {
                if (GUILayout.Button(skin.DeleteAllIcon, skin.DangerIconButton))
                    pendingAction = DeleteAllContainers;
            }
        }

        private void DrawContainerActions()
        {
            using (new EditorGUI.DisabledScope(Service.SelectedContainerType == null))
            {
                if (GUILayout.Button(skin.ImportIcon, skin.IconButton))
                    pendingAction = ImportFromClipboard;
            }

            using (new EditorGUI.DisabledScope(Service.SelectedContainer == null))
            {
                if (GUILayout.Button(skin.ExportIcon, skin.IconButton))
                    pendingAction = ExportToClipboard;
            }

            using (new EditorGUI.DisabledScope(!Service.HasPendingChanges))
            {
                if (GUILayout.Button(skin.RevertIcon, skin.IconButton))
                    pendingAction = RevertContainer;
            }

            using (new EditorGUI.DisabledScope(!Service.HasSelection))
            {
                var saveStyle = Service.HasPendingChanges ? skin.PrimaryIconButton : skin.IconButton;

                if (GUILayout.Button(skin.SaveIcon, saveStyle))
                    pendingAction = Service.Save;

                if (GUILayout.Button(skin.DeleteIcon, skin.DangerIconButton))
                    pendingAction = DeleteSelectedContainer;
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

            DataEditorGUI.DrawSeparator(skin);
        }

        private void DrawFooter()
        {
            using (new EditorGUILayout.HorizontalScope(skin.Footer))
            {
                if (Event.current.type == EventType.Repaint)
                {
                    var background = new Rect(0f, position.height - DataEditorSkin.FOOTER_HEIGHT,
                        position.width, DataEditorSkin.FOOTER_HEIGHT);

                    EditorGUI.DrawRect(background, skin.ToolbarBackground);
                    EditorGUI.DrawRect(new Rect(background.x, background.y, background.width, 1f), skin.Border);
                }

                GUILayout.Label(Service.FolderPath, skin.FooterLabel);
                GUILayout.FlexibleSpace();
                GUILayout.Label(
                    Service.HasPendingChanges ? "Unsaved changes" : $"{Service.FileCount} file(s)",
                    Service.HasPendingChanges ? skin.FooterAccentLabel : skin.FooterLabel);
            }
        }

        private string GetInspectorTitle()
        {
            if (Service.SelectedContainerType != null)
                return ContainerNaming.FormatTypeName(Service.SelectedContainerType.Name);

            return Service.HasSelection
                ? ContainerNaming.GetDisplayName(Service.SelectedPath)
                : "Inspector";
        }

        private void SwitchMode(int index)
        {
            Service.DiscardPendingChanges();

            modeIndex = index;
            inspector.ResetFoldouts();
            propertyInspector.ResetFoldouts();
            inspectorScroll = Vector2.zero;
            fileScroll = Vector2.zero;

            Service.Refresh();
        }

        private void SelectContainer(string path)
        {
            Service.Select(path);
            inspector.ResetFoldouts();
            propertyInspector.ResetFoldouts();
            inspectorScroll = Vector2.zero;
        }

        private static void RunRecordAction(DataContainerService service, Action action)
        {
            action.Invoke();
            service.HasPendingChanges = true;
        }

        private void RevertContainer()
        {
            string fileName = ContainerNaming.GetDisplayName(Service.SelectedPath);

            bool confirmed = EditorUtility.DisplayDialog(
                "Revert Changes",
                $"Discard unsaved changes of \"{fileName}\"?",
                "Revert",
                "Cancel");

            if (!confirmed)
                return;

            Service.Revert();

            inspector.ResetFoldouts();
            propertyInspector.ResetFoldouts();
        }

        private void DeleteSelectedContainer()
        {
            string fileName = ContainerNaming.GetDisplayName(Service.SelectedPath);

            bool confirmed = EditorUtility.DisplayDialog(
                "Delete Data File",
                $"Delete the data file of \"{fileName}\"?",
                "Delete",
                "Cancel");

            if (!confirmed)
                return;

            Service.DeleteSelected();
            inspector.ResetFoldouts();
            propertyInspector.ResetFoldouts();
        }

        private void DeleteAllContainers()
        {
            bool confirmed = EditorUtility.DisplayDialog(
                "Delete All Data Files",
                $"Delete all data files of the {Service.Title.ToLowerInvariant()}?",
                "Delete All",
                "Cancel");

            if (!confirmed)
                return;

            Service.DeleteAll();
            inspector.ResetFoldouts();
            propertyInspector.ResetFoldouts();
        }

        private void ImportFromClipboard()
        {
            string json = EditorGUIUtility.systemCopyBuffer;

            if (string.IsNullOrWhiteSpace(json))
            {
                ShowNotification(new GUIContent("Clipboard is empty"));
                return;
            }

            if (!Service.TryImport(json, out string error))
            {
                EditorUtility.DisplayDialog("Import failed", error, "OK");
                return;
            }

            inspector.ResetFoldouts();
            propertyInspector.ResetFoldouts();
            ShowNotification(new GUIContent("JSON imported"));
        }

        private void ExportToClipboard()
        {
            string json = Service.Export();

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
                skin = new DataEditorSkin();
        }
    }
}
