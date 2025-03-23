using System.Net.Mime;
using BudgetManager.InfluxDbData;
using BudgetManager.Services.Contracts;
using Microsoft.AspNetCore.Http;
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
            .WithOpenApi()
            .Produces(StatusCodes.Status200OK,contentType: "application/json")
            .Produces(StatusCodes.Status204NoContent, contentType: "application/problem+json");

            app.MapGet("/crypto/{ticker}/priceFrom/{from}", GetCryptoPriceDataFromDate)
            .WithName(nameof(GetCryptoPriceDataFromDate))
            .WithOpenApi()
            .Produces(StatusCodes.Status200OK, contentType: "application/json")
            .Produces(StatusCodes.Status204NoContent, contentType: "application/problem+json");

            app.MapGet("/crypto/{ticker}/price/{date}", GetCryptoPriceDataAtDate)
            .WithName(nameof(GetCryptoPriceDataAtDate))
            .WithOpenApi()
            .Produces(StatusCodes.Status200OK, contentType: "application/json")
            .Produces(StatusCodes.Status204NoContent, contentType: "application/problem+json");
        }

        /// <summary>
        /// Endpoint to get history price of crypto currency
        /// </summary>
        /// <param name="cryptoService">Crypto service</param>
        /// <param name="ticker">Ticker of crypto currency</param>
        /// <returns>Price history</returns>
        public static async Task<Results<Ok<IEnumerable<CryptoDataV2>>, NoContent>> GetCryptoPriceData([FromServices] ICryptoService cryptoService, [FromRoute] string ticker)
        {
            IEnumerable<CryptoDataV2> data = await cryptoService.GetCryptoPriceHistory(ticker);

            if (data is null)
                return TypedResults.NoContent();

            return TypedResults.Ok(data);
        }

        /// <summary>
        /// Endpoint to get history price of crypto currency from specific date
        /// </summary>
        /// <param name="cryptoService">Crypto service</param>
        /// <param name="ticker">Ticker of crypto currency</param>
        /// <param name="from">From date for price history</param>
        /// <returns>Price history from specific date</returns>
        public static async Task<Results<Ok<IEnumerable<CryptoDataV2>>, NoContent>> GetCryptoPriceDataFromDate([FromServices] ICryptoService cryptoService,
            [FromRoute] string ticker, [FromRoute] DateTime from)
        {
            IEnumerable<CryptoDataV2> data = await cryptoService.GetCryptoPriceHistory(ticker, from);

            if (data is null)
                return TypedResults.NoContent();

            return TypedResults.Ok(data);
        }

        /// <summary>
        /// Endpoint to get price of crypto currency at specific date
        /// </summary>
        /// <param name="cryptoService">Crypto service</param>
        /// <param name="ticker">Ticker of crypto currency</param>
        /// <param name="date">Date of price</param>
        /// <returns>Price of crypto currency at specific date</returns>
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