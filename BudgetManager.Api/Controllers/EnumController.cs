using BudgetManager.Data.DataModels;
using BudgetManager.Domain.DTOs;
using BudgetManager.Repository;
using BudgetManager.Services.Contracts;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace BudgetManager.Api.Controllers
{
    [ApiController]
    [Route("enumItem")]
    public class EnumController : BaseController
    {
        private readonly IBaseService<EnumItemModel, EnumItem, IRepository<EnumItem>> enumItemService;

        public EnumController(IHttpContextAccessor httpContextAccessor, IBaseService<EnumItemModel, EnumItem, IRepository<EnumItem>> enumItemService) : base(httpContextAccessor)
        {
            this.enumItemService = enumItemService;
        }

        [HttpGet]
        public void GetAll()
        {
            Ok(enumItemService.GetAll());
        }
    }
}
