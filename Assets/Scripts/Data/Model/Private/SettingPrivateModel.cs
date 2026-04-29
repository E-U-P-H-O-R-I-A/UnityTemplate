using Data.Scheme.Private;

namespace Data.Model.Private
{
    public class SettingPrivateModel : BaseSinglePrivateModel<SettingPrivateScheme>
    {
        protected override SettingPrivateScheme CreateScheme() => new();
    }
}