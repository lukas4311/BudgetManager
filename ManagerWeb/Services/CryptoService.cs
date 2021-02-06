using Data.DataModels;
using ManagerWeb.Extensions;
using ManagerWeb.Models.DTOs;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Repository;
using System.Collections.Generic;
using System.Linq;

namespace ManagerWeb.Services
{
    public class CryptoService : ICryptoService
    {
        private readonly IHttpContextAccessor httpContextAccessor;
        private readonly ICryptoTradeHistoryRepository cryptoTradeHistoryRepository;
        private readonly IUserIdentityRepository userIdentityRepository;

        public CryptoService(IHttpContextAccessor httpContextAccessor, ICryptoTradeHistoryRepository cryptoTradeHistoryRepository, IUserIdentityRepository userIdentityRepository)
        {
            this.httpContextAccessor = httpContextAccessor;
            this.cryptoTradeHistoryRepository = cryptoTradeHistoryRepository;
            this.userIdentityRepository = userIdentityRepository;
        }

        public IEnumerable<TradeHistory> Get()
        {
            string userName = this.httpContextAccessor.HttpContext.User.Identity.Name;
            return this.userIdentityRepository.FindByCondition(u => u.Login == userName)
                .SelectMany(a => a.CryptoTradesHistory)
                .Include(s => s.CurrencySymbol)
                .Include(s => s.CryptoTicker)
                .Select(e => e.MapToOverViewViewModel());
        }

        public TradeHistory Get(int id)
        {
            string userName = this.httpContextAccessor.HttpContext.User.Identity.Name;
            return this.userIdentityRepository.FindByCondition(u => u.Login == userName)
                .SelectMany(a => a.CryptoTradesHistory)
                .Include(s => s.CurrencySymbol)
                .Include(s => s.CryptoTicker)
                .Where(s => s.Id == id)
                .Select(e => e.MapToOverViewViewModel())
                .Single();
        }
    }
}
