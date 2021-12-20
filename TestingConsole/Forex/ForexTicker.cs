namespace BudgetManager.TestingConsole
{
    public class ForexTicker
    {
        private ForexTicker(string value)
        {
            this.Value = value;
        }

        public string Value { get; private set; }

        public static ForexTicker EurUsd => new ForexTicker("EUR/USD");
        public static ForexTicker EurCzk = new ForexTicker("EUR/CZK");
        public static ForexTicker UsdEur = new ForexTicker("USD/EUR");
        public static ForexTicker UsdCzk = new ForexTicker("USD/CZK");
        public static ForexTicker CzkEur = new ForexTicker("CZK/EUR");
        public static ForexTicker CzkUsd = new ForexTicker("CZK/USD");
    }
}
