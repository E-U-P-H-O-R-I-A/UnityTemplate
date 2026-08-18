using System;
using System.Linq;
using Data;
using UnityEditor;
using UnityEngine;

namespace Editor.Data_Editor
{
    public class PublicDataService : DataModelService
    {
        public const string FOLDER = "Assets/Resources_moved/Data";
        private const string ASSET_EXTENSION = ".asset";

        private SerializedObject serializedModel;
        private string snapshot;

        public override string Title => "Public Models";
        public override string ShortTitle => "Public";
        public override string FolderPath => FOLDER;
        public override SerializedObject SerializedModel => serializedModel;

        protected override string FileFilter => "*" + ASSET_EXTENSION;

        public static string GetAssetPath(Type modelType) =>
            $"{FOLDER}/{modelType.Name}{ASSET_EXTENSION}";

        public override string Export() =>
            SelectedModel is ScriptableObject asset ? EditorJsonUtility.ToJson(asset, true) : null;

        public override bool TryImport(string json, out string error)
        {
            error = null;

            if (SelectedModel is not ScriptableObject asset)
            {
                error = "No model selected.";
                return false;
            }

            try
            {
                EditorJsonUtility.FromJsonOverwrite(json, asset);

                EditorUtility.SetDirty(asset);
                ReplaceSelectedModel(asset);

                return true;
            }
            catch (Exception e)
            {
                error = e.Message;
                return false;
            }
        }

        protected override Type FindModelType(string path) =>
            SchemeReflection.FindModelType(path, typeof(IPublicModel));

        protected override object LoadModel(string path) =>
            AssetDatabase.LoadAssetAtPath<ScriptableObject>(path);

        protected override void WriteModel(string path, object model)
        {
            var asset = (ScriptableObject)model;

            EditorUtility.SetDirty(asset);
            AssetDatabase.SaveAssets();

            var ids = SchemeReflection
                .GetSchemes(asset)
                .Select(scheme => scheme.ID)
                .ToList();

            snapshot = EditorJsonUtility.ToJson(asset);

            if (PublicModelCodeGenerator.Generate(asset.GetType(), ids))
                AssetDatabase.Refresh();
        }

        protected override void EnsureFiles()
        {
            bool created = false;

            foreach (var modelType in SchemeReflection.GetConcreteTypes(typeof(IPublicModel)))
            {
                string path = GetAssetPath(modelType);

                if (AssetDatabase.LoadAssetAtPath<ScriptableObject>(path) != null)
                    continue;

                var asset = ScriptableObject.CreateInstance(modelType);
                asset.name = modelType.Name;

                AssetDatabase.CreateAsset(asset, path);
                created = true;
            }

            if (created)
                AssetDatabase.SaveAssets();
        }

        protected override void DeleteFile(string path) =>
            AssetDatabase.DeleteAsset(path);

        protected override void OnSelectionChanged()
        {
            var asset = SelectedModel as ScriptableObject;

            serializedModel = asset == null ? null : new SerializedObject(asset);
            snapshot = asset == null ? null : EditorJsonUtility.ToJson(asset);
        }

        protected override void DiscardChanges()
        {
            if (SelectedModel is not ScriptableObject asset || string.IsNullOrEmpty(snapshot))
                return;

            EditorJsonUtility.FromJsonOverwrite(snapshot, asset);
            EditorUtility.ClearDirty(asset);
        }
    }
}
