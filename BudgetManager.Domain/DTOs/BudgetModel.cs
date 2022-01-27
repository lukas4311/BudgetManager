using AutoMapper;
using BudgetManager.Data.DataModels;
using System;

namespace BudgetManager.Domain.DTOs
{
    public class BudgetModel : IUserDtoModel<Budget>
    {
        public int Id { get; set; }

        public DateTime DateFrom { get; set; }

        public DateTime DateTo { get; set; }

        public int Amount { get; set; }

        public string Name { get; set; }

        public int UserIdentityId { get; set; }

        public Budget ToEntity()
        {
            MapperConfiguration config = new MapperConfiguration(cfg => cfg.CreateMap<BudgetModel, Budget>());
            Mapper mapper = new Mapper(config);
            return mapper.Map<Budget>(this);
        }
    }
}
