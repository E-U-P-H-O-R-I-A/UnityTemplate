using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using Data;
using Data.Scheme;
using UnityEditor;
using UnityEngine;

namespace Editor
{
    public class PrivateDataEditor : EditorWindow
    {
        private const string FOLDER_NAME = "PrivateData";
        private const string JSON_EXTENSION = "*.json";
        private const string MODEL_NAMESPACE_PREFIX = "Data.";
        private const string MODEL_NAME_SUFFIX = "PrivateModel";

        private const float SIDEBAR_WIDTH = 250f;
        private const float ROW_HEIGHT = 36f;
        private const float TOOLBAR_HEIGHT = 32f;
        private const float HEADER_HEIGHT = 34f;
        private const float SPACING = 8f;

        private string[] saveFiles = Array.Empty<string>();
        private string selectedPath;
        private Type selectedModelType;
        private IPrivateModel selectedModel;
        private string loadError;
        private string searchQuery = string.Empty;
        private Vector2 fileScroll;
        private Vector2 inspectorScroll;
        private bool hasPendingChanges;
        private int rowIndex;
        private Action pendingAction;
        private Skin skin;
        private readonly Dictionary<string, bool> foldouts = new();
        private readonly Dictionary<string, string> fileMeta = new();

        private static string SaveFolderPath =>
            Path.Combine(Application.persistentDataPath, FOLDER_NAME);

        [MenuItem("Tools/Data Utility")]
        public static void Open()
        {
            foreach (var opened in Resources.FindObjectsOfTypeAll<PrivateDataEditor>())
                opened.Close();

            var window = CreateInstance<PrivateDataEditor>();
            window.titleContent = new GUIContent("Data Utility", GetFileIcon());
            window.minSize = new Vector2(780f, 520f);
            window.RefreshFiles();
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
            if (!hasPendingChanges)
                RefreshFiles();
        }

        private void OnGUI()
        {
            EnsureSkin();

            if (Event.current.type == EventType.Repaint)
                EditorGUI.DrawRect(new Rect(0f, 0f, position.width, position.height), skin.WindowBackground);

            DrawToolbar();

            using (new EditorGUILayout.HorizontalScope(skin.Body, GUILayout.ExpandHeight(true)))
            {
                DrawFileList();
                GUILayout.Space(SPACING);
                DrawInspectorPanel();
            }

            DrawFooter();

            if (Event.current.type == EventType.MouseMove)
                Repaint();

            FlushPendingAction();
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

        private void DrawToolbar()
        {
            using (new EditorGUILayout.HorizontalScope(skin.Toolbar))
            {
                if (Event.current.type == EventType.Repaint)
                {
                    EditorGUI.DrawRect(new Rect(0f, 0f, position.width, TOOLBAR_HEIGHT), skin.ToolbarBackground);
                    EditorGUI.DrawRect(new Rect(0f, TOOLBAR_HEIGHT - 1f, position.width, 1f), skin.Border);
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

        private void DrawFileList()
        {
            var files = GetFilteredFiles();

            using (new EditorGUILayout.VerticalScope(skin.Panel, GUILayout.Width(SIDEBAR_WIDTH),
                       GUILayout.ExpandHeight(true)))
            {
                DrawPanelHeader("Models", null, DrawFileActions);

                fileScroll = EditorGUILayout.BeginScrollView(fileScroll, GUIStyle.none, GUI.skin.verticalScrollbar);

                using (new EditorGUILayout.VerticalScope(skin.ScrollContent))
                {
                    if (files.Length == 0)
                    {
                        DrawEmptyState(saveFiles.Length == 0
                            ? "No save files yet"
                            : "Nothing matches the search");
                    }

                    foreach (var file in files)
                        DrawFileRow(file);
                }

                EditorGUILayout.EndScrollView();
            }
        }

        private void DrawFileActions()
        {
            if (GUILayout.Button(skin.RefreshIcon, skin.IconButton))
                pendingAction = RefreshFiles;

            if (GUILayout.Button(skin.FolderIcon, skin.IconButton))
                pendingAction = () => EditorUtility.RevealInFinder(SaveFolderPath);

            using (new EditorGUI.DisabledScope(saveFiles.Length == 0))
            {
                if (GUILayout.Button(skin.DeleteAllIcon, skin.DangerIconButton))
                    pendingAction = DeleteAllFiles;
            }
        }

        private void DrawModelActions()
        {
            using (new EditorGUI.DisabledScope(selectedModelType == null))
            {
                if (GUILayout.Button(skin.ImportIcon, skin.IconButton))
                    pendingAction = ImportFromClipboard;
            }

            using (new EditorGUI.DisabledScope(selectedModel == null))
            {
                if (GUILayout.Button(skin.ExportIcon, skin.IconButton))
                    pendingAction = ExportToClipboard;
            }

            using (new EditorGUI.DisabledScope(string.IsNullOrEmpty(selectedPath)))
            {
                var saveStyle = hasPendingChanges ? skin.PrimaryIconButton : skin.IconButton;

                if (GUILayout.Button(skin.SaveIcon, saveStyle))
                    pendingAction = SaveSelectedFile;

                if (GUILayout.Button(skin.DeleteIcon, skin.DangerIconButton))
                    pendingAction = DeleteSelectedFile;
            }
        }

        private void ImportFromClipboard()
        {
            if (selectedModelType == null)
                return;

            string json = EditorGUIUtility.systemCopyBuffer;

            if (string.IsNullOrWhiteSpace(json))
            {
                ShowNotification(new GUIContent("Clipboard is empty"));
                return;
            }

            try
            {
                var dump = JsonUtility.FromJson<SchemesDump>(json);
                if (dump?.items == null || dump.items.Count == 0)
                    throw new Exception("Clipboard JSON contains no schemes.");

                var model = (IPrivateModel)Activator.CreateInstance(selectedModelType);
                model.ImportFromJson(json);

                selectedModel = model;
                loadError = null;
                hasPendingChanges = true;
                foldouts.Clear();

                ShowNotification(new GUIContent("JSON imported"));
            }
            catch (Exception e)
            {
                EditorUtility.DisplayDialog("Import failed", e.Message, "OK");
            }
        }

        private void ExportToClipboard()
        {
            if (selectedModel == null)
                return;

            EditorGUIUtility.systemCopyBuffer = selectedModel.ExportToJson();
            ShowNotification(new GUIContent("JSON copied"));
        }

        private void DrawFileRow(string file)
        {
            bool isSelected = file == selectedPath;
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
                    GetDisplayFileName(file),
                    isSelected ? skin.RowTitleActive : skin.RowTitle);

                GUI.Label(new Rect(textX, rect.y + 19f, textWidth, 13f),
                    GetFileMeta(file),
                    skin.RowSubtitle);

                if (isSelected && hasPendingChanges)
                {
                    GUI.DrawTexture(
                        new Rect(rect.xMax - 15f, rect.y + rect.height * 0.5f - 3.5f, 7f, 7f),
                        skin.Dot);
                }
            }

            if (Event.current.type != EventType.MouseDown || Event.current.button != 0 || !isHovered)
                return;

            GUI.FocusControl(null);
            pendingAction = () => SelectFile(file);
            Event.current.Use();
        }

        private void DrawInspectorPanel()
        {
            using (new EditorGUILayout.VerticalScope(skin.Panel, GUILayout.ExpandHeight(true)))
            {
                DrawPanelHeader(GetInspectorTitle(), GetFileMeta(selectedPath), DrawModelActions);

                inspectorScroll = EditorGUILayout.BeginScrollView(
                    inspectorScroll, GUIStyle.none, GUI.skin.verticalScrollbar);

                using (new EditorGUILayout.VerticalScope(skin.ScrollContent))
                {
                    if (!string.IsNullOrEmpty(loadError))
                        EditorGUILayout.HelpBox(loadError, MessageType.Error);
                    else if (selectedModel == null)
                        DrawEmptyState("Select a save file");
                    else
                        DrawModelInspector();
                }

                EditorGUILayout.EndScrollView();
            }
        }

        private void DrawFooter()
        {
            using (new EditorGUILayout.HorizontalScope(skin.Footer))
            {
                if (Event.current.type == EventType.Repaint)
                {
                    var background = new Rect(0f, position.height - skin.Footer.fixedHeight,
                        position.width, skin.Footer.fixedHeight);

                    EditorGUI.DrawRect(background, skin.ToolbarBackground);
                    EditorGUI.DrawRect(new Rect(background.x, background.y, background.width, 1f), skin.Border);
                }

                GUILayout.Label(SaveFolderPath, skin.FooterLabel);
                GUILayout.FlexibleSpace();
                GUILayout.Label(
                    hasPendingChanges ? "Unsaved changes" : $"{saveFiles.Length} file(s)",
                    hasPendingChanges ? skin.FooterAccentLabel : skin.FooterLabel);
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

            DrawSeparator();
        }

        private void DrawSeparator()
        {
            var rect = GUILayoutUtility.GetRect(1f, 1f, GUILayout.ExpandWidth(true));

            if (Event.current.type == EventType.Repaint)
                EditorGUI.DrawRect(rect, skin.Separator);
        }

        private void DrawEmptyState(string message)
        {
            GUILayout.Space(16f);
            GUILayout.Label(message, skin.EmptyLabel);
        }

        private string[] GetFilteredFiles()
        {
            if (string.IsNullOrWhiteSpace(searchQuery))
                return saveFiles;

            string query = searchQuery.Trim();

            return saveFiles
                .Where(file =>
                    Contains(GetDisplayFileName(file), query) ||
                    Contains(Path.GetFileNameWithoutExtension(file), query))
                .ToArray();
        }

        private static bool Contains(string value, string query) =>
            value.IndexOf(query, StringComparison.OrdinalIgnoreCase) >= 0;

        private string GetInspectorTitle()
        {
            if (selectedModelType != null)
                return FormatModelName(selectedModelType.Name);

            return string.IsNullOrEmpty(selectedPath)
                ? "Inspector"
                : GetDisplayFileName(selectedPath);
        }

        private string GetFileMeta(string path)
        {
            if (string.IsNullOrEmpty(path))
                return string.Empty;

            return fileMeta.TryGetValue(path, out string meta) ? meta : string.Empty;
        }

        private void RefreshFiles()
        {
            Directory.CreateDirectory(SaveFolderPath);

            saveFiles = Directory
                .GetFiles(SaveFolderPath, JSON_EXTENSION, SearchOption.TopDirectoryOnly)
                .OrderBy(Path.GetFileName)
                .ToArray();

            CacheFileMeta();

            if (!string.IsNullOrEmpty(selectedPath) && saveFiles.Contains(selectedPath))
            {
                SelectFile(selectedPath);
                return;
            }

            if (saveFiles.Length > 0)
            {
                SelectFile(saveFiles[0]);
                return;
            }

            selectedPath = null;
            selectedModelType = null;
            selectedModel = null;
            loadError = null;
            hasPendingChanges = false;
        }

        private void CacheFileMeta()
        {
            fileMeta.Clear();

            foreach (var file in saveFiles)
            {
                var info = new FileInfo(file);
                fileMeta[file] = info.Exists ? FormatSize(info.Length) : "missing";
            }
        }

        private void SelectFile(string path)
        {
            selectedPath = path;
            selectedModelType = GetModelType(path);
            selectedModel = null;
            loadError = null;
            hasPendingChanges = false;
            inspectorScroll = Vector2.zero;
            foldouts.Clear();

            if (selectedModelType == null)
            {
                loadError = $"Could not find private model type for file: {Path.GetFileName(path)}";
                return;
            }

            try
            {
                selectedModel = (IPrivateModel)Activator.CreateInstance(selectedModelType);

                string fileJson = File.Exists(path) ? File.ReadAllText(path) : string.Empty;
                selectedModel.ImportFromJson(fileJson);
            }
            catch (Exception e)
            {
                selectedModel = null;
                loadError = e.Message;
            }
        }

        private void SaveSelectedFile()
        {
            if (string.IsNullOrEmpty(selectedPath))
                return;

            if (selectedModel == null)
                return;

            Directory.CreateDirectory(SaveFolderPath);
            File.WriteAllText(selectedPath, selectedModel.ExportToJson());
            hasPendingChanges = false;
            CacheFileMeta();
        }

        private void DeleteSelectedFile()
        {
            if (string.IsNullOrEmpty(selectedPath) || !File.Exists(selectedPath))
                return;

            string fileName = Path.GetFileName(selectedPath);
            bool confirmed = EditorUtility.DisplayDialog(
                "Delete Save",
                $"Delete save file \"{fileName}\"?",
                "Delete",
                "Cancel");

            if (!confirmed)
                return;

            File.Delete(selectedPath);
            selectedPath = null;
            RefreshFiles();
        }

        private void DeleteAllFiles()
        {
            bool confirmed = EditorUtility.DisplayDialog(
                "Delete All Saves",
                "Delete all private save files?",
                "Delete All",
                "Cancel");

            if (!confirmed)
                return;

            foreach (var file in saveFiles)
            {
                if (File.Exists(file))
                    File.Delete(file);
            }

            selectedPath = null;
            RefreshFiles();
        }

        private void DrawModelInspector()
        {
            var schemes = GetSchemes(selectedModel).ToList();
            if (schemes.Count == 0)
            {
                DrawEmptyState("This save has no serialized schemes");
                return;
            }

            float previousLabelWidth = EditorGUIUtility.labelWidth;
            EditorGUIUtility.labelWidth = Mathf.Clamp(position.width * 0.24f, 140f, 260f);
            rowIndex = 0;

            for (int i = 0; i < schemes.Count; i++)
            {
                var scheme = schemes[i];
                string key = $"{selectedPath}:{i}:{scheme.ID}";

                using (new EditorGUILayout.VerticalScope(skin.Section))
                {
                    if (!DrawSectionHeader(scheme.ID, key))
                        continue;

                    GUILayout.Space(2f);
                    DrawSerializableFields(scheme, key);
                }
            }

            EditorGUIUtility.labelWidth = previousLabelWidth;
        }

        private bool DrawSectionHeader(string label, string key)
        {
            foldouts.TryGetValue(key, out bool isExpanded);

            var rect = GUILayoutUtility.GetRect(0f, 20f, GUILayout.ExpandWidth(true));

            if (Event.current.type == EventType.Repaint && rect.Contains(Event.current.mousePosition))
                EditorGUI.DrawRect(rect, skin.RowHover);

            isExpanded = EditorGUI.Foldout(
                new Rect(rect.x + 2f, rect.y + 2f, rect.width - 4f, 16f),
                isExpanded,
                label,
                true,
                skin.SectionFoldout);

            foldouts[key] = isExpanded;
            return isExpanded;
        }

        private void DrawSerializableFields(object target, string path)
        {
            foreach (var field in GetSerializableFields(target.GetType()))
            {
                DrawField(target, field, $"{path}.{field.Name}");
            }
        }

        private void DrawField(object target, FieldInfo field, string path)
        {
            object currentValue = field.GetValue(target);
            string label = ObjectNames.NicifyVariableName(field.Name);

            EditorGUI.BeginChangeCheck();

            object nextValue;
            if (IsInlineType(field.FieldType))
            {
                using (new EditorGUILayout.VerticalScope(NextRowStyle()))
                    nextValue = DrawValue(label, field.FieldType, currentValue, path);
            }
            else
            {
                nextValue = DrawValue(label, field.FieldType, currentValue, path);
            }

            if (!EditorGUI.EndChangeCheck())
                return;

            field.SetValue(target, nextValue);
            hasPendingChanges = true;
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

            if (typeof(IList).IsAssignableFrom(type))
            {
                DrawList(label, value as IList, path);
                return value;
            }

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
            {
                EditorGUILayout.LabelField(" ", "Empty", skin.InlineHint);
            }

            for (int i = 0; i < list.Count; i++)
            {
                object element = list[i];
                string elementPath = $"{path}[{i}]";
                Type elementType = element?.GetType();

                if (elementType == null)
                {
                    EditorGUILayout.LabelField($"Element {i}", "null");
                    continue;
                }

                EditorGUI.BeginChangeCheck();
                object nextValue = DrawValue($"Element {i}", elementType, element, elementPath);

                if (EditorGUI.EndChangeCheck())
                {
                    list[i] = nextValue;
                    hasPendingChanges = true;
                }
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

        private static IEnumerable<PrivateScheme> GetSchemes(IPrivateModel model)
        {
            var collectionField = FindField(model.GetType(), "schemes");
            if (collectionField?.GetValue(model) is IEnumerable collection)
            {
                foreach (var item in collection)
                {
                    if (item is PrivateScheme scheme)
                        yield return scheme;
                }

                yield break;
            }

            var singleField = FindField(model.GetType(), "scheme");
            if (singleField == null)
                yield break;

            var singleScheme = singleField.GetValue(model) as PrivateScheme;
            if (singleScheme == null)
            {
                var getScheme = model.GetType().GetMethod(
                    nameof(PrivateModel.Single<PrivateScheme>.GetScheme),
                    Type.EmptyTypes);

                singleScheme = getScheme?.Invoke(model, null) as PrivateScheme;
            }

            if (singleScheme != null)
                yield return singleScheme;
        }

        private static IEnumerable<FieldInfo> GetSerializableFields(Type type)
        {
            return type
                .GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)
                .Where(field =>
                    !field.IsStatic &&
                    !field.IsInitOnly &&
                    !field.IsNotSerialized &&
                    (field.IsPublic || field.GetCustomAttribute<SerializeField>() != null));
        }

        private static FieldInfo FindField(Type type, string fieldName)
        {
            while (type != null)
            {
                var field = type.GetField(fieldName, BindingFlags.Instance | BindingFlags.NonPublic);
                if (field != null)
                    return field;

                type = type.BaseType;
            }

            return null;
        }

        private static Type GetModelType(string path)
        {
            string fileName = Path.GetFileNameWithoutExtension(path);

            return typeof(IPrivateModel).Assembly
                .GetTypes()
                .FirstOrDefault(type =>
                    type.IsClass &&
                    !type.IsAbstract &&
                    typeof(IPrivateModel).IsAssignableFrom(type) &&
                    SanitizeFileName(type.FullName ?? type.Name) == fileName);
        }

        private static string GetDisplayFileName(string path)
        {
            string fileName = Path.GetFileNameWithoutExtension(path);

            return FormatModelName(fileName.StartsWith(MODEL_NAMESPACE_PREFIX, StringComparison.Ordinal)
                ? fileName.Substring(MODEL_NAMESPACE_PREFIX.Length)
                : fileName);
        }

        private static string FormatModelName(string name)
        {
            if (string.IsNullOrEmpty(name))
                return name;

            if (name.Length > MODEL_NAME_SUFFIX.Length && name.EndsWith(MODEL_NAME_SUFFIX, StringComparison.Ordinal))
                name = name.Substring(0, name.Length - MODEL_NAME_SUFFIX.Length);

            var builder = new StringBuilder(name.Length + 4);
            builder.Append(name[0]);

            for (int i = 1; i < name.Length; i++)
            {
                char symbol = name[i];

                if (IsWordStart(name, i))
                    builder.Append(' ');

                builder.Append(char.IsUpper(symbol) ? char.ToLowerInvariant(symbol) : symbol);
            }

            return builder.ToString();
        }

        private static bool IsWordStart(string name, int index)
        {
            if (!char.IsUpper(name[index]))
                return false;

            if (!char.IsUpper(name[index - 1]))
                return true;

            return index + 1 < name.Length && char.IsLower(name[index + 1]);
        }

        private static string SanitizeFileName(string name)
        {
            foreach (var c in Path.GetInvalidFileNameChars())
                name = name.Replace(c, '_');

            return name.Replace(' ', '_');
        }

        private static string FormatSize(long bytes)
        {
            if (bytes < 1024L)
                return $"{bytes} B";

            if (bytes < 1024L * 1024L)
                return $"{bytes / 1024f:0.#} KB";

            return $"{bytes / (1024f * 1024f):0.#} MB";
        }

        private static Texture GetFileIcon() =>
            EditorGUIUtility.ObjectContent(null, typeof(TextAsset)).image;

        private void EnsureSkin()
        {
            if (skin == null || !skin.IsValid)
                skin = new Skin();
        }

        private sealed class Skin
        {
            private readonly List<Texture2D> textures = new();

            public readonly Color WindowBackground;
            public readonly Color ToolbarBackground;
            public readonly Color Border;
            public readonly Color Separator;
            public readonly Color Accent;
            public readonly Color RowHover;
            public readonly Color RowSelected;

            public readonly GUIStyle Body;
            public readonly GUIStyle Toolbar;
            public readonly GUIStyle ToolbarTitle;
            public readonly GUIStyle Panel;
            public readonly GUIStyle PanelHeader;
            public readonly GUIStyle PanelTitle;
            public readonly GUIStyle ScrollContent;
            public readonly GUIStyle MetaLabel;
            public readonly GUIStyle RowTitle;
            public readonly GUIStyle RowTitleActive;
            public readonly GUIStyle RowSubtitle;
            public readonly GUIStyle RowEven;
            public readonly GUIStyle RowOdd;
            public readonly GUIStyle RowSelectedCard;
            public readonly GUIStyle RowHoverCard;
            public readonly Texture2D Dot;
            public readonly GUIStyle Section;
            public readonly GUIStyle SectionFoldout;
            public readonly GUIStyle NestedFoldout;
            public readonly GUIStyle Button;
            public readonly GUIStyle PrimaryButton;
            public readonly GUIStyle DangerButton;
            public readonly GUIStyle IconButton;
            public readonly GUIStyle PrimaryIconButton;
            public readonly GUIStyle DangerIconButton;
            public readonly GUIContent ImportIcon;
            public readonly GUIContent ExportIcon;
            public readonly GUIContent SaveIcon;
            public readonly GUIContent DeleteIcon;
            public readonly GUIContent DeleteAllIcon;
            public readonly GUIContent RefreshIcon;
            public readonly GUIContent FolderIcon;
            public readonly GUIStyle Footer;
            public readonly GUIStyle FooterLabel;
            public readonly GUIStyle FooterAccentLabel;
            public readonly GUIStyle EmptyLabel;
            public readonly GUIStyle InlineHint;

            public Skin()
            {
                bool dark = EditorGUIUtility.isProSkin;

                WindowBackground = dark ? Rgb(0x1E, 0x1F, 0x22) : Rgb(0xC6, 0xC6, 0xC8);
                ToolbarBackground = dark ? Rgb(0x26, 0x27, 0x2B) : Rgb(0xD6, 0xD6, 0xD8);
                Border = dark ? Rgb(0x14, 0x15, 0x17) : Rgb(0xA4, 0xA4, 0xA6);
                Separator = dark ? Rgb(0x35, 0x37, 0x3C) : Rgb(0xBD, 0xBD, 0xBF);
                Accent = dark ? Rgb(0x4C, 0x8D, 0xFF) : Rgb(0x2B, 0x66, 0xE0);
                RowHover = new Color(1f, 1f, 1f, dark ? 0.05f : 0.28f);
                RowSelected = new Color(Accent.r, Accent.g, Accent.b, dark ? 0.20f : 0.24f);

                var panelFill = dark ? Rgb(0x2B, 0x2D, 0x33) : Rgb(0xE0, 0xE0, 0xE2);
                var sectionFill = dark ? Rgb(0x32, 0x35, 0x3B) : Rgb(0xD5, 0xD5, 0xD8);
                var buttonFill = dark ? Rgb(0x3A, 0x3D, 0x44) : Rgb(0xEC, 0xEC, 0xEE);
                var buttonHover = dark ? Rgb(0x45, 0x49, 0x51) : Rgb(0xF6, 0xF6, 0xF8);
                var buttonActive = dark ? Rgb(0x2F, 0x32, 0x38) : Rgb(0xD2, 0xD2, 0xD4);
                var danger = dark ? Rgb(0x4A, 0x33, 0x36) : Rgb(0xE8, 0xD5, 0xD5);
                var dangerHover = dark ? Rgb(0x5C, 0x3B, 0x3F) : Rgb(0xF2, 0xDC, 0xDC);
                var dangerText = dark ? Rgb(0xE8, 0x8B, 0x8B) : Rgb(0x8E, 0x2B, 0x2B);

                var text = dark ? Rgb(0xD2, 0xD3, 0xD6) : Rgb(0x22, 0x22, 0x24);
                var textStrong = dark ? Rgb(0xF0, 0xF0, 0xF2) : Rgb(0x14, 0x14, 0x16);
                var textDim = dark ? Rgb(0x86, 0x89, 0x90) : Rgb(0x60, 0x60, 0x64);

                Body = new GUIStyle { padding = new RectOffset(8, 8, 8, 6) };

                Toolbar = new GUIStyle
                {
                    fixedHeight = TOOLBAR_HEIGHT,
                    padding = new RectOffset(10, 8, 5, 5)
                };

                ToolbarTitle = Label(textStrong, 13, FontStyle.Bold);
                ToolbarTitle.alignment = TextAnchor.MiddleLeft;

                Panel = Card(panelFill, Border, 6, new RectOffset(0, 0, 2, 2));
                PanelHeader = new GUIStyle { padding = new RectOffset(10, 8, 6, 6) };
                PanelTitle = Label(textStrong, 12, FontStyle.Bold);
                PanelTitle.alignment = TextAnchor.MiddleLeft;
                ScrollContent = new GUIStyle { padding = new RectOffset(4, 4, 4, 6) };

                MetaLabel = Label(textDim, 11, FontStyle.Normal);
                MetaLabel.alignment = TextAnchor.MiddleLeft;
                MetaLabel.padding = new RectOffset(0, 0, 0, 0);

                RowTitle = Label(text, 12, FontStyle.Normal);
                RowTitleActive = Label(textStrong, 12, FontStyle.Bold);
                RowSubtitle = Label(textDim, 10, FontStyle.Normal);

                RowEven = new GUIStyle { padding = new RectOffset(2, 2, 1, 1) };
                RowOdd = new GUIStyle
                {
                    padding = new RectOffset(2, 2, 1, 1),
                    normal = { background = Solid(new Color(1f, 1f, 1f, dark ? 0.028f : 0.16f)) }
                };

                RowSelectedCard = Card(RowSelected, new Color(Accent.r, Accent.g, Accent.b, dark ? 0.55f : 0.5f),
                    6, new RectOffset(0, 0, 0, 0));
                RowHoverCard = Card(RowHover, Color.clear, 6, new RectOffset(0, 0, 0, 0));
                Dot = Rounded(Accent, Color.clear, 12);

                Section = Card(sectionFill, Border, 6, new RectOffset(6, 6, 4, 6));
                Section.margin = new RectOffset(0, 0, 0, 6);

                SectionFoldout = new GUIStyle(EditorStyles.foldout) { fontStyle = FontStyle.Bold, fontSize = 12 };
                Tint(SectionFoldout, textStrong);

                NestedFoldout = new GUIStyle(EditorStyles.foldout) { fontSize = 12 };
                Tint(NestedFoldout, text);

                Button = ActionButton(buttonFill, buttonHover, buttonActive, text, Border);
                PrimaryButton = ActionButton(Accent,
                    Color.Lerp(Accent, Color.white, 0.15f),
                    Color.Lerp(Accent, Color.black, 0.15f),
                    Color.white,
                    Color.Lerp(Accent, Color.black, 0.25f));
                DangerButton = ActionButton(danger, dangerHover, danger, dangerText, Border);

                IconButton = IconVariant(Button);
                PrimaryIconButton = IconVariant(PrimaryButton);
                DangerIconButton = IconVariant(DangerButton);

                ImportIcon = new GUIContent(TrayArrowIcon(false, text), "Import JSON from clipboard");
                ExportIcon = new GUIContent(TrayArrowIcon(true, text), "Copy JSON to clipboard");
                SaveIcon = Icon("SaveAs", "S", "Save selected file");
                DeleteIcon = Icon("TreeEditor.Trash", "D", "Delete selected file");
                DeleteAllIcon = Icon("CrossIcon", "X", "Delete all save files");
                RefreshIcon = Icon("Refresh", "R", "Refresh file list");
                FolderIcon = Icon("FolderOpened Icon", "F", "Reveal folder in explorer");

                Footer = new GUIStyle
                {
                    fixedHeight = 22f,
                    padding = new RectOffset(10, 10, 4, 4)
                };

                FooterLabel = Label(textDim, 11, FontStyle.Normal);
                FooterAccentLabel = Label(Accent, 11, FontStyle.Bold);

                EmptyLabel = Label(textDim, 12, FontStyle.Normal);
                EmptyLabel.alignment = TextAnchor.MiddleCenter;

                InlineHint = Label(textDim, 11, FontStyle.Italic);
            }

            public bool IsValid => textures.Count == 0 || textures[0] != null;

            public void Dispose()
            {
                foreach (var texture in textures)
                {
                    if (texture != null)
                        UnityEngine.Object.DestroyImmediate(texture);
                }

                textures.Clear();
            }

            private static Color Rgb(int r, int g, int b) =>
                new Color(r / 255f, g / 255f, b / 255f);

            private static GUIStyle Label(Color color, int fontSize, FontStyle fontStyle)
            {
                var style = new GUIStyle(EditorStyles.label)
                {
                    fontSize = fontSize,
                    fontStyle = fontStyle,
                    wordWrap = false,
                    clipping = TextClipping.Clip
                };

                Tint(style, color);
                return style;
            }

            private static void Tint(GUIStyle style, Color color)
            {
                style.normal.textColor = color;
                style.onNormal.textColor = color;
                style.hover.textColor = color;
                style.onHover.textColor = color;
                style.active.textColor = color;
                style.onActive.textColor = color;
                style.focused.textColor = color;
                style.onFocused.textColor = color;
            }

            private GUIStyle Card(Color fill, Color border, int radius, RectOffset padding)
            {
                int slice = Mathf.Min(radius + 2, 11);

                return new GUIStyle
                {
                    normal = { background = Rounded(fill, border, radius) },
                    border = new RectOffset(slice, slice, slice, slice),
                    padding = padding,
                    margin = new RectOffset(0, 0, 0, 0)
                };
            }

            private GUIStyle ActionButton(Color fill, Color hover, Color active, Color text, Color border)
            {
                var style = Card(fill, border, 5, new RectOffset(8, 8, 0, 0));

                style.hover.background = Rounded(hover, border, 5);
                style.active.background = Rounded(active, border, 5);
                style.focused.background = style.normal.background;
                style.onNormal.background = style.normal.background;
                style.onHover.background = style.hover.background;
                style.onActive.background = style.active.background;

                Tint(style, text);

                style.alignment = TextAnchor.MiddleCenter;
                style.fontSize = 11;
                style.fixedHeight = 22f;
                style.margin = new RectOffset(2, 2, 0, 0);

                return style;
            }

            private static GUIStyle IconVariant(GUIStyle source)
            {
                return new GUIStyle(source)
                {
                    imagePosition = ImagePosition.ImageOnly,
                    padding = new RectOffset(7, 7, 5, 5),
                    margin = new RectOffset(2, 2, 0, 0),
                    fixedWidth = 30f,
                    fixedHeight = 26f
                };
            }

            private static GUIContent Icon(string iconName, string fallbackText, string tooltip)
            {
                var texture = LoadIcon(iconName);

                return texture != null
                    ? new GUIContent(texture, tooltip)
                    : new GUIContent(fallbackText, tooltip);
            }

            private static Texture LoadIcon(string iconName)
            {
                if (EditorGUIUtility.isProSkin)
                {
                    var darkIcon = EditorGUIUtility.FindTexture($"d_{iconName}");
                    if (darkIcon != null)
                        return darkIcon;
                }

                return EditorGUIUtility.FindTexture(iconName);
            }

            private Texture2D TrayArrowIcon(bool pointingUp, Color color)
            {
                const int size = 16;
                const int samples = 4;

                var texture = CreateTexture(size);
                var pixels = new Color[size * size];

                for (int y = 0; y < size; y++)
                {
                    for (int x = 0; x < size; x++)
                    {
                        int hits = 0;

                        for (int subY = 0; subY < samples; subY++)
                        {
                            for (int subX = 0; subX < samples; subX++)
                            {
                                float sampleX = x + (subX + 0.5f) / samples;
                                float sampleY = y + (subY + 0.5f) / samples;

                                if (IsInsideTrayArrow(sampleX, sampleY, pointingUp))
                                    hits++;
                            }
                        }

                        var pixel = color;
                        pixel.a = color.a * hits / (samples * samples);
                        pixels[y * size + x] = pixel;
                    }
                }

                texture.SetPixels(pixels);
                texture.Apply();

                return texture;
            }

            private static bool IsInsideTrayArrow(float x, float y, bool pointingUp)
            {
                if (IsInsideBox(x, y, 2f, 14f, 2f, 3.6f))
                    return true;

                if (IsInsideBox(x, y, 2f, 3.6f, 2f, 7f) || IsInsideBox(x, y, 12.4f, 14f, 2f, 7f))
                    return true;

                const float centerX = 8f;
                const float stemHalfWidth = 1.1f;
                const float headHalfWidth = 3.5f;

                return pointingUp
                    ? IsInsideBox(x, y, centerX - stemHalfWidth, centerX + stemHalfWidth, 5.5f, 10.5f) ||
                      IsInsideArrowHead(x, y, centerX, 14.5f, 10.5f, headHalfWidth)
                    : IsInsideBox(x, y, centerX - stemHalfWidth, centerX + stemHalfWidth, 9.5f, 14.5f) ||
                      IsInsideArrowHead(x, y, centerX, 5.5f, 9.5f, headHalfWidth);
            }

            private static bool IsInsideBox(float x, float y, float left, float right, float bottom, float top)
            {
                return x >= left && x <= right && y >= bottom && y <= top;
            }

            private static bool IsInsideArrowHead(float x, float y, float centerX, float apexY, float baseY,
                float halfWidth)
            {
                float progress = (apexY - y) / (apexY - baseY);

                if (progress < 0f || progress > 1f)
                    return false;

                return Mathf.Abs(x - centerX) <= halfWidth * progress;
            }

            private Texture2D Solid(Color color)
            {
                var texture = CreateTexture(1);
                texture.SetPixel(0, 0, color);
                texture.Apply();

                return texture;
            }

            private Texture2D Rounded(Color fill, Color border, int radius)
            {
                const int size = 24;
                const float outline = 1.25f;

                var texture = CreateTexture(size);
                var pixels = new Color[size * size];

                float half = size * 0.5f;
                float clampedRadius = Mathf.Clamp(radius, 1, (int)half);
                bool hasBorder = border.a > 0.001f;

                for (int y = 0; y < size; y++)
                {
                    for (int x = 0; x < size; x++)
                    {
                        float dx = Mathf.Max(Mathf.Abs(x + 0.5f - half) - (half - clampedRadius), 0f);
                        float dy = Mathf.Max(Mathf.Abs(y + 0.5f - half) - (half - clampedRadius), 0f);
                        float distance = Mathf.Sqrt(dx * dx + dy * dy) - clampedRadius;

                        float coverage = Mathf.Clamp01(0.5f - distance);
                        float borderWeight = hasBorder ? Mathf.Clamp01(distance + outline) : 0f;

                        var color = Color.Lerp(fill, border, borderWeight);
                        color.a = Mathf.Lerp(fill.a, hasBorder ? border.a : fill.a, borderWeight) * coverage;

                        pixels[y * size + x] = color;
                    }
                }

                texture.SetPixels(pixels);
                texture.Apply();

                return texture;
            }

            private Texture2D CreateTexture(int size)
            {
                var texture = new Texture2D(size, size, TextureFormat.RGBA32, false)
                {
                    hideFlags = HideFlags.HideAndDontSave,
                    filterMode = FilterMode.Bilinear,
                    wrapMode = TextureWrapMode.Clamp
                };

                textures.Add(texture);
                return texture;
            }
        }
    }
}
