using BudgetManager.FinancialApi.Enums;
using BudgetManager.Services.Contracts;
using Microsoft.AspNetCore.Mvc;

namespace BudgetManager.FinancialApi.Endpoints
{
    public static class ForexEndpoints
    {
        private const int SameCurrencyExchangeRate = 1;

        public static void RegisterForexEndpoints(this WebApplication app)
        {
            app.MapGet("/forex/exchangerate/{from}/{to}", GetForexPairPrice)
            .WithName("GetForexPairPrice")
            .WithOpenApi();
        }

        public static async Task<IResult> GetForexPairPrice([FromServices] IForexService forexService, [FromRoute] CurrencySymbol from, [FromRoute] CurrencySymbol to)
        {
            if (from == to)
                return Results.Ok(SameCurrencyExchangeRate);

            var data = await forexService.GetExchangeRate(from.ToString(), to.ToString());
            return Results.Ok(data);
        }
    }
}
