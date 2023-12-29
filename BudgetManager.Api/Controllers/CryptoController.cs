using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using BudgetManager.Data.DataModels;
using BudgetManager.Domain.DTOs;
using BudgetManager.Repository;
using BudgetManager.Services.Contracts;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BudgetManager.Api.Controllers
{
    [ApiController]
    [Route("cryptos")]
    public class CryptoController : BaseController
    {
        private readonly ICryptoService cryptoService;
        private readonly IForexService forexService;
        private readonly ICryptoTickerRepository cryptoTickerRepository;

        public CryptoController(IHttpContextAccessor httpContextAccessor, ICryptoService cryptoService, IForexService forexService, ICryptoTickerRepository cryptoTickerRepository) : base(httpContextAccessor)
        {
            this.cryptoService = cryptoService;
            this.forexService = forexService;
            this.cryptoTickerRepository = cryptoTickerRepository;
        }

        [HttpGet("all")]
        public ActionResult<IEnumerable<TradeHistory>> Get()
        {
            return Ok(this.cryptoService.GetByUser(this.GetUserId()));
        }

        [HttpPost]
        public IActionResult Add([FromBody] TradeHistory tradeHistory)
        {
            tradeHistory.UserIdentityId = this.GetUserId();
            this.cryptoService.Add(tradeHistory);
            return Ok();
        }

        [HttpPut]
        public IActionResult Update([FromBody] TradeHistory tradeHistory)
        {
            tradeHistory.UserIdentityId = this.GetUserId();
            this.cryptoService.Update(tradeHistory);
            return Ok();
        }

        [HttpDelete]
        public IActionResult Delete([FromBody] int id)
        {
            if (!this.cryptoService.UserHasRightToCryptoTrade(id, this.GetUserId()))
                return StatusCode(StatusCodes.Status401Unauthorized);

            this.cryptoService.Delete(id);
            return Ok();
        }

        [HttpGet("actualExchangeRate/{fromCurrency}/{toCurrency}")]
        public async Task<ActionResult<double>> GetCurrentExchangeRate(string fromCurrency, string toCurrency)
        {
            double exhangeRate = await this.forexService.GetCurrentExchangeRate(fromCurrency, toCurrency).ConfigureAwait(false);

            if (exhangeRate == 0)
                exhangeRate = await this.cryptoService.GetCurrentExchangeRate(fromCurrency, toCurrency).ConfigureAwait(false);

            return Ok(exhangeRate);
        }
        
        [HttpGet("exchangeRate/{fromCurrency}/{toCurrency}/{atDate}")]
        public async Task<ActionResult<double>> GetCurrentExchangeRate(string fromCurrency, string toCurrency, DateTime atDate)
        {
            double exhangeRate = await this.cryptoService.GetCurrentExchangeRate(fromCurrency, toCurrency, atDate).ConfigureAwait(false);
            return Ok(exhangeRate);
        } 

        [HttpGet("tradeDetail/{tradeId}")]
        public ActionResult<TradeHistory> Get(int id)
        {
            return Ok(this.cryptoService.Get(id, this.GetUserId()));
        }

        [HttpGet("tickers")]
        public ActionResult<IEnumerable<CryptoTicker>> GetTickers() => Ok(this.cryptoTickerRepository.FindAll());

        [HttpPost("brokerReport")]
        public async Task<IActionResult> Post(IFormFile file)
        {
            using (MemoryStream ms = new MemoryStream())
            {
                await file.CopyToAsync(ms);
                byte[] fileBytes = ms.ToArray();
                string fileContentBase64 = Convert.ToBase64String(fileBytes);

                // TODO: create EF entity and store report to process
            }

            return Ok();
        }
    }
}
