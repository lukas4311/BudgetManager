﻿using ManagerWeb.Models.DTOs;
using ManagerWeb.Services;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;

namespace ManagerWeb.Controllers
{
    [ApiController]
    [Route("crypto")]
    public class CryptoController : ControllerBase
    {
        private readonly ICryptoService cryptoService;

        public CryptoController(ICryptoService cryptoService)
        {
            this.cryptoService = cryptoService;
        }

        [HttpGet("getAll")]
        public ActionResult<IEnumerable<TradeHistory>> Get()
        {
            return Ok(this.cryptoService.Get());
        }

        [HttpGet("get")]
        public IActionResult Get(int id)
        {
            return Ok(this.cryptoService.Get(id));
        }
    }
}