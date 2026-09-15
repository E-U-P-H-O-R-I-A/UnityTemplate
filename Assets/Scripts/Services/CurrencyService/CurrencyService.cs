using Data;
using MessagePipe;
using Services.PrivateModelProvider;
using Signals;
using CurrencyType = Data.CurrencyPublicModel.Type;

namespace Services.CurrencyService
{
    public class CurrencyService : ICurrencyService
    {
        private readonly IPrivateModelProvider privateModelProvider;
        private readonly IPublisher<UpdateCurrencySignal> updateCurrencyPublisher;

        private CurrencyPrivateModel currencyPrivateModel;

        public CurrencyService(IPrivateModelProvider privateModelProvider,
            IPublisher<UpdateCurrencySignal> updateCurrencyPublisher)
        {
            this.privateModelProvider = privateModelProvider;
            this.updateCurrencyPublisher = updateCurrencyPublisher;
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
            Publish(transaction.type);
            Save();
        }

        public bool DecreaseCurrency(CurrencyTransaction transaction)
        {
            if (!IsEnoughCurrency(transaction))
                return false;
            
            GetScheme(transaction.type).DecreaseCurrency(transaction.amount);
            Publish(transaction.type);
            Save();

            return true;
        }

        private void Save() => 
            privateModelProvider.SaveModel<CurrencyPrivateModel>();

        private CurrencyPrivateScheme GetScheme(CurrencyType currencyType) => 
            currencyPrivateModel.GetScheme(currencyType.ToString());

        private void Publish(CurrencyType currencyType) => 
            updateCurrencyPublisher.Publish(new UpdateCurrencySignal(currencyType, GetAmountCurrency(currencyType)));
    }
}
