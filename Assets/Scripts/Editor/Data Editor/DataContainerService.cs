using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;

namespace Editor.Data_Editor
{
    public abstract class DataContainerService
    {
        private const char HEADER_SEPARATOR = '\n';

        private readonly Dictionary<string, string> fileMeta = new();

        private string[] files = Array.Empty<string>();

        public abstract string Title { get; }
        public abstract string ShortTitle { get; }
        public abstract string FolderPath { get; }

        public virtual SerializedObject SerializedContainer => null;

        public IReadOnlyList<string> Files => files;
        public int FileCount => files.Length;
        public string SelectedPath { get; private set; }
        public Type SelectedContainerType { get; private set; }
        public object SelectedContainer { get; private set; }
        public string LoadError { get; private set; }
        public bool HasPendingChanges { get; set; }
        public bool HasSelection => !string.IsNullOrEmpty(SelectedPath);

        public string Export()
        {
            if (SelectedContainerType == null)
                return null;

            string payload = ExportPayload();

            return payload == null ? null : SelectedContainerType.Name + HEADER_SEPARATOR + payload;
        }

        public bool TryImport(string json, out string error)
        {
            error = null;

            if (SelectedContainerType == null)
            {
                error = "No container selected.";
                return false;
            }

            if (!TrySplitEnvelope(json, out string containerName, out string payload))
            {
                error = "Import data has no container header, export it from Data Utility first.";
                return false;
            }

            if (containerName != SelectedContainerType.Name)
            {
                error = $"Import data belongs to {containerName}, not to {SelectedContainerType.Name}.";
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
                .Select(ContainerNaming.NormalizePath)
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
            SelectedContainerType = null;
            SelectedContainer = null;
            LoadError = null;
            HasPendingChanges = false;

            if (HasSelection)
            {
                SelectedContainerType = FindContainerType(path);

                if (SelectedContainerType == null)
                    LoadError = $"Could not find a container type for file: {Path.GetFileName(path)}";
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
                    Contains(ContainerNaming.GetDisplayName(file), trimmed) ||
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
            if (!HasSelection || SelectedContainer == null)
                return;

            EnsureFolder();
            WriteContainer(SelectedPath, SelectedContainer);

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

        protected void ReplaceSelectedContainer(object container)
        {
            SelectedContainer = container;
            LoadError = null;
            HasPendingChanges = true;

            OnSelectionChanged();
        }

        protected abstract string FileFilter { get; }

        protected abstract string ExportPayload();

        protected abstract bool TryImportPayload(string payload, out string error);

        protected abstract Type FindContainerType(string path);

        protected abstract object LoadContainer(string path);

        protected abstract void WriteContainer(string path, object container);

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

        private static bool TrySplitEnvelope(string json, out string containerName, out string payload)
        {
            containerName = null;
            payload = null;

            if (string.IsNullOrWhiteSpace(json))
                return false;

            int separator = json.IndexOf(HEADER_SEPARATOR);

            if (separator <= 0)
                return false;

            containerName = json.Substring(0, separator).Trim();
            payload = json.Substring(separator + 1);

            return containerName.Length > 0 &&
                   !containerName.StartsWith("{") &&
                   !string.IsNullOrWhiteSpace(payload);
        }

        private void LoadSelected(string path)
        {
            try
            {
                SelectedContainer = LoadContainer(path);

                if (SelectedContainer == null)
                    LoadError = $"Could not load {Path.GetFileName(path)}";
            }
            catch (Exception e)
            {
                SelectedContainer = null;
                LoadError = e.Message;
            }
        }

        private void CacheFileMeta()
        {
            fileMeta.Clear();

            foreach (var file in files)
            {
                var info = new FileInfo(file);
                fileMeta[file] = info.Exists ? ContainerNaming.FormatSize(info.Length) : "missing";
            }
        }

        private static bool Contains(string value, string query) =>
            value.IndexOf(query, StringComparison.OrdinalIgnoreCase) >= 0;
    }
}
