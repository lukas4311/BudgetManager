namespace BudgetManager.Domain.DTOs
{
    public class CryptoTickerModel
    {
        /// <summary>
        /// Gets or sets the unique identifier for the cryptocurrency ticker.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Gets or sets the ticker symbol of the cryptocurrency.
        /// </summary>
        public string Ticker { get; set; }

        /// <summary>
        /// Gets or sets the name of the cryptocurrency.
        /// </summary>
        public string Name { get; set; }
    }
}
