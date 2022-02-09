using BudgetManager.ManagerWeb.Models.SettingModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace BudgetManager.ManagerWeb.Controllers
{
    [ApiController]
    [Route("setting")]
    public class SettingController : ControllerBase
    {
        private ApiUrls apiUrls;

        public SettingController(IOptions<ApiUrls> options)
        {
            this.apiUrls = options.Value;
        }

        [HttpGet("apiRoutes")]
        public ActionResult<IOptions<ApiUrls>> GetPaymentTypes()
        {
            return Ok(this.apiUrls);
        }
    }
}
