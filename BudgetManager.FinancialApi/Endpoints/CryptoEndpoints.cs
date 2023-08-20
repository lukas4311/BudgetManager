using BudgetManager.InfluxDbData;
using BudgetManager.InfluxDbData.Models;
using BudgetManager.Services.Contracts;
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

        public static async Task<IResult> GetCryptoPriceData([FromServices] ICryptoService cryptoService, [FromRoute] string ticker)
        {
            IEnumerable<CryptoDataV2> data = await cryptoService.GetCryptoPriceHistory(ticker);
            return Results.Ok(data);
        }

        public static async Task<IResult> GetCryptoPriceDataFromDate([FromServices] ICryptoService cryptoService, 
            [FromRoute] string ticker, [FromRoute] DateTime from)
        {
            IEnumerable<CryptoDataV2> data = await cryptoService.GetCryptoPriceHistory(ticker, from);
            return Results.Ok(data);
        }

        public static async Task<IResult> GetCryptoPriceDataAtDate([FromServices] ICryptoService cryptoService,
            [FromRoute] string ticker, [FromRoute] DateTime date)
        {
            CryptoDataV2 data = await cryptoService.GetCryptoPriceAtDate(ticker, date);
            return Results.Ok(data);
        }
    }
}