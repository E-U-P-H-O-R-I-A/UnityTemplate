using System;
using System.IO;
using System.Linq;
using Data;
using UnityEngine;

namespace EditorTools.DataEditor
{
    public class PrivateDataService : DataContainerService
    {
        private const string FOLDER_NAME = "PrivateData";
        private const string TEMP_EXTENSION = ".tmp";

        public override string Title => "Private Containers";
        public override string ShortTitle => "Private";

        public override string FolderPath =>
            ContainerNaming.NormalizePath(Path.Combine(Application.persistentDataPath, FOLDER_NAME));

        protected override string FileFilter => "*.json";

        protected override string ExportPayload() =>
            SelectedContainer is IPrivateContainer container ? container.ExportToJson() : null;

        protected override bool TryImportPayload(string payload, out string error)
        {
            error = null;

            try
            {
                var dump = JsonUtility.FromJson<RecordDump>(payload);

                if (dump?.items == null || dump.items.Count == 0)
                    throw new Exception("Import data contains no records.");

                var container = (IPrivateContainer)Activator.CreateInstance(SelectedContainerType);
                container.ImportFromJson(payload);

                int applied = RecordReflection.GetRecords(container).Count();

                if (applied == 0)
                    throw new Exception($"None of the {dump.items.Count} record(s) fit {SelectedContainerType.Name}.");

                if (applied < dump.items.Count)
                {
                    Debug.LogWarning($"[Data Utility] {dump.items.Count - applied} record(s) were skipped " +
                                     $"while importing into {SelectedContainerType.Name}.");
                }

                ReplaceSelectedContainer(container);

                return true;
            }
            catch (Exception e)
            {
                error = e.Message;
                return false;
            }
        }

        protected override Type FindContainerType(string path) =>
            RecordReflection.FindContainerType(path, typeof(IPrivateContainer));

        protected override object LoadContainer(string path)
        {
            var container = (IPrivateContainer)Activator.CreateInstance(SelectedContainerType);
            container.ImportFromJson(File.Exists(path) ? File.ReadAllText(path) : string.Empty);

            return container;
        }

        protected override void WriteContainer(string path, object container)
        {
            string temporary = path + TEMP_EXTENSION;

            File.WriteAllText(temporary, ((IPrivateContainer)container).ExportToJson());

            if (File.Exists(path))
                File.Delete(path);

            File.Move(temporary, path);
        }
    }
}
