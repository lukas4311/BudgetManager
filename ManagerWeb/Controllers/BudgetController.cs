﻿using System.Collections.Generic;
using BudgetManager.ManagerWeb.Models.DTOs;
using BudgetManager.ManagerWeb.Services;
using Microsoft.AspNetCore.Mvc;

namespace BudgetManager.ManagerWeb.Controllers
{
    [ApiController]
    [Route("budget")]
    public class BudgetController : ControllerBase
    {
        private readonly IBudgetService budgetService;

        public BudgetController(IBudgetService budgetService)
        {
            this.budgetService = budgetService;
        }

        [HttpGet("getAll")]
        public ActionResult<IEnumerable<BudgetModel>> Get()
        {
            return Ok(this.budgetService.Get());
        }

        [HttpGet("get")]
        public ActionResult<BudgetModel> Get(int id)
        {
            return Ok(this.budgetService.Get(id));
        }

        [HttpGet("getActual")]
        public ActionResult<IEnumerable<BudgetModel>> GetActual()
        {
            return Ok(this.budgetService.GetActual());
        }

        [HttpPost("add")]
        public IActionResult Add([FromBody] BudgetModel budgetModel)
        {
            this.budgetService.Add(budgetModel);
            return Ok();
        }

        [HttpPut("update")]
        public IActionResult Update([FromBody] BudgetModel budgetModel)
        {
            this.budgetService.Update(budgetModel);
            return Ok();
        }

        [HttpDelete("delete")]
        public IActionResult Delete([FromBody] int id)
        {
            this.budgetService.Delete(id);
            return Ok();
        }
    }
}
