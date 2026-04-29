using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using Data.Model;
using Data.Scheme;
using UnityEditor;
using UnityEngine;

namespace Editor
{
    public class PrivateDataEditor : EditorWindow
    {
        private const string FOLDER_NAME = "PrivateData";
        private const string JSON_EXTENSION = "*.json";
        private const string MODEL_NAMESPACE_PREFIX = "Data.Model.Private.";

        private string[] saveFiles = Array.Empty<string>();
        private string selectedPath;
        private Type selectedModelType;
        private IPrivateModel selectedModel;
        private string loadError;
        private Vector2 fileScroll;
        private Vector2 inspectorScroll;
        private bool hasPendingChanges;
        private readonly Dictionary<string, bool> foldouts = new();

        private static string SaveFolderPath => 
            Path.Combine(Application.persistentDataPath, FOLDER_NAME);

        [MenuItem("Tools/Data Utility")]
        public static void Open()
        {
            var window = GetWindow<PrivateDataEditor>("Save Utility");
            window.minSize = new Vector2(730f, 480f);
            window.maxSize = new Vector2(730f, 480f);
            window.RefreshFiles();
        }
        
        private void OnFocus()
        {
            if (!hasPendingChanges)
                RefreshFiles();
        }

        private void OnGUI()
        {
            using (new EditorGUILayout.HorizontalScope())
            {
                DrawFileList();
                DrawInspectorEditor();
            }
        }
        
        private void DrawFileList()
        {
            using (new EditorGUILayout.VerticalScope(GUILayout.Width(240f)))
            {
                EditorGUILayout.LabelField("Files", EditorStyles.boldLabel);

                fileScroll = EditorGUILayout.BeginScrollView(fileScroll, GUI.skin.box);

                if (saveFiles.Length == 0)
                {
                    EditorGUILayout.HelpBox("No save files found.", MessageType.Info);
                }

                foreach (var file in saveFiles)
                {
                    bool isSelected = file == selectedPath;
                    string label = GetDisplayFileName(file);

                    if (GUILayout.Toggle(isSelected, label, "Button") && !isSelected)
                        SelectFile(file);
                }

                EditorGUILayout.EndScrollView();
            }
        }

        private void DrawInspectorEditor()
        {
            using (new EditorGUILayout.VerticalScope(GUILayout.Width(480f)))
            {
                using (new EditorGUILayout.HorizontalScope())
                {
                    EditorGUILayout.LabelField(GetSelectedFileLabel(), EditorStyles.boldLabel);

                    using (new EditorGUI.DisabledScope(string.IsNullOrEmpty(selectedPath)))
                    {
                        if (GUILayout.Button("Save", GUILayout.Width(90f)))
                            SaveSelectedFile();

                        if (GUILayout.Button("Delete", GUILayout.Width(90f)))
                            DeleteSelectedFile();
                    }

                    using (new EditorGUI.DisabledScope(saveFiles.Length == 0))
                    {
                        if (GUILayout.Button("Delete All", GUILayout.Width(90f)))
                            DeleteAllFiles();
                    }
                }

                EditorGUILayout.Space(4f);

                inspectorScroll = EditorGUILayout.BeginScrollView(inspectorScroll, GUI.skin.box);

                if (!string.IsNullOrEmpty(loadError))
                {
                    EditorGUILayout.HelpBox(loadError, MessageType.Error);
                }
                else if (selectedModel == null)
                {
                    EditorGUILayout.HelpBox("Select a save file.", MessageType.Info);
                }
                else
                {
                    DrawModelInspector();
                }

                EditorGUILayout.EndScrollView();
            }
        }

        private string GetSelectedFileLabel()
        {
            if (string.IsNullOrEmpty(selectedPath))
                return "Select a save file";

            string label = GetDisplayFileName(selectedPath);
            return hasPendingChanges ? $"{label} *" : label;
        }

        private void RefreshFiles()
        {
            Directory.CreateDirectory(SaveFolderPath);

            saveFiles = Directory
                .GetFiles(SaveFolderPath, JSON_EXTENSION, SearchOption.TopDirectoryOnly)
                .OrderBy(Path.GetFileName)
                .ToArray();

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
            //RefreshFiles();
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
            EditorGUILayout.LabelField(selectedModelType.Name, EditorStyles.boldLabel);
            EditorGUILayout.Space(4f);

            var schemes = GetSchemes(selectedModel).ToList();
            if (schemes.Count == 0)
            {
                EditorGUILayout.HelpBox("This save has no serialized schemes.", MessageType.Info);
                return;
            }

            for (int i = 0; i < schemes.Count; i++)
            {
                var scheme = schemes[i];
                string key = $"{selectedPath}:{i}:{scheme.ID}";
                string label = $"{scheme.ID}";

                foldouts.TryGetValue(key, out bool isExpanded);
                isExpanded = EditorGUILayout.Foldout(isExpanded, label, true);
                foldouts[key] = isExpanded;

                if (!isExpanded)
                    continue;

                EditorGUI.indentLevel++;
                DrawSerializableFields(scheme, key);
                EditorGUI.indentLevel--;
                EditorGUILayout.Space(4f);
            }
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
            object nextValue = DrawValue(label, field.FieldType, currentValue, path);

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
                EditorGUILayout.LabelField(label, "null");
                return null;
            }

            foldouts.TryGetValue(path, out bool isExpanded);
            isExpanded = EditorGUILayout.Foldout(isExpanded, label, true);
            foldouts[path] = isExpanded;

            if (isExpanded)
            {
                EditorGUI.indentLevel++;
                DrawSerializableFields(value, path);
                EditorGUI.indentLevel--;
            }

            return value;
        }

        private void DrawList(string label, IList list, string path)
        {
            foldouts.TryGetValue(path, out bool isExpanded);
            isExpanded = EditorGUILayout.Foldout(isExpanded, $"{label} ({list?.Count ?? 0})", true);
            foldouts[path] = isExpanded;

            if (!isExpanded || list == null)
                return;

            EditorGUI.indentLevel++;

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

        private static IEnumerable<BasePrivateScheme> GetSchemes(IPrivateModel model)
        {
            var collectionField = FindField(model.GetType(), "schemes");
            if (collectionField?.GetValue(model) is IEnumerable collection)
            {
                foreach (var item in collection)
                {
                    if (item is BasePrivateScheme scheme)
                        yield return scheme;
                }

                yield break;
            }

            var singleField = FindField(model.GetType(), "scheme");
            if (singleField == null)
                yield break;

            var singleScheme = singleField.GetValue(model) as BasePrivateScheme;
            if (singleScheme == null)
            {
                var getScheme = model.GetType().GetMethod(nameof(BaseSinglePrivateModel<BasePrivateScheme>.GetScheme), Type.EmptyTypes);
                singleScheme = getScheme?.Invoke(model, null) as BasePrivateScheme;
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

            return fileName.StartsWith(MODEL_NAMESPACE_PREFIX, StringComparison.Ordinal)
                ? fileName.Substring(MODEL_NAMESPACE_PREFIX.Length)
                : fileName;
        }

        private static string SanitizeFileName(string name)
        {
            foreach (var c in Path.GetInvalidFileNameChars())
                name = name.Replace(c, '_');

            return name.Replace(' ', '_');
        }
    }
}
