using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using BudgetManager.Data.DataModels;
using BudgetManager.Domain.DTOs;
using BudgetManager.Domain.DTOs.Queries;
using BudgetManager.Repository;
using BudgetManager.Services;
using BudgetManager.Services.Contracts;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace BudgetManager.Api.Controllers
{
    [ApiController]
    [Route("cryptos")]
    public class CryptoController : BaseController
    {
        private readonly ICryptoService cryptoService;
        private readonly IForexService forexService;

        public CryptoController(IHttpContextAccessor httpContextAccessor, ICryptoService cryptoService, IForexService forexService) : base(httpContextAccessor)
        {
            this.cryptoService = cryptoService;
            this.forexService = forexService;
        }

        [Produces("application/json")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [HttpGet("all")]
        public ActionResult<IEnumerable<TradeHistory>> Get()
        {
            return Ok(cryptoService.GetByUser(GetUserId()));
        }

        [Produces("application/json")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [HttpPost]
        public IActionResult Add([FromBody] TradeHistory tradeHistory)
        {
            tradeHistory.UserIdentityId = GetUserId();
            cryptoService.Add(tradeHistory);
            return Ok();
        }

        [Produces("application/json")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [HttpPut]
        public IActionResult Update([FromBody] TradeHistory tradeHistory)
        {
            tradeHistory.UserIdentityId = GetUserId();
            cryptoService.Update(tradeHistory);
            return Ok();
        }

        [Produces("application/json")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [HttpDelete]
        public IActionResult Delete([FromBody] int id)
        {
            if (!cryptoService.UserHasRightToCryptoTrade(id, GetUserId()))
                return StatusCode(StatusCodes.Status401Unauthorized);

            cryptoService.Delete(id);
            return Ok();
        }

        [Produces("application/json")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [HttpGet("actualExchangeRate/{fromCurrency}/{toCurrency}")]
        public async Task<ActionResult<double>> GetCurrentExchangeRate(string fromCurrency, string toCurrency)
        {
            double exhangeRate = await forexService.GetCurrentExchangeRate(fromCurrency, toCurrency).ConfigureAwait(false);

            if (exhangeRate == 0)
                exhangeRate = await cryptoService.GetCurrentExchangeRate(fromCurrency, toCurrency).ConfigureAwait(false);

            return Ok(exhangeRate);
        }

        [Produces("application/json")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [HttpGet("exchangeRate/{fromCurrency}/{toCurrency}/{atDate}")]
        public async Task<ActionResult<double>> GetCurrentExchangeRate(string fromCurrency, string toCurrency, DateTime atDate)
        {
            double exhangeRate = await cryptoService.GetCurrentExchangeRate(fromCurrency, toCurrency, atDate).ConfigureAwait(false);
            return Ok(exhangeRate);
        }

        [Produces("application/json")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [HttpGet("tradeDetail/{tradeId}")]
        public ActionResult<TradeHistory> Get(int tradeId)
        {
            return Ok(cryptoService.Get(tradeId, GetUserId()));
        }

        [Produces("application/json")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [HttpGet("tickers")]
        public ActionResult<IEnumerable<CryptoTickerModel>> GetTickers() => Ok(cryptoService.GetAllTickers());

        [Produces("application/json")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [HttpPost("brokerReport/{brokerId}")]
        public async Task<IActionResult> UploadReport([FromRoute]int brokerId, IFormFile file)
        {
            using MemoryStream ms = new MemoryStream();
            await file.CopyToAsync(ms);
            byte[] fileBytes = ms.ToArray();
            cryptoService.StoreReportToProcess(fileBytes, GetUserId(), brokerId);

            return Ok();
        }

        [Produces("application/json")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [HttpGet("trade/monthlygrouped")]
        public ActionResult<IEnumerable<TradesGroupedMonth>> GetGroupedTradesByMonth()
        {
            var data = cryptoService.GetAllTradesGroupedByMonth(GetUserId());
            return Ok(data);
        }

        [Produces("application/json")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [HttpGet("trade/tradedategrouped")]
        public ActionResult<IEnumerable<TradesGroupedMonth>> GetGroupedByTickerAndTradeDate()
        {
            var data = cryptoService.GetAllTradesGroupedByTradeDate(GetUserId());
            return Ok(data);
        }

        [Produces("application/json")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [HttpGet("trade/tickergrouped")]
        public ActionResult<IEnumerable<TradesGroupedMonth>> GetGroupedByTicker()
        {
            var data = cryptoService.GetAllTradesGroupedByTicker(GetUserId());
            return Ok(data);
        }
    }
}
