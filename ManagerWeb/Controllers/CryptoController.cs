using ManagerWeb.Services;
using Microsoft.AspNetCore.Mvc;

namespace ManagerWeb.Controllers
{
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
        public IActionResult Get()
        {
            return Ok(this.cryptoService.Get());
        }

        [HttpGet("get")]
        public IActionResult Get(int id)
        {
            return Ok(this.cryptoService.Get(id));
        }
    }
}
