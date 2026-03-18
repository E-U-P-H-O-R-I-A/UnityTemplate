namespace Data.Model
{
    public interface IPrivateModel
    {
        public string ExportToJson();

        public void ImportFromJson(string json);
    }
}