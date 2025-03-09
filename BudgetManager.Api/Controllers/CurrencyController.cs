using System.Collections.Generic;
using BudgetManager.Data.DataModels;
using BudgetManager.Repository;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace BudgetManager.Api.Controllers
{
    [ApiController]
    [Route("currency")]
    public class CurrencyController : BaseController
    {
        private readonly IRepository<CurrencySymbol> currencySymbolRepository;

        public CurrencyController(IHttpContextAccessor httpContextAccessor, IRepository<CurrencySymbol> currencySymbolRepository) : base(httpContextAccessor)
        {
            this.currencySymbolRepository = currencySymbolRepository;
        }

        [Produces("application/json")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [HttpGet("all")]
        public ActionResult<IEnumerable<CurrencySymbol>> Get()
        {
            return Ok(currencySymbolRepository.FindAll());
        }
    }
}
