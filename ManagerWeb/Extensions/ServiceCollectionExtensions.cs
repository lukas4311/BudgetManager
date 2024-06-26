﻿using BudgetManager.ManagerWeb.Services;
using Microsoft.Extensions.DependencyInjection;

namespace BudgetManager.ManagerWeb.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static void RegisterServices(this IServiceCollection services)
        {
            services.AddTransient<IUserService, UserService>();
            services.AddTransient<IPaymentService, PaymentService>();
            services.AddTransient<ITagService, TagService>();
            services.AddTransient<IBankAccountService, BankAccountService>();
            services.AddTransient<IBudgetService, BudgetService>();
            services.AddTransient<ICryptoService, CryptoService>();
            services.AddTransient<IForexService, ForexService>();
        }
    }
}
