using BudgetManager.InfluxDbData.Models;
using BudgetManager.Services.Contracts;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace BudgetManager.FinancialApi.Endpoints
{
    public static class StockEndpoints
    {
        public static void RegisterStockEndpoints(this WebApplication app)
        {
            app.MapGet("/stock/{ticker}/price/all", GetStockPriceData)
                .WithName(nameof(GetStockPriceData))
                .WithOpenApi()
                .Produces(StatusCodes.Status200OK, contentType: "application/json")
                .Produces(StatusCodes.Status204NoContent, contentType: "application/problem+json")
                .Produces(StatusCodes.Status404NotFound, contentType: "application/problem+json")
                .Produces(StatusCodes.Status500InternalServerError, contentType: "application/problem+json");

            app.MapGet("stock/{ticker}/priceFrom/{from}", GetStockPriceDataFromDate)
                .WithName(nameof(GetStockPriceDataFromDate))
                .WithOpenApi()
                .Produces(StatusCodes.Status200OK, contentType: "application/json")
                .Produces(StatusCodes.Status204NoContent, contentType: "application/problem+json")
                .Produces(StatusCodes.Status404NotFound, contentType: "application/problem+json")
                .Produces(StatusCodes.Status500InternalServerError, contentType: "application/problem+json");

            app.MapGet("stock/{ticker}/price/{date}", GetStockPriceDataAtDate)
                .WithName(nameof(GetStockPriceDataAtDate))
                .WithOpenApi()
                .Produces(StatusCodes.Status200OK, contentType: "application/json")
                .Produces(StatusCodes.Status204NoContent, contentType: "application/problem+json")
                .Produces(StatusCodes.Status404NotFound, contentType: "application/problem+json")
                .Produces(StatusCodes.Status500InternalServerError, contentType: "application/problem+json");

            app.MapGet("stocks/price/{date}", GetStocksPriceDataAtDate)
                .WithName(nameof(GetStocksPriceDataAtDate))
                .WithOpenApi()
                .Produces(StatusCodes.Status200OK, contentType: "application/json")
                .Produces(StatusCodes.Status204NoContent, contentType: "application/problem+json")
                .Produces(StatusCodes.Status500InternalServerError, contentType: "application/problem+json");
        }

        /// <summary>
        /// Endpoint to get stock history of price
        /// </summary>
        /// <param name="stockTradeHistoryService">Stock price service</param>
        /// <param name="stockTickerService">Stock ticker service</param>
        /// <param name="ticker">Stock ticker</param>
        /// <returns>Price history</returns>
        public static async Task<Results<Ok<IEnumerable<StockPrice>>, NotFound, NoContent>> GetStockPriceData([FromServices] IStockTradeHistoryService stockTradeHistoryService, [FromServices] IStockTickerService stockTickerService, [FromRoute] string ticker)
        {
            var tickers = stockTickerService.GetAllAvailableTickersForPriceSearch();

            if (tickers.Count(t => string.Compare(t, ticker, StringComparison.OrdinalIgnoreCase) == 0) == 0)
                return TypedResults.NotFound();

            IEnumerable<StockPrice> data = await stockTradeHistoryService.GetStockPriceHistory(ticker);

            if (data is null)
                return TypedResults.NoContent();

            return TypedResults.Ok(data);
        }

        /// <summary>
        /// Endpoint to get stock history of price from specific date
        /// </summary>
        /// <param name="stockTradeHistoryService">Stock price service</param>
        /// <param name="stockTickerService">Stock ticker service</param>
        /// <param name="ticker">Stock ticker</param>
        /// <param name="from">Start price history</param>
        /// <returns>Price history</returns>
        public static async Task<Results<Ok<IEnumerable<StockPrice>>, NotFound, NoContent>> GetStockPriceDataFromDate([FromServices] IStockTradeHistoryService stockTradeHistoryService, [FromServices] IStockTickerService stockTickerService,
            [FromRoute] string ticker, [FromRoute] DateTime from)
        {
            var tickers = stockTickerService.GetAllAvailableTickersForPriceSearch();

            if (tickers.Count(t => string.Compare(t, ticker, StringComparison.OrdinalIgnoreCase) == 0) == 0)
                return TypedResults.NotFound();

            IEnumerable<StockPrice> data = await stockTradeHistoryService.GetStockPriceHistory(ticker, from);

            if (data is null)
                return TypedResults.NoContent();

            return TypedResults.Ok(data);
        }

        /// <summary>
        /// Endpoint to get stock price in specific date
        /// </summary>
        /// <param name="stockTradeHistoryService">Stock price service</param>
        /// <param name="stockTickerService">Stock ticker service</param>
        /// <param name="ticker">Stock ticker</param>
        /// <param name="date">Specific price date</param>
        /// <returns>Price of stock in specific date</returns>
        public static async Task<Results<Ok<StockPrice>, NotFound, NoContent>> GetStockPriceDataAtDate([FromServices] IStockTradeHistoryService stockTradeHistoryService, [FromServices] IStockTickerService stockTickerService,
            [FromRoute] string ticker, [FromRoute] DateTime date)
        {
            var tickers = stockTickerService.GetAllAvailableTickersForPriceSearch();

            if (tickers.Count(t => string.Compare(t, ticker, StringComparison.OrdinalIgnoreCase) == 0) == 0)
                return TypedResults.NotFound();

            StockPrice data = await stockTradeHistoryService.GetStockPriceAtDate(ticker, date);

            if (data is null)
                return TypedResults.NoContent();

            return TypedResults.Ok(data);
        }

        /// <summary>
        /// Endpoint to get stocks price at specific date
        /// </summary>
        /// <param name="stockTradeHistoryService">Stock price service</param>
        /// <param name="stockTickerService">Stock ticker service</param>
        /// <param name="tickers">Stock tickers</param>
        /// <param name="date">Specific price date</param>
        /// <returns>Price of stocks in specific date</returns>
        public static async Task<Results<Ok<IEnumerable<StockPrice>>, NoContent>> GetStocksPriceDataAtDate([FromServices] IStockTradeHistoryService stockTradeHistoryService, [FromServices] IStockTickerService stockTickerService,
            [FromQuery] string[] tickers, [FromRoute] DateTime date)
        {
            IEnumerable<StockPrice> stockPrices = await stockTradeHistoryService.GetStocksPriceAtDate(tickers, date);

            if (stockPrices is null)
                return TypedResults.NoContent();

            return TypedResults.Ok(stockPrices);
        }
    }
}
