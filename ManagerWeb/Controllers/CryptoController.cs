using ManagerWeb.Models.DTOs;
using ManagerWeb.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;

namespace ManagerWeb.Controllers
{
    [Authorize]
    [ApiController]
    [Route("crypto")]
    public class CryptoController : ControllerBase
    {
        private readonly ICryptoService cryptoService;

        public CryptoController(ICryptoService cryptoService)
        {
            this.cryptoService = cryptoService;
        }

        [HttpGet("getAll")]
        public ActionResult<IEnumerable<TradeHistory>> Get()
        {
            return Ok(this.cryptoService.Get());
        }

        [HttpGet("getExchangeRate/{fromCurrency}/{toCurrency}")]
        public ActionResult GetCurrentExchangeRate(string fromCurrency, string toCurrency)
        {
            // TODO: watch influx currency bucket if ticker [fromCurrency/toCurrency] exists there
            // if not then watch bucket of crypto if ticker [fromCurrency/toCurrency] exists there
            // then return last evidated exchange rate
            // if not throw exception

            return Ok();
        }

        [HttpGet("get")]
        public ActionResult<TradeHistory> Get(int id)
        {
            return Ok(this.cryptoService.Get(id));
        }
    }
}
