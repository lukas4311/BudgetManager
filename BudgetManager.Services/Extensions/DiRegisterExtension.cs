using System.Linq;
using Autofac;
using AutoMapper;
using AutoMapper.Internal;
using BudgetManager.Data.DataModels;
using BudgetManager.Domain.DTOs;

namespace BudgetManager.Services.Extensions
{
    public static class DiRegisterExtension
    {
        public static void RegisterServices(this ContainerBuilder containerBuilder)
        {
            containerBuilder.RegisterAssemblyTypes(typeof(DiRegisterExtension).Assembly)
                .Where(t => t.Namespace == "BudgetManager.Services")
               .AsImplementedInterfaces()
               .InstancePerLifetimeScope();
        }

        public static void RegisterModelMapping(this ContainerBuilder containerBuilder)
        {
            //Automapper
            MapperConfiguration config = new MapperConfiguration(

                cfg =>
                {
                    cfg.Internal().MethodMappingEnabled = false;
                    cfg.CreateMap<BankAccount, BankAccountModel>()
                        .ForMember(dest => dest.Code, opt => opt.MapFrom(src => src.Code))
                        .ForMember(dest => dest.OpeningBalance, opt => opt.MapFrom(src => src.OpeningBalance))
                        .ForMember(dest => dest.UserIdentityId, opt => opt.MapFrom(src => src.UserIdentityId))
                        .ForMember(o => o.Id, m => m.Ignore());

                    cfg.CreateMap<BankAccountModel, BankAccount>()
                        .ForMember(dest => dest.Code, opt => opt.MapFrom(src => src.Code))
                        .ForMember(dest => dest.OpeningBalance, opt => opt.MapFrom(src => src.OpeningBalance))
                        .ForMember(dest => dest.UserIdentityId, opt => opt.MapFrom(src => src.UserIdentityId));

                    cfg.CreateMap<BudgetModel, Budget>();
                    cfg.CreateMap<Budget, BudgetModel>();
                    cfg.CreateMap<ComodityTradeHistoryModel, ComodityTradeHistory>();
                    cfg.CreateMap<ComodityTradeHistory, ComodityTradeHistoryModel>();
                    cfg.CreateMap<CryptoTradeHistory, TradeHistory>();
                    cfg.CreateMap<TradeHistory, CryptoTradeHistory>();
                    cfg.CreateMap<Tag, TagModel>();
                    cfg.CreateMap<TagModel, Tag>();
                    cfg.CreateMap<PaymentModel, Payment>();
                    cfg.CreateMap<Payment, PaymentModel>();
                    cfg.CreateMap<OtherInvestment, OtherInvestmentModel>();
                    cfg.CreateMap<OtherInvestmentModel, OtherInvestment>();
                    cfg.CreateMap<OtherInvestmentBalaceHistory, OtherInvestmentBalaceHistoryModel>();
                    cfg.CreateMap<OtherInvestmentBalaceHistoryModel, OtherInvestmentBalaceHistory>();
                    cfg.CreateMap<OtherInvestmentTag, OtherInvestmentTagModel>();
                    cfg.CreateMap<OtherInvestmentTagModel, OtherInvestmentTag>();
                    cfg.CreateMap<StockTickerModel, StockTicker>();
                    cfg.CreateMap<StockTicker, StockTickerModel>();
                    cfg.CreateMap<StockTradeHistoryModel, StockTradeHistory>();
                    cfg.CreateMap<StockTradeHistory, StockTradeHistoryModel>();
                    cfg.CreateMap<StockTradeHistory, StockTradeHistoryGetModel>()
                    //.ForMember(m => m.TradeSizeAfterAplit, act => act.Ignore())
                    .ForMember(dest => dest.CurrencySymbol, opt =>
                    {
                        opt.PreCondition(src => src.CurrencySymbol is not null);
                        opt.MapFrom(x => x.CurrencySymbol.Symbol);
                    });
                    //.ForMember(a => a.CurrencySymbol, o => o.MapFrom(x => x.CurrencySymbol.Symbol));
                    cfg.CreateMap<StockTradeHistoryGetModel, StockTradeHistory>();
                        
                    cfg.CreateMap<CompanyProfile, CompanyProfileModel>();
                    cfg.CreateMap<CompanyProfileModel, CompanyProfile>();
                    cfg.CreateMap<StockSplit, StockSplitModel>();
                    cfg.CreateMap<StockSplitModel, StockSplit>();
                    cfg.CreateMap<UserCreateModel, UserData>();
                    cfg.CreateMap<UserCreateModel, UserIdentification>();
                }
            );

            containerBuilder.RegisterInstance(config).As<IConfigurationProvider>();
            containerBuilder.RegisterType<Mapper>().As<IMapper>();
        }
    }
}
