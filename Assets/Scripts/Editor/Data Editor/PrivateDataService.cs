using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Data;
using UnityEngine;

namespace Editor.DataEditor
{
    /// <summary>
    /// Owns the save files on disk and the currently inspected model.
    /// Contains no GUI code: dialogs and notifications belong to the window.
    /// </summary>
    public class PrivateDataService
    {
        private const string FOLDER_NAME = "PrivateData";
        private const string JSON_EXTENSION = "*.json";

        private readonly Dictionary<string, string> fileMeta = new();

        private string[] files = Array.Empty<string>();

        public static string FolderPath =>
            Path.Combine(Application.persistentDataPath, FOLDER_NAME);

        public IReadOnlyList<string> Files => files;
        public int FileCount => files.Length;
        public string SelectedPath { get; private set; }
        public Type SelectedModelType { get; private set; }
        public IPrivateModel SelectedModel { get; private set; }
        public string LoadError { get; private set; }
        public bool HasPendingChanges { get; set; }
        public bool HasSelection => !string.IsNullOrEmpty(SelectedPath);

        public void Refresh()
        {
            Directory.CreateDirectory(FolderPath);

            files = Directory
                .GetFiles(FolderPath, JSON_EXTENSION, SearchOption.TopDirectoryOnly)
                .OrderBy(Path.GetFileName)
                .ToArray();

            CacheFileMeta();

            if (HasSelection && files.Contains(SelectedPath))
            {
                Select(SelectedPath);
                return;
            }

            if (files.Length > 0)
            {
                Select(files[0]);
                return;
            }

            ClearSelection();
        }

        public void Select(string path)
        {
            SelectedPath = path;
            SelectedModelType = SchemeReflection.FindModelType(path);
            SelectedModel = null;
            LoadError = null;
            HasPendingChanges = false;

            if (SelectedModelType == null)
            {
                LoadError = $"Could not find private model type for file: {Path.GetFileName(path)}";
                return;
            }

            try
            {
                SelectedModel = (IPrivateModel)Activator.CreateInstance(SelectedModelType);

                string fileJson = File.Exists(path) ? File.ReadAllText(path) : string.Empty;
                SelectedModel.ImportFromJson(fileJson);
            }
            catch (Exception e)
            {
                SelectedModel = null;
                LoadError = e.Message;
            }
        }

        public string[] Filter(string query)
        {
            if (string.IsNullOrWhiteSpace(query))
                return files;

            string trimmed = query.Trim();

            return files
                .Where(file =>
                    Contains(ModelNaming.GetDisplayName(file), trimmed) ||
                    Contains(Path.GetFileNameWithoutExtension(file), trimmed))
                .ToArray();
        }

        public string GetFileMeta(string path)
        {
            if (string.IsNullOrEmpty(path))
                return string.Empty;

            return fileMeta.TryGetValue(path, out string meta) ? meta : string.Empty;
        }

        public void Save()
        {
            if (!HasSelection || SelectedModel == null)
                return;

            Directory.CreateDirectory(FolderPath);
            File.WriteAllText(SelectedPath, SelectedModel.ExportToJson());
            HasPendingChanges = false;

            CacheFileMeta();
        }

        public void DeleteSelected()
        {
            if (!HasSelection || !File.Exists(SelectedPath))
                return;

            File.Delete(SelectedPath);
            SelectedPath = null;

            Refresh();
        }

        public void DeleteAll()
        {
            foreach (var file in files)
            {
                if (File.Exists(file))
                    File.Delete(file);
            }

            SelectedPath = null;

            Refresh();
        }

        public string Export()
        {
            return SelectedModel?.ExportToJson();
        }

        public bool TryImport(string json, out string error)
        {
            error = null;

            if (SelectedModelType == null)
            {
                error = "No model selected.";
                return false;
            }

            try
            {
                var dump = JsonUtility.FromJson<SchemesDump>(json);
                if (dump?.items == null || dump.items.Count == 0)
                    throw new Exception("Clipboard JSON contains no schemes.");

                var model = (IPrivateModel)Activator.CreateInstance(SelectedModelType);
                model.ImportFromJson(json);

                SelectedModel = model;
                LoadError = null;
                HasPendingChanges = true;

                return true;
            }
            catch (Exception e)
            {
                error = e.Message;
                return false;
            }
        }

        private void CacheFileMeta()
        {
            fileMeta.Clear();

            foreach (var file in files)
            {
                var info = new FileInfo(file);
                fileMeta[file] = info.Exists ? ModelNaming.FormatSize(info.Length) : "missing";
            }
        }

        private void ClearSelection()
        {
            SelectedPath = null;
            SelectedModelType = null;
            SelectedModel = null;
            LoadError = null;
            HasPendingChanges = false;
        }

        private static bool Contains(string value, string query) =>
            value.IndexOf(query, StringComparison.OrdinalIgnoreCase) >= 0;
    }
}
