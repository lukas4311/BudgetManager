using System.Collections.Generic;
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
    [Route("currency")]
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
        [Produces("application/json")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [HttpGet("all")]
        public ActionResult<IEnumerable<CurrencySymbol>> Get()
        {
            return Ok(currencySymbolRepository.FindAll());
        }
    }
}