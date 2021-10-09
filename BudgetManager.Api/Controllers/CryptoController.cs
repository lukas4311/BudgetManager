using System.Collections.Generic;
using System.Threading.Tasks;
using BudgetManager.Domain.DTOs;
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

        [HttpGet("all")]
        public ActionResult<IEnumerable<TradeHistory>> Get()
        {
            return Ok(this.cryptoService.GetByUser(this.GetUserId()));
        }

        [HttpGet("actualExchangeRate/{fromCurrency}/{toCurrency}")]
        public async Task<ActionResult<double>> GetCurrentExchangeRate(string fromCurrency, string toCurrency)
        {
            double exhangeRate = await this.forexService.GetCurrentExchangeRate(fromCurrency, toCurrency).ConfigureAwait(false);

            if (exhangeRate == 0)
                exhangeRate = await this.cryptoService.GetCurrentExchangeRate(fromCurrency, toCurrency).ConfigureAwait(false);

            return Ok(exhangeRate);
        }

        [HttpGet("tradeDetail/{tradeId}")]
        public ActionResult<TradeHistory> Get(int id)
        {
            return Ok(this.cryptoService.Get(id, this.GetUserId()));
        }
    }
}
