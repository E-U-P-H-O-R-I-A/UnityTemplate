using Data.Scheme.Public;

namespace Data.Scheme.Private
{
    public class CurrencyPrivateScheme : BasePrivateScheme
    {
        private int type;

        private int value;

        public int Value => value;
        public override int ID => type;
        public CurrencyType Type => (CurrencyType)type;

        public CurrencyPrivateScheme(CurrencyType type, int value = 0)
        {
            this.type = (int)type;
            this.value = value;
        }

        public bool IsEnoughCurrency(int value)
        {
            return this.value >= value;
        }

        public void IncreaseCurrency(int value)
        {
            this.value += value;
        }

        public void DecreaseCurrency(int value)
        {
            this.value -= value;
        }
    }
}