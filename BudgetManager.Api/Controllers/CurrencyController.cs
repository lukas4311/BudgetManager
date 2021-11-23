using System.Collections.Generic;
using System.Threading.Tasks;
using BudgetManager.Domain.DTOs;
using BudgetManager.Repository;
using BudgetManager.Services.Contracts;
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
        public ActionResult<IEnumerable<TradeHistory>> Get()
        {
            return Ok(this.currencySymbolRepository.FindAll());
        }
    }
}
