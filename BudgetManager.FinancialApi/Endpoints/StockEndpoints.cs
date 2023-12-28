using BudgetManager.InfluxDbData.Models;
using BudgetManager.Services.Contracts;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace BudgetManager.FinancialApi.Endpoints
{
    public static class StockEndpoints
    {
        public static void RegisterStockEndpoints(this WebApplication app)
        {
            app.MapGet("/stock/{ticker}/price/all", GetStockPriceData)
            .WithName(nameof(GetStockPriceData))
            .WithOpenApi();

            app.MapGet("stock/{ticker}/priceFrom/{from}", GetStockPriceDataFromDate)
            .WithName(nameof(GetStockPriceDataFromDate))
            .WithOpenApi();

            app.MapGet("stock/{ticker}/price/{date}", GetStockPriceDataAtDate)
            .WithName(nameof(GetStockPriceDataAtDate))
            .WithOpenApi();

            app.MapGet("stocks/price/{date}", GetStocksPriceDataAtDate)
                .WithName(nameof(GetStocksPriceDataAtDate))
                .WithOpenApi();
        }

        public static async Task<Results<Ok<IEnumerable<StockPrice>>, NotFound, NoContent>> GetStockPriceData([FromServices] IStockTradeHistoryService stockTradeHistoryService, [FromServices] IStockTickerService stockTickerService, [FromRoute] string ticker)
        {
            var empty = Array.Empty<StockPrice>().AsEnumerable();
            if (stockTickerService.GetAll().Count(t => string.Compare(t.Ticker, ticker, true) == 0) == 0)
                return TypedResults.NotFound();

            IEnumerable<StockPrice> data = await stockTradeHistoryService.GetStockPriceHistory(ticker);

            if (data is null)
                return TypedResults.NoContent();

            return TypedResults.Ok(data);
        }

        public static async Task<Results<Ok<IEnumerable<StockPrice>>, NotFound, NoContent>> GetStockPriceDataFromDate([FromServices] IStockTradeHistoryService stockTradeHistoryService, [FromServices] IStockTickerService stockTickerService,
            [FromRoute] string ticker, [FromRoute] DateTime from)
        {
            if (stockTickerService.GetAll().Count(t => string.Compare(t.Ticker, ticker, true) == 0) == 0)
                return TypedResults.NotFound();

            IEnumerable<StockPrice> data = await stockTradeHistoryService.GetStockPriceHistory(ticker, from);

            if (data is null)
                return TypedResults.NoContent();

            return TypedResults.Ok(data);
        }

        public static async Task<Results<Ok<StockPrice>, NotFound, NoContent>> GetStockPriceDataAtDate([FromServices] IStockTradeHistoryService stockTradeHistoryService, [FromServices] IStockTickerService stockTickerService,
            [FromRoute] string ticker, [FromRoute] DateTime date)
        {
            if (stockTickerService.GetAll().Count(t => string.Compare(t.Ticker, ticker, true) == 0) == 0)
                return TypedResults.NotFound();

            StockPrice data = await stockTradeHistoryService.GetStockPriceAtDate(ticker, date);

            if (data is null)
                return TypedResults.NoContent();

            return TypedResults.Ok(data);
        }

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
