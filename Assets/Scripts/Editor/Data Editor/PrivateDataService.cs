using System;
using System.IO;
using System.Linq;
using Data;
using UnityEngine;

namespace Editor.Data_Editor
{
    public class PrivateDataService : DataModelService
    {
        private const string FOLDER_NAME = "PrivateData";
        private const string TEMP_EXTENSION = ".tmp";

        public override string Title => "Private Models";
        public override string ShortTitle => "Private";

        public override string FolderPath =>
            ModelNaming.NormalizePath(Path.Combine(Application.persistentDataPath, FOLDER_NAME));

        protected override string FileFilter => "*.json";

        protected override string ExportPayload() =>
            SelectedModel is IPrivateModel model ? model.ExportToJson() : null;

        protected override bool TryImportPayload(string payload, out string error)
        {
            error = null;

            try
            {
                var dump = JsonUtility.FromJson<SchemesDump>(payload);

                if (dump?.items == null || dump.items.Count == 0)
                    throw new Exception("Import data contains no schemes.");

                var model = (IPrivateModel)Activator.CreateInstance(SelectedModelType);
                model.ImportFromJson(payload);

                int applied = SchemeReflection.GetSchemes(model).Count();

                if (applied == 0)
                    throw new Exception($"None of the {dump.items.Count} scheme(s) fit {SelectedModelType.Name}.");

                if (applied < dump.items.Count)
                {
                    Debug.LogWarning($"[Data Utility] {dump.items.Count - applied} scheme(s) were skipped " +
                                     $"while importing into {SelectedModelType.Name}.");
                }

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

        protected override void WriteModel(string path, object model)
        {
            string temporary = path + TEMP_EXTENSION;

            File.WriteAllText(temporary, ((IPrivateModel)model).ExportToJson());

            if (File.Exists(path))
                File.Delete(path);

            File.Move(temporary, path);
        }
    }
}
