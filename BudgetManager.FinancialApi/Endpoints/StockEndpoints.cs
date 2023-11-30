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
            .WithOpenApi();

            app.MapGet("stock/{ticker}/priceFrom/{from}", GetStockPriceDataFromDate)
            .WithName(nameof(GetStockPriceDataFromDate))
            .WithOpenApi();

            app.MapGet("stock/{ticker}/price/{date}", GetStockPriceDataAtDate)
            .WithName(nameof(GetStockPriceDataAtDate))
            .WithOpenApi();

            app.MapGet("stock/{tickers}/price/{date}", GetStocksPriceDataAtDate)
                .WithName(nameof(GetStocksPriceDataAtDate))
                .WithOpenApi();
        }

        public static async Task<Results<Ok<IEnumerable<StockPrice>>, NotFound>> GetStockPriceData([FromServices] IStockTradeHistoryService stockTradeHistoryService, [FromServices] IStockTickerService stockTickerService, [FromRoute] string ticker)
        {
            var empty = Array.Empty<StockPrice>().AsEnumerable();
            if (stockTickerService.GetAll().Count(t => string.Compare(t.Ticker, ticker, true) == 0) == 0)
                return TypedResults.NotFound();

            IEnumerable<StockPrice> data = await stockTradeHistoryService.GetStockPriceHistory(ticker);
            return TypedResults.Ok(data);
        }

        public static async Task<Results<Ok<IEnumerable<StockPrice>>, NotFound>> GetStockPriceDataFromDate([FromServices] IStockTradeHistoryService stockTradeHistoryService, [FromServices] IStockTickerService stockTickerService,
            [FromRoute] string ticker, [FromRoute] DateTime from)
        {
            if (stockTickerService.GetAll().Count(t => string.Compare(t.Ticker, ticker, true) == 0) == 0)
                return TypedResults.NotFound();

            IEnumerable<StockPrice> data = await stockTradeHistoryService.GetStockPriceHistory(ticker, from);
            return TypedResults.Ok(data);
        }

        public static async Task<Results<Ok<StockPrice>, NotFound>> GetStockPriceDataAtDate([FromServices] IStockTradeHistoryService stockTradeHistoryService, [FromServices] IStockTickerService stockTickerService,
            [FromRoute] string ticker, [FromRoute] DateTime date)
        {
            if (stockTickerService.GetAll().Count(t => string.Compare(t.Ticker, ticker, true) == 0) == 0)
                return TypedResults.NotFound();

            StockPrice data = await stockTradeHistoryService.GetStockPriceAtDate(ticker, date);
            return TypedResults.Ok(data);
        }

        public static async Task<Ok<IEnumerable<StockPrice>>> GetStocksPriceDataAtDate([FromServices] IStockTradeHistoryService stockTradeHistoryService, [FromServices] IStockTickerService stockTickerService,
            [FromRoute] string[] tickers, [FromRoute] DateTime date)
        {
            IEnumerable<StockPrice> stockPrices = new List<StockPrice>();
            stockTradeHistoryService.GetStocksPriceAtDate(tickers, date);
            return TypedResults.Ok(stockPrices);
        }
    }
}
