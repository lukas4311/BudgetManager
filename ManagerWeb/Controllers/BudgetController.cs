using Data.DataModels;
using ManagerWeb.Models.DTOs;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Repository;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;

namespace ManagerWeb.Controllers
{
    [ApiController]
    [Route("budget")]
    public class BudgetController : ControllerBase
    {
        private readonly IBudgetRepository budgetRepository;
        private readonly IHttpContextAccessor httpContextAccessor;

        public BudgetController(IBudgetRepository budgetRepository, IHttpContextAccessor httpContextAccessor)
        {
            this.budgetRepository = budgetRepository;
            this.httpContextAccessor = httpContextAccessor;
        }

        [HttpGet("getAll")]
        public IActionResult Get()
        {
            string loggedUserLogin = httpContextAccessor.HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);
            ICollection<BudgetModel> budgets = this.budgetRepository.FindByCondition(a => a.UserIdentity.Login == loggedUserLogin).Select(b => new BudgetModel
            {
                Amount = b.Amount,
                DateFrom = b.DateFrom,
                DateTo = b.DateTo,
                Id = b.Id
            }).ToList();

            return Ok(budgets);
        }

        [HttpPost("add")]
        public IActionResult Add([FromBody] BudgetModel budgetModel)
        {
            string loggedUserLogin = httpContextAccessor.HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);

            this.budgetRepository.Create(new Budget()
            {
                Amount = budgetModel.Amount,
                DateFrom = budgetModel.DateFrom,
                DateTo = budgetModel.DateTo,
            });

            return Ok();
        }
    }
}
