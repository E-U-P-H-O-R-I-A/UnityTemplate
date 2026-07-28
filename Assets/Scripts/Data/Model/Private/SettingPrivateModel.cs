namespace Data
{
    public class SettingPrivateModel : PrivateModel.Single<SettingPrivateScheme>
    {
        protected override SettingPrivateScheme CreateScheme() => new();
    }
}
