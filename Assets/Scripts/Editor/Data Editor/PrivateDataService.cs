using System;
using System.IO;
using Data;
using UnityEngine;

namespace Editor.Data_Editor
{
    public class PrivateDataService : DataModelService
    {
        private const string FOLDER_NAME = "PrivateData";

        public override string Title => "Private Models";
        public override string ShortTitle => "Private";

        public override string FolderPath =>
            ModelNaming.NormalizePath(Path.Combine(Application.persistentDataPath, FOLDER_NAME));

        protected override string FileFilter => "*.json";

        public override string Export() =>
            SelectedModel is IPrivateModel model ? model.ExportToJson() : null;

        public override bool TryImport(string json, out string error)
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

                ReplaceSelectedModel(model);

                return true;
            }
            catch (Exception e)
            {
                error = e.Message;
                return false;
            }
        }

        protected override Type FindModelType(string path) =>
            SchemeReflection.FindModelType(path, typeof(IPrivateModel));

        protected override object LoadModel(string path)
        {
            var model = (IPrivateModel)Activator.CreateInstance(SelectedModelType);
            model.ImportFromJson(File.Exists(path) ? File.ReadAllText(path) : string.Empty);

            return model;
        }

        protected override void WriteModel(string path, object model) =>
            File.WriteAllText(path, ((IPrivateModel)model).ExportToJson());
    }
}
