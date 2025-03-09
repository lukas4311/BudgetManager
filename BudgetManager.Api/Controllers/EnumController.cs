using BudgetManager.Domain.DTOs;
using BudgetManager.Services.Contracts;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;

namespace BudgetManager.Api.Controllers
{
    [ApiController]
    [Route("enumItem")]
    public class EnumController : BaseController
    {
        private readonly IEnumService enumItemService;

        public EnumController(IHttpContextAccessor httpContextAccessor, IEnumService enumItemService) : base(httpContextAccessor)
        {
            this.enumItemService = enumItemService;
        }

        [Produces("application/json")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [HttpGet]
        public ActionResult<IEnumerable<EnumItemModel>> GetAll()
        {
            return Ok(enumItemService.GetAll());
        }

        [Produces("application/json")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [HttpGet("/{enumItemCode}")]
        public ActionResult<EnumItemModelAdjusted> GetByCode([FromRoute] string enumItemCode)
        {
            return Ok(enumItemService.GetByCode(enumItemCode));
        }

        [Produces("application/json")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [HttpGet("/type/{enumItemTypeCode}")]
        public ActionResult<IEnumerable<EnumItemModelAdjusted>> GetAllByTypeCode([FromRoute] string enumItemTypeCode)
        {
            return Ok(enumItemService.GetAllByTypeCode(enumItemTypeCode));
        }
    }
}
