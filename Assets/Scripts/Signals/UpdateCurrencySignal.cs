using CurrencyType = Data.CurrencyPublicModel.Type;

namespace Signals
{
    public readonly struct UpdateCurrencySignal
    {
        public readonly CurrencyType type;
        public readonly int amount;

        public UpdateCurrencySignal(CurrencyType type, int amount)
        {
            this.type = type;
            this.amount = amount;
        }
    }
}
