using System.ComponentModel;
using System.Linq;
using Autofac;
using AutoMapper;
using AutoMapper.Internal;
using BudgetManager.Data.DataModels;
using BudgetManager.Domain.DTOs;
using BudgetManager.Services.Contracts;

namespace BudgetManager.Services.Extensions
{
    /// <summary>
    /// Dependency injection container registration extensions
    /// </summary>
    public static class DiRegisterExtension
    {
        /// <summary>
        /// Register all services
        /// </summary>
        /// <param name="containerBuilder">Container builder</param>
        public static void RegisterServices(this ContainerBuilder containerBuilder)
        {
            containerBuilder.RegisterAssemblyTypes(typeof(DiRegisterExtension).Assembly)
                .Where(t => t.Namespace == "BudgetManager.Services")
               .AsImplementedInterfaces()
               .InstancePerLifetimeScope();

            containerBuilder.RegisterGeneric(typeof(BaseService<,,>))
                .As(typeof(IBaseService<,,>))
                .InstancePerLifetimeScope();
        }

        /// <summary>
        /// Registration of all automapper maping configuration
        /// </summary>
        /// <param name="containerBuilder">Container builders</param>
        public static void RegisterModelMapping(this ContainerBuilder containerBuilder)
        {
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
                    cfg.CreateMap<StockTradeHistoryModel, Trade>()
                        .ForMember(dest => dest.TickerId, opt => opt.MapFrom(x => x.StockTickerId))
                        .ForMember(dest => dest.TradeCurrencySymbolId, opt => opt.MapFrom(x => x.CurrencySymbolId))
                        .ReverseMap();
                    cfg.CreateMap<CompanyProfile, CompanyProfileModel>();
                    cfg.CreateMap<CompanyProfileModel, CompanyProfile>();
                    cfg.CreateMap<StockSplit, StockSplitModel>()
                        .ForMember(dest => dest.StockTickerId, opt => opt.MapFrom(x => x.TickerId))
                        .ReverseMap();
                    //cfg.CreateMap<StockSplitModel, StockSplit>();
                    cfg.CreateMap<UserCreateModel, UserData>();
                    cfg.CreateMap<UserCreateModel, UserIdentity>();
                    cfg.CreateMap<NotificationModel, Notification>();
                    cfg.CreateMap<Notification, NotificationModel>();
                    cfg.CreateMap<EnumItem, EnumItemModel>();
                    cfg.CreateMap<Trade, StockTradeHistoryGetModel>()
                        .ForMember(dest => dest.CurrencySymbolId, opt => opt.MapFrom(x => x.TradeCurrencySymbolId))
                        .ForMember(dest => dest.CurrencySymbol, opt => opt.MapFrom(x => x.TradeCurrencySymbol.Code))
                        .ForMember(dest => dest.StockTickerId, opt => opt.MapFrom(x => x.TickerId));
                    cfg.CreateMap<StockTradeHistoryGetModel, Trade>()
                        .ForMember(dest => dest.TradeCurrencySymbolId, opt => opt.MapFrom(x => x.CurrencySymbolId))
                        .ForMember(dest => dest.TickerId, opt => opt.MapFrom(x => x.StockTickerId));
                    cfg.CreateMap<Trade, TradeHistory>()
                        .ForMember(dest => dest.CurrencySymbolId, opt => opt.MapFrom(x => x.TradeCurrencySymbolId))
                        .ForMember(dest => dest.CurrencySymbol, opt => opt.MapFrom(x => x.TradeCurrencySymbol.Code))
                        .ForMember(dest => dest.CryptoTickerId, opt => opt.MapFrom(x => x.TickerId))
                        .ForMember(dest => dest.CryptoTicker, opt => opt.MapFrom(x => x.Ticker.Code));
                    cfg.CreateMap<TradeHistory, Trade>()
                        .ForMember(dest => dest.TradeCurrencySymbolId, opt => opt.MapFrom(x => x.CurrencySymbolId))
                        .ForMember(dest => dest.TickerId, opt => opt.MapFrom(x => x.CryptoTickerId));
                    cfg.CreateMap<EnumItem, StockTickerModel>()
                        .ForMember(dest => dest.Ticker, opt => opt.MapFrom(x => x.Code))
                        .ReverseMap();
                    cfg.CreateMap<EnumItem, CryptoTickerModel>()
                        .ForMember(dest => dest.Ticker, opt => opt.MapFrom(x => x.Code))
                        .ReverseMap();
                }
            );

            containerBuilder.RegisterInstance(config).As<IConfigurationProvider>();
            containerBuilder.RegisterType<Mapper>().As<IMapper>();
        }
    }
}
