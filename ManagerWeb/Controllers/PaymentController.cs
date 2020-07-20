using System;
using System.Collections.Generic;
using Data.DataModels;
using Microsoft.AspNetCore.Mvc;

namespace ManagerWeb.Controllers
{
    public class PaymentController : Controller
    {
        [HttpGet]
        public JsonResult GetPaymentsData(DateTime fromDate)
        {
            List<Payment> payments = new List<Payment>();
            payments.Add(new Payment { Amount = 150, Date = DateTime.Now, Id = 1, Name = "MC Donald", Description = "Nejaka utrata v MC Donald. Asi nejaky cheseburger." });
            payments.Add(new Payment { Amount = 140, Date = DateTime.Now, Id = 2, Name = "bendas" });
            payments.Add(new Payment { Amount = 109, Date = DateTime.Now, Id = 3, Name = "obed" });
            payments.Add(new Payment { Amount = 549, Date = DateTime.Now, Id = 4, Name = "Decathlon", Description = "Kraviny asjdfkljadk lfjkla." });
            return Json(payments);
        }
    }
}