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

        public static async Task<Ok<IEnumerable<CryptoDataV2>>> GetCryptoPriceData([FromServices] ICryptoService cryptoService, [FromRoute] string ticker)
        {
            IEnumerable<CryptoDataV2> data = await cryptoService.GetCryptoPriceHistory(ticker);
            return TypedResults.Ok(data);
        }

        public static async Task<Ok<IEnumerable<CryptoDataV2>>> GetCryptoPriceDataFromDate([FromServices] ICryptoService cryptoService, 
            [FromRoute] string ticker, [FromRoute] DateTime from)
        {
            IEnumerable<CryptoDataV2> data = await cryptoService.GetCryptoPriceHistory(ticker, from);
            return TypedResults.Ok(data);
        }

        public static async Task<Ok<CryptoDataV2>> GetCryptoPriceDataAtDate([FromServices] ICryptoService cryptoService,
            [FromRoute] string ticker, [FromRoute] DateTime date)
        {
            CryptoDataV2 data = await cryptoService.GetCryptoPriceAtDate(ticker, date);
            return TypedResults.Ok(data);
        }
    }
}