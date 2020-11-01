using ManagerWeb.Models.DTOs;
using System;
using System.Collections.Generic;

namespace ManagerWeb.Services
{
    internal interface IBankAccountService
    {
        IEnumerable<BankBalanceModel> GetBankAccountsBalanceToDate(DateTime? toDate);
    }
}