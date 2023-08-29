using BudgetManager.Services;
using BudgetManager.Services.Contracts;
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
            .WithOpenApi();

            app.MapGet("gold/actualPrice/{currencyCode}", GetCurrentGoldPriceForOunce)
            .WithName(nameof(GetCurrentGoldPriceForOunce))
            .WithOpenApi();

            app.MapGet("stock/{ticker}/price/{date}", GetCurrentGoldPriceForOunceForSpecificCurrency)
            .WithName(nameof(GetCurrentGoldPriceForOunceForSpecificCurrency))
            .WithOpenApi();
        }

        public static async Task<IResult> GetCurrentGoldPriceForOunce([FromServices] IComodityService comodityService)
        {
            double exhangeRate = await comodityService.GetCurrentGoldPriceForOunce().ConfigureAwait(false);
            return Results.Ok(exhangeRate);
        }

        public static async Task<IResult> GetCurrentGoldPriceForOunceForSpecificCurrency([FromServices] IComodityService comodityService, [FromServices] IForexService forexService, string currencyCode)
        {
            double currencyExchangeRate = 1.0;

            if (string.Compare(currencyCode, GoldPriceCurrency, true) != 0)
            {
                currencyExchangeRate = await forexService.GetCurrentExchangeRate(GoldPriceCurrency, currencyCode).ConfigureAwait(false);

                if (currencyExchangeRate == 0)
                    throw new ArgumentException("Currency code is not valid");
            }

            double exhangeRate = await comodityService.GetCurrentGoldPriceForOunce().ConfigureAwait(false);
            return Results.Ok(exhangeRate * currencyExchangeRate);
        }
    }
}
