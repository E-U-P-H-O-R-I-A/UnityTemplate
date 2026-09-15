namespace Data
{
    public class SettingsPrivateContainer : PrivateContainer.Single<SettingsPrivateRecord>
    {
        protected override SettingsPrivateRecord CreateRecord() => new();
    }
}
