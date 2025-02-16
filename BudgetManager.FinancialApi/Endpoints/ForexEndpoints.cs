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
            .WithOpenApi()
            .Produces(StatusCodes.Status200OK, contentType: "application/json");

            app.MapGet("/forex/exchangerate/{from}/{to}/{date}", GetForexPairPriceAtDate)
            .WithName(nameof(GetForexPairPriceAtDate))
            .WithOpenApi()
            .Produces(StatusCodes.Status200OK, contentType: "application/json");
        }

        /// <summary>
        /// Endpoint to get forex price of pair
        /// </summary>
        /// <param name="forexService">Forex service</param>
        /// <param name="from">From forex symbol</param>
        /// <param name="to">To forex symbol</param>
        /// <returns>Price of forex pair</returns>
        public static async Task<Ok<double>> GetForexPairPrice([FromServices] IForexService forexService, [FromRoute] CurrencySymbol from, [FromRoute] CurrencySymbol to)
        {
            if (from == to)
                return TypedResults.Ok(SameCurrencyExchangeRate);

            var data = await forexService.GetExchangeRate(from.ToString(), to.ToString());

            return TypedResults.Ok(data);
        }

        /// <summary>
        /// Endpoint to get forex price of pair at date
        /// </summary>
        /// <param name="forexService">Forex service</param>
        /// <param name="from">From forex symbol</param>
        /// <param name="to">To forex symbol</param>
        /// <param name="date">Specific price date</param>
        /// <returns>Price of forex pair</returns>
        public static async Task<Ok<double>> GetForexPairPriceAtDate([FromServices] IForexService forexService, [FromRoute] CurrencySymbol from, [FromRoute] CurrencySymbol to, [FromRoute] DateTime date)
        {
            if (from == to)
                return TypedResults.Ok(SameCurrencyExchangeRate);

            var data = await forexService.GetExchangeRate(from.ToString(), to.ToString(), date);

            return TypedResults.Ok(data);
        }
    }
}
