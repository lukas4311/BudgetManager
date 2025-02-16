using BudgetManager.Services;
using BudgetManager.Services.Contracts;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace BudgetManager.FinancialApi.Endpoints
{
    public static class ComodityEndpoints
    {
        private const string GoldPriceCurrency = "USD";

        public static void RegisterComodityEndpoints(this WebApplication app)
        {
            app.MapGet("/gold/actualPrice", GetCurrentGoldPriceForOunce)
                .WithName(nameof(GetCurrentGoldPriceForOunce))
                .WithOpenApi()
                .Produces(StatusCodes.Status200OK, contentType: "application/json");

            app.MapGet("gold/actualPrice/{currencyCode}", GetCurrentGoldPriceForOunceForSpecificCurrency)
                .WithName(nameof(GetCurrentGoldPriceForOunceForSpecificCurrency))
                .WithOpenApi()
                .Produces(StatusCodes.Status200OK, contentType: "application/json")
                .Produces(StatusCodes.Status400BadRequest);
        }

        /// <summary>
        /// Method to get current price for ounce of gold
        /// </summary>
        /// <param name="comodityService">Comodity service</param>
        /// <returns>Price ounce of gold</returns>
        public static async Task<Ok<double>> GetCurrentGoldPriceForOunce([FromServices] IComodityService comodityService)
        {
            double exhangeRate = await comodityService.GetCurrentGoldPriceForOunce().ConfigureAwait(false);
            return TypedResults.Ok(exhangeRate);
        }

        /// <summary>
        /// Endpoint to get price for ounce of gold in specific currency
        /// </summary>
        /// <param name="comodityService">Comodity service</param>
        /// <param name="forexService">Forex service</param>
        /// <param name="currencyCode">Currency of price of gold</param>
        /// <returns>Price ounce of gold</returns>
        public static async Task<Results<Ok<double>, BadRequest<string>>> GetCurrentGoldPriceForOunceForSpecificCurrency([FromServices] IComodityService comodityService, [FromServices] IForexService forexService, string currencyCode)
        {
            double currencyExchangeRate = 1.0;

            if (string.Compare(currencyCode, GoldPriceCurrency, StringComparison.OrdinalIgnoreCase) != 0)
            {
                currencyExchangeRate = await forexService.GetExchangeRate(GoldPriceCurrency, currencyCode).ConfigureAwait(false);

                if (currencyExchangeRate == 0)
                    return TypedResults.BadRequest("Currency code is not valid");
            }

            double exhangeRate = await comodityService.GetCurrentGoldPriceForOunce().ConfigureAwait(false);
            return TypedResults.Ok(exhangeRate * currencyExchangeRate);
        }
    }
}
