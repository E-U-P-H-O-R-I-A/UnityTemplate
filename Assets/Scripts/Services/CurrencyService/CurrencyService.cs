using Data;
using MessagePipe;
using Services.PrivateContainerProvider;
using Signals;
using CurrencyId = Data.CurrencyPublicContainer.Id;

namespace Services.CurrencyService
{
    public class CurrencyService : ICurrencyService
    {
        private readonly IPrivateContainerProvider privateContainerProvider;
        private readonly IPublisher<UpdateCurrencySignal> updateCurrencyPublisher;

        private CurrencyPrivateContainer currencyPrivateContainer;

        public CurrencyService(IPrivateContainerProvider privateContainerProvider,
            IPublisher<UpdateCurrencySignal> updateCurrencyPublisher)
        {
            this.privateContainerProvider = privateContainerProvider;
            this.updateCurrencyPublisher = updateCurrencyPublisher;
        }

        public void Initialize() => 
            currencyPrivateContainer = privateContainerProvider.GetContainer<CurrencyPrivateContainer>();

        public int GetAmountCurrency(CurrencyId currencyType) => 
            GetRecord(currencyType).Value;

        public bool IsEnoughCurrency(CurrencyTransaction transaction) => 
            GetRecord(transaction.type).IsEnough(transaction.amount);

        public void IncreaseCurrency(CurrencyTransaction transaction)
        {
            GetRecord(transaction.type).Increase(transaction.amount);
            Publish(transaction.type);
            Save();
        }

        public bool DecreaseCurrency(CurrencyTransaction transaction)
        {
            if (!IsEnoughCurrency(transaction))
                return false;
            
            GetRecord(transaction.type).Decrease(transaction.amount);
            Publish(transaction.type);
            Save();

            return true;
        }

        private void Save() => 
            privateContainerProvider.SaveContainer<CurrencyPrivateContainer>();

        private CurrencyPrivateRecord GetRecord(CurrencyId currencyType) => 
            currencyPrivateContainer.GetRecord(currencyType.ToString());

        private void Publish(CurrencyId currencyType) => 
            updateCurrencyPublisher.Publish(new UpdateCurrencySignal(currencyType, GetAmountCurrency(currencyType)));
    }
}
