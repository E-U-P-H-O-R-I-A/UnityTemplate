namespace Data
{
    public interface IPrivateModel
    {
        public string ExportToJson();

        public void ImportFromJson(string json);
    }
}