using Data.Scheme.Public;

namespace Services.CurrencyService
{
    public interface ICurrencyService
    {
        void Init();
        
        int GetAmountCurrency(CurrencyType currencyType);

        bool IsEnoughCurrency(CurrencyType currencyType, int amount);

        void IncreaseCurrency(CurrencyType currencyType, int amount);

        void DecreaseCurrency(CurrencyType currencyType, int amount);
    }
}