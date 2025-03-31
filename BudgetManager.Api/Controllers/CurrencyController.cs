using System.Collections.Generic;
using Asp.Versioning;
using BudgetManager.Data.DataModels;
using BudgetManager.Repository;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace BudgetManager.Api.Controllers
{
    /// <summary>
    /// Controller responsible for handling currency-related operations in the Budget Manager API.
    /// Provides endpoints for retrieving currency information.
    /// </summary>
    [ApiController]
    [ApiVersion("1.0")]
    [Route("v{version:apiVersion}/currency")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    [Produces("application/json", "application/problem+json")]
    public class CurrencyController : BaseController
    {
        private readonly IRepository<CurrencySymbol> currencySymbolRepository;

        /// <summary>
        /// Initializes a new instance of the <see cref="CurrencyController"/> class.
        /// </summary>
        /// <param name="httpContextAccessor">The HTTP context accessor for accessing the current HTTP context.</param>
        /// <param name="currencySymbolRepository">The repository for currency symbol operations.</param>
        public CurrencyController(IHttpContextAccessor httpContextAccessor, IRepository<CurrencySymbol> currencySymbolRepository) : base(httpContextAccessor)
        {
            this.currencySymbolRepository = currencySymbolRepository;
        }

        /// <summary>
        /// Retrieves all available currency symbols.
        /// </summary>
        /// <returns>A collection of all currency symbols.</returns>
        /// <response code="200">Returns the collection of currency symbols.</response>
        [HttpGet("all"), MapToApiVersion("1.0")]
        public ActionResult<IEnumerable<CurrencySymbol>> Get()
        {
            return Ok(currencySymbolRepository.FindAll());
        }
    }
}