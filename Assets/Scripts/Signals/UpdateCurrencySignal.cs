using CurrencyId = Data.CurrencyPublicContainer.Id;

namespace Signals
{
    public readonly struct UpdateCurrencySignal
    {
        public readonly CurrencyId type;
        public readonly int amount;

        public UpdateCurrencySignal(CurrencyId type, int amount)
        {
            this.type = type;
            this.amount = amount;
        }
    }
}
