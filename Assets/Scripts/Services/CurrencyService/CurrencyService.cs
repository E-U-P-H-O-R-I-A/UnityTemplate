using Data.Model.Private;
using Data.Scheme.Private;
using Data.Scheme.Public;
using Services.PrivateModelProvider;
using VContainer;

namespace Services.CurrencyService
{
    public class CurrencyService : ICurrencyService
    {
        [Inject] public IPrivateModelProvider PrivateModelProvider { get; set; }

        private CurrencyPrivateModel currencyPrivateModel;

        public void Init() => 
            currencyPrivateModel = PrivateModelProvider.GetModel<CurrencyPrivateModel>();

        public int GetAmountCurrency(CurrencyType currencyType) => 
            GetScheme(currencyType).Value;

        public bool IsEnoughCurrency(CurrencyType currencyType, int amount) => 
            GetScheme(currencyType).IsEnoughCurrency(amount);

        public void IncreaseCurrency(CurrencyType currencyType, int amount)
        {
            GetScheme(currencyType).IncreaseCurrency(amount);
            Save();
        }

        public void DecreaseCurrency(CurrencyType currencyType, int amount)
        {
            GetScheme(currencyType).DecreaseCurrency(amount);
            Save();
        }

        private void Save() => 
            PrivateModelProvider.SaveModel<CurrencyPrivateModel>();

        private CurrencyPrivateScheme GetScheme(CurrencyType currencyType) => 
            currencyPrivateModel.GetScheme((int)currencyType);
    }
}