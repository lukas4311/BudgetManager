using Asp.Versioning;
using BudgetManager.Domain.DTOs;
using BudgetManager.Services.Contracts;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;

namespace BudgetManager.Api.Controllers
{
    /// <summary>
    /// Controller responsible for handling enum item related operations in the Budget Manager API.
    /// Provides endpoints for retrieving enum items.
    /// </summary>
    [ApiController]
    [ApiVersion("1.0")]
    [Route("v{version:apiVersion}/enumItem")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    [Produces("application/json", "application/problem+json")]
    public class EnumController : BaseController
    {
        private readonly IEnumService enumItemService;

        /// <summary>
        /// Initializes a new instance of the <see cref="EnumController"/> class.
        /// </summary>
        /// <param name="httpContextAccessor">The HTTP context accessor for accessing the current HTTP context.</param>
        /// <param name="enumItemService">The service responsible for enum item operations.</param>
        public EnumController(IHttpContextAccessor httpContextAccessor, IEnumService enumItemService) : base(httpContextAccessor)
        {
            this.enumItemService = enumItemService;
        }

        /// <summary>
        /// Retrieves all enum items.
        /// </summary>
        /// <returns>A collection of all enum items.</returns>
        /// <response code="200">Returns the collection of enum items.</response>
        [HttpGet, MapToApiVersion("1.0")]
        public ActionResult<IEnumerable<EnumItemModel>> GetAll()
        {
            return Ok(enumItemService.GetAll());
        }

        /// <summary>
        /// Retrieves an enum item by its code.
        /// </summary>
        /// <param name="enumItemCode">The code of the enum item to retrieve.</param>
        /// <returns>The enum item with the specified code.</returns>
        /// <response code="200">Returns the enum item with the specified code.</response>
        [HttpGet("{enumItemCode}"), MapToApiVersion("1.0")]
        public ActionResult<EnumItemModelAdjusted> GetByCode([FromRoute] string enumItemCode)
        {
            return Ok(enumItemService.GetByCode(enumItemCode));
        }

        /// <summary>
        /// Retrieves all enum items of a specific type.
        /// </summary>
        /// <param name="enumItemTypeCode">The code of the enum item type to filter by.</param>
        /// <returns>A collection of enum items of the specified type.</returns>
        /// <response code="200">Returns the collection of enum items of the specified type.</response>
        [HttpGet("type/{enumItemTypeCode}"), MapToApiVersion("1.0")]
        public ActionResult<IEnumerable<EnumItemModelAdjusted>> GetAllByTypeCode([FromRoute] string enumItemTypeCode)
        {
            return Ok(enumItemService.GetAllByTypeCode(enumItemTypeCode));
        }
    }
}