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
                .Produces(StatusCodes.Status200OK);

            app.MapGet("gold/actualPrice/{currencyCode}", GetCurrentGoldPriceForOunceForSpecificCurrency)
                .WithName(nameof(GetCurrentGoldPriceForOunceForSpecificCurrency))
                .WithOpenApi()
                .Produces(StatusCodes.Status200OK)
                .Produces(StatusCodes.Status204NoContent);
        }

        public static async Task<Ok<double>> GetCurrentGoldPriceForOunce([FromServices] IComodityService comodityService)
        {
            double exhangeRate = await comodityService.GetCurrentGoldPriceForOunce().ConfigureAwait(false);
            return TypedResults.Ok(exhangeRate);
        }

        public static async Task<Ok<double>> GetCurrentGoldPriceForOunceForSpecificCurrency([FromServices] IComodityService comodityService, [FromServices] IForexService forexService, string currencyCode)
        {
            double currencyExchangeRate = 1.0;

            if (string.Compare(currencyCode, GoldPriceCurrency, true) != 0)
            {
                currencyExchangeRate = await forexService.GetExchangeRate(GoldPriceCurrency, currencyCode).ConfigureAwait(false);

                if (currencyExchangeRate == 0)
                    throw new ArgumentException("Currency code is not valid");
            }

            double exhangeRate = await comodityService.GetCurrentGoldPriceForOunce().ConfigureAwait(false);
            return TypedResults.Ok(exhangeRate * currencyExchangeRate);
        }
    }
}
