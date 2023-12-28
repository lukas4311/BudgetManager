using BudgetManager.FinancialApi.Enums;
using BudgetManager.Services.Contracts;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace BudgetManager.FinancialApi.Endpoints
{
    public static class ForexEndpoints
    {
        private const double SameCurrencyExchangeRate = 1.0;

        public static void RegisterForexEndpoints(this WebApplication app)
        {
            app.MapGet("/forex/exchangerate/{from}/{to}", GetForexPairPrice)
            .WithName(nameof(GetForexPairPrice))
            .WithOpenApi();

            app.MapGet("/forex/exchangerate/{from}/{to}/{date}", GetForexPairPriceAtDate)
            .WithName(nameof(GetForexPairPriceAtDate))
            .WithOpenApi();
        }

        public static async Task<Ok<double>> GetForexPairPrice([FromServices] IForexService forexService, [FromRoute] CurrencySymbol from, [FromRoute] CurrencySymbol to)
        {
            if (from == to)
                return TypedResults.Ok(SameCurrencyExchangeRate);

            var data = await forexService.GetExchangeRate(from.ToString(), to.ToString());

            return TypedResults.Ok(data);
        }

        public static async Task<Ok<double>> GetForexPairPriceAtDate([FromServices] IForexService forexService, [FromRoute] CurrencySymbol from, [FromRoute] CurrencySymbol to, [FromRoute] DateTime date)
        {
            if (from == to)
                return TypedResults.Ok(SameCurrencyExchangeRate);

            var data = await forexService.GetExchangeRate(from.ToString(), to.ToString(), date);

            return TypedResults.Ok(data);
        }
    }
}
