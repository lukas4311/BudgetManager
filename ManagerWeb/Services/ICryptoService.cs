using ManagerWeb.Models.DTOs;
using System.Collections.Generic;

namespace ManagerWeb.Services
{
    public interface ICryptoService
    {
        IEnumerable<TradeHistory> Get();

        TradeHistory Get(int id);
    }
}