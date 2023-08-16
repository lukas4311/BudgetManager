using BudgetManager.InfluxDbData.Models;
using BudgetManager.Services.Contracts;
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
        }

        public static async Task<IResult> GetStockPriceData([FromServices] IStockTradeHistoryService stockTradeHistoryService, [FromServices] IStockTickerService stockTickerService, [FromRoute] string ticker)
        {
            if (stockTickerService.GetAll().Count(t => string.Compare(t.Ticker, ticker, true) == 0) == 0)
                return Results.NoContent();

            IEnumerable<StockPrice> data = await stockTradeHistoryService.GetStockPriceHistory(ticker);
            return Results.Ok(data);
        }

        public static async Task<IResult> GetStockPriceDataFromDate([FromServices] IStockTradeHistoryService stockTradeHistoryService, [FromServices] IStockTickerService stockTickerService, 
            [FromRoute] string ticker, [FromRoute] DateTime from)
        {
            if (stockTickerService.GetAll().Count(t => string.Compare(t.Ticker, ticker, true) == 0) == 0)
                return Results.NoContent();

            IEnumerable<StockPrice> data = await stockTradeHistoryService.GetStockPriceHistory(ticker, from);
            return Results.Ok(data);
        }

        public static async Task<IResult> GetStockPriceDataAtDate([FromServices] IStockTradeHistoryService stockTradeHistoryService, [FromServices] IStockTickerService stockTickerService,
            [FromRoute] string ticker, [FromRoute] DateTime date)
        {
            if (stockTickerService.GetAll().Count(t => string.Compare(t.Ticker, ticker, true) == 0) == 0)
                return Results.NoContent();

            StockPrice data = await stockTradeHistoryService.GetStockPriceAtDate(ticker, date);
            return Results.Ok(data);
        }
    }
}
