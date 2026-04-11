using Data.Model.Private;
using Data.Scheme.Private;
using Data.Scheme.Public;
using Services.PrivateModelProvider;

namespace Services.CurrencyService
{
    public class CurrencyService : ICurrencyService
    {
        private readonly IPrivateModelProvider privateModelProvider;
        
        private CurrencyPrivateModel currencyPrivateModel;

        public CurrencyService(IPrivateModelProvider privateModelProvider)
        {
            this.privateModelProvider = privateModelProvider;
        }

        public void Init() => 
            currencyPrivateModel = privateModelProvider.GetModel<CurrencyPrivateModel>();

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
            privateModelProvider.SaveModel<CurrencyPrivateModel>();

        private CurrencyPrivateScheme GetScheme(CurrencyType currencyType) => 
            currencyPrivateModel.GetScheme(((int)currencyType).ToString());
    }
}
