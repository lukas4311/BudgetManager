using BudgetManager.InfluxDbData;
using BudgetManager.Services.Contracts;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace BudgetManager.FinancialApi.Endpoints
{
    public static class CryptoEndpoints
    {
        public static void RegisterCryptoEndpoints(this WebApplication app)
        {
            app.MapGet("/crypto/{ticker}/price/all", GetCryptoPriceData)
            .WithName(nameof(GetCryptoPriceData))
            .WithOpenApi();

            app.MapGet("/crypto/{ticker}/priceFrom/{from}", GetCryptoPriceDataFromDate)
            .WithName(nameof(GetCryptoPriceDataFromDate))
            .WithOpenApi();

            app.MapGet("/crypto/{ticker}/price/{date}", GetCryptoPriceDataAtDate)
            .WithName(nameof(GetCryptoPriceDataAtDate))
            .WithOpenApi();
        }

        public static async Task<Results<Ok<IEnumerable<CryptoDataV2>>, NoContent>> GetCryptoPriceData([FromServices] ICryptoService cryptoService, [FromRoute] string ticker)
        {
            IEnumerable<CryptoDataV2> data = await cryptoService.GetCryptoPriceHistory(ticker);

            if (data is null)
                return TypedResults.NoContent();

            return TypedResults.Ok(data);
        }

        public static async Task<Results<Ok<IEnumerable<CryptoDataV2>>, NoContent>> GetCryptoPriceDataFromDate([FromServices] ICryptoService cryptoService,
            [FromRoute] string ticker, [FromRoute] DateTime from)
        {
            IEnumerable<CryptoDataV2> data = await cryptoService.GetCryptoPriceHistory(ticker, from);

            if (data is null)
                return TypedResults.NoContent();

            return TypedResults.Ok(data);
        }

        public static async Task<Results<Ok<CryptoDataV2>, NoContent>> GetCryptoPriceDataAtDate([FromServices] ICryptoService cryptoService,
            [FromRoute] string ticker, [FromRoute] DateTime date)
        {
            CryptoDataV2 data = await cryptoService.GetCryptoPriceAtDate(ticker, date);

            if (data is null)
                return TypedResults.NoContent();

            return TypedResults.Ok(data);
        }
    }
}