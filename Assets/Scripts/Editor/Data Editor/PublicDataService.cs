using System;
using System.Linq;
using Data;
using UnityEditor;
using UnityEngine;

namespace EditorTools.DataEditor
{
    public class PublicDataService : DataContainerService
    {
        public const string FOLDER = "Assets/Data";
        private const string ASSET_EXTENSION = ".asset";

        private SerializedObject serializedContainer;
        private string snapshot;

        public override string Title => "Public Containers";
        public override string ShortTitle => "Public";
        public override string FolderPath => FOLDER;
        public override SerializedObject SerializedContainer => serializedContainer;

        protected override string FileFilter => "*" + ASSET_EXTENSION;

        public static string GetAssetPath(Type containerType) =>
            $"{FOLDER}/{containerType.Name}{ASSET_EXTENSION}";

        protected override string ExportPayload() =>
            SelectedContainer is ScriptableObject asset ? EditorJsonUtility.ToJson(asset, true) : null;

        protected override bool TryImportPayload(string payload, out string error)
        {
            error = null;

            if (SelectedContainer is not ScriptableObject asset)
            {
                error = "No container selected.";
                return false;
            }

            string baseline = snapshot;

            try
            {
                EditorJsonUtility.FromJsonOverwrite(payload, asset);

                EditorUtility.SetDirty(asset);
                ReplaceSelectedContainer(asset);

                snapshot = baseline;

                return true;
            }
            catch (Exception e)
            {
                error = e.Message;
                return false;
            }
        }

        protected override Type FindContainerType(string path) =>
            RecordReflection.FindContainerType(path, typeof(IPublicContainer));

        protected override object LoadContainer(string path) =>
            AssetDatabase.LoadAssetAtPath<ScriptableObject>(path);

        protected override void WriteContainer(string path, object container)
        {
            var asset = (ScriptableObject)container;

            EditorUtility.SetDirty(asset);
            AssetDatabase.SaveAssetIfDirty(asset);

            var ids = RecordReflection
                .GetRecords(asset)
                .Select(record => record.Id)
                .ToList();

            snapshot = EditorJsonUtility.ToJson(asset);

            if (PublicContainerCodeGenerator.Generate(asset.GetType(), ids))
                AssetDatabase.Refresh();
        }

        protected override void EnsureFiles()
        {
            foreach (var containerType in RecordReflection.GetConcreteTypes(typeof(IPublicContainer)))
            {
                string path = GetAssetPath(containerType);

                if (AssetDatabase.LoadAssetAtPath<ScriptableObject>(path) != null)
                    continue;

                var asset = ScriptableObject.CreateInstance(containerType);
                asset.name = containerType.Name;

                AssetDatabase.CreateAsset(asset, path);
            }
        }

        protected override void DeleteFile(string path) =>
            AssetDatabase.DeleteAsset(path);

        protected override void OnSelectionChanged()
        {
            var asset = SelectedContainer as ScriptableObject;

            serializedContainer = asset == null ? null : new SerializedObject(asset);
            snapshot = asset == null ? null : EditorJsonUtility.ToJson(asset);
        }

        protected override void DiscardChanges()
        {
            if (SelectedContainer is not ScriptableObject asset || string.IsNullOrEmpty(snapshot))
                return;

            EditorJsonUtility.FromJsonOverwrite(snapshot, asset);
            EditorUtility.ClearDirty(asset);
        }
    }
}
