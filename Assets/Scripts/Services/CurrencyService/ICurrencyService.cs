using Data;
using CurrencyId = Data.CurrencyPublicContainer.Id;

namespace Services.CurrencyService
{
    public interface ICurrencyService
    {
        void Initialize();
        
        int GetAmountCurrency(CurrencyId currencyType);
        void IncreaseCurrency(CurrencyTransaction transaction);
        bool IsEnoughCurrency(CurrencyTransaction transaction);
        bool DecreaseCurrency(CurrencyTransaction transaction);
    }
}
