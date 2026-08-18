using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;

namespace Editor.Data_Editor
{
    public abstract class DataModelService
    {
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

        public abstract string Export();

        public abstract bool TryImport(string json, out string error);

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
