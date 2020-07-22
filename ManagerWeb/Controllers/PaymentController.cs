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
            payments.Add(new Payment { Amount = 140, Date = new DateTime(2020, 7, 19), Id = 2, Name = "bendas" });
            payments.Add(new Payment { Amount = 109, Date = new DateTime(2020, 7, 12), Id = 3, Name = "obed" });
            payments.Add(new Payment { Amount = 549, Date = new DateTime(2020, 7, 16), Id = 4, Name = "Decathlon", Description = "Kraviny asjdfkljadk lfjkla." });
            payments.Add(new Payment { Amount = 2500, Date = new DateTime(2020, 7, 10), Id = 5, Name = "BTC" });
            payments.Add(new Payment { Amount = 300, Date = new DateTime(2020, 7, 20), Id = 6, Name = "DM", Description = "Mliko." });
            payments.Add(new Payment { Amount = 240, Date = new DateTime(2020, 7, 1), Id = 7, Name = "bendas" });
            return Json(payments);
        }

        [HttpPost]
        public JsonResult AddPayment(Payment payment)
        {

            return Json(new { success = true });
        }
    }
}