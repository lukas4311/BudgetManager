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
        private readonly ICurrencySymbolRepository currencySymbolRepository;

        public CurrencyController(IHttpContextAccessor httpContextAccessor, ICurrencySymbolRepository currencySymbolRepository) : base(httpContextAccessor)
        {
            this.currencySymbolRepository = currencySymbolRepository;
        }

        [HttpGet("all")]
        public ActionResult<IEnumerable<CurrencySymbol>> Get()
        {
            return Ok(this.currencySymbolRepository.FindAll());
        }
    }
}
