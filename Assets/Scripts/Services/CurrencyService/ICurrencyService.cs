using Data;
using CurrencyType = Data.CurrencyPublicModel.Type;

namespace Services.CurrencyService
{
    public interface ICurrencyService
    {
        void Initialize();
        
        int GetAmountCurrency(CurrencyType currencyType);
        void IncreaseCurrency(CurrencyTransaction transaction);
        bool IsEnoughCurrency(CurrencyTransaction transaction);
        bool DecreaseCurrency(CurrencyTransaction transaction);
    }
}
