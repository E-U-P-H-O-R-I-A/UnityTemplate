using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;

namespace Editor.Data_Editor
{
    public abstract class DataModelService
    {
        private const char HEADER_SEPARATOR = '\n';

        private readonly Dictionary<string, string> fileMeta = new();

        private string[] files = Array.Empty<string>();

        public abstract string Title { get; }
        public abstract string ShortTitle { get; }
        public abstract string FolderPath { get; }

        public virtual SerializedObject SerializedModel => null;

        public IReadOnlyList<string> Files => files;
        public int FileCount => files.Length;
        public string SelectedPath { get; private set; }
        public Type SelectedModelType { get; private set; }
        public object SelectedModel { get; private set; }
        public string LoadError { get; private set; }
        public bool HasPendingChanges { get; set; }
        public bool HasSelection => !string.IsNullOrEmpty(SelectedPath);

        public string Export()
        {
            if (SelectedModelType == null)
                return null;

            string payload = ExportPayload();

            return payload == null ? null : SelectedModelType.Name + HEADER_SEPARATOR + payload;
        }

        public bool TryImport(string json, out string error)
        {
            error = null;

            if (SelectedModelType == null)
            {
                error = "No model selected.";
                return false;
            }

            if (!TrySplitEnvelope(json, out string modelName, out string payload))
            {
                error = "Import data has no model header, export it from Data Utility first.";
                return false;
            }

            if (modelName != SelectedModelType.Name)
            {
                error = $"Import data belongs to {modelName}, not to {SelectedModelType.Name}.";
                return false;
            }

            return TryImportPayload(payload, out error);
        }

        public void Refresh()
        {
            EnsureFolder();
            EnsureFiles();

            files = Directory
                .GetFiles(FolderPath, FileFilter, SearchOption.TopDirectoryOnly)
                .Select(ModelNaming.NormalizePath)
                .OrderBy(Path.GetFileName)
                .ToArray();

            CacheFileMeta();

            Select(HasSelection && files.Contains(SelectedPath) ? SelectedPath : files.FirstOrDefault());
        }

        public void Select(string path)
        {
            if (HasPendingChanges && path != SelectedPath)
                DiscardPendingChanges();

            SelectedPath = path;
            SelectedModelType = null;
            SelectedModel = null;
            LoadError = null;
            HasPendingChanges = false;

            if (HasSelection)
            {
                SelectedModelType = FindModelType(path);

                if (SelectedModelType == null)
                    LoadError = $"Could not find a model type for file: {Path.GetFileName(path)}";
                else
                    LoadSelected(path);
            }

            OnSelectionChanged();
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

            EnsureFolder();
            WriteModel(SelectedPath, SelectedModel);

            HasPendingChanges = false;

            CacheFileMeta();
        }

        public void Revert()
        {
            if (!HasSelection)
                return;

            DiscardPendingChanges();
            Select(SelectedPath);
        }

        public void DiscardPendingChanges()
        {
            if (!HasPendingChanges)
                return;

            DiscardChanges();
            HasPendingChanges = false;
        }

        public void DeleteSelected()
        {
            if (!HasSelection || !File.Exists(SelectedPath))
                return;

            DeleteFile(SelectedPath);
            SelectedPath = null;

            Refresh();
        }

        public void DeleteAll()
        {
            foreach (var file in files)
            {
                if (File.Exists(file))
                    DeleteFile(file);
            }

            SelectedPath = null;

            Refresh();
        }

        protected void ReplaceSelectedModel(object model)
        {
            SelectedModel = model;
            LoadError = null;
            HasPendingChanges = true;

            OnSelectionChanged();
        }

        protected abstract string FileFilter { get; }

        protected abstract string ExportPayload();

        protected abstract bool TryImportPayload(string payload, out string error);

        protected abstract Type FindModelType(string path);

        protected abstract object LoadModel(string path);

        protected abstract void WriteModel(string path, object model);

        protected virtual void EnsureFolder() =>
            Directory.CreateDirectory(FolderPath);

        protected virtual void EnsureFiles()
        {
        }

        protected virtual void DiscardChanges()
        {
        }

        protected virtual void DeleteFile(string path) =>
            File.Delete(path);

        protected virtual void OnSelectionChanged()
        {
        }

        private static bool TrySplitEnvelope(string json, out string modelName, out string payload)
        {
            modelName = null;
            payload = null;

            if (string.IsNullOrWhiteSpace(json))
                return false;

            int separator = json.IndexOf(HEADER_SEPARATOR);

            if (separator <= 0)
                return false;

            modelName = json.Substring(0, separator).Trim();
            payload = json.Substring(separator + 1);

            return modelName.Length > 0 &&
                   !modelName.StartsWith("{") &&
                   !string.IsNullOrWhiteSpace(payload);
        }

        private void LoadSelected(string path)
        {
            try
            {
                SelectedModel = LoadModel(path);

                if (SelectedModel == null)
                    LoadError = $"Could not load {Path.GetFileName(path)}";
            }
            catch (Exception e)
            {
                SelectedModel = null;
                LoadError = e.Message;
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

        private static bool Contains(string value, string query) =>
            value.IndexOf(query, StringComparison.OrdinalIgnoreCase) >= 0;
    }
}
