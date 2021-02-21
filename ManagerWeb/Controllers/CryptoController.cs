using InfluxDbData;
using ManagerWeb.Models.DTOs;
using ManagerWeb.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ManagerWeb.Controllers
{
    [Authorize]
    [ApiController]
    [Route("crypto")]
    public class CryptoController : ControllerBase
    {
        private readonly ICryptoService cryptoService;
        private readonly IForexService forexService;

        public CryptoController(ICryptoService cryptoService, IForexService forexService)
        {
            this.cryptoService = cryptoService;
            this.forexService = forexService;
        }

        [HttpGet("getAll")]
        public ActionResult<IEnumerable<TradeHistory>> Get()
        {
            return Ok(this.cryptoService.Get());
        }

        [HttpGet("getExchangeRate/{fromCurrency}/{toCurrency}")]
        public async Task<ActionResult<double>> GetCurrentExchangeRate(string fromCurrency, string toCurrency)
        {
            double exhangeRate = await this.forexService.GetCurrentExchangeRate(fromCurrency, toCurrency).ConfigureAwait(false);

            if (exhangeRate == 0)
                exhangeRate = await this.cryptoService.GetCurrentExchangeRate(fromCurrency, toCurrency).ConfigureAwait(false);

            return Ok(exhangeRate);
        }

        [HttpGet("get")]
        public ActionResult<TradeHistory> Get(int id)
        {
            return Ok(this.cryptoService.Get(id));
        }
    }
}
