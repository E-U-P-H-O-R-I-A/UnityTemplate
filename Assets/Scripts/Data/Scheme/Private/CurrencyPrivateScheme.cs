namespace Data.Scheme.Private
{
    public class CurrencyPrivateScheme : BasePrivateScheme
    {
        private string id;

        private int value;

        public int Value => value;
        public override string ID => id;

        public CurrencyPrivateScheme(string id, int value = 0)
        {
            this.id = id;
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