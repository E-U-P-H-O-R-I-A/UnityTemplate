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

        public void Initialize() => 
            currencyPrivateModel = privateModelProvider.GetModel<CurrencyPrivateModel>();

        public int GetAmountCurrency(CurrencyType currencyType) => 
            GetScheme(currencyType).Value;

        public bool IsEnoughCurrency(CurrencyTransaction transaction) => 
            GetScheme(transaction.type).IsEnoughCurrency(transaction.amount);

        public void IncreaseCurrency(CurrencyTransaction transaction)
        {
            GetScheme(transaction.type).IncreaseCurrency(transaction.amount);
            Save();
        }

        public bool DecreaseCurrency(CurrencyTransaction transaction)
        {
            if (!IsEnoughCurrency(transaction))
                return false;
            
            GetScheme(transaction.type).DecreaseCurrency(transaction.amount);
            Save();

            return true;
        }

        private void Save() => 
            privateModelProvider.SaveModel<CurrencyPrivateModel>();

        private CurrencyPrivateScheme GetScheme(CurrencyType currencyType) => 
            currencyPrivateModel.GetScheme(((int)currencyType).ToString());
    }
}
