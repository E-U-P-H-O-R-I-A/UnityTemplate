namespace Data
{
    public interface IPrivateContainer
    {
        public string ExportToJson();

        public void ImportFromJson(string json);
    }
}