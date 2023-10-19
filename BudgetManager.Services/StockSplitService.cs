using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using AutoMapper;
using BudgetManager.Data.DataModels;
using BudgetManager.Domain.DTOs;
using BudgetManager.Repository;
using BudgetManager.Services.Contracts;
using Microsoft.EntityFrameworkCore;

namespace BudgetManager.Services
{
    public class StockSplitService : BaseService<StockSplitModel, StockSplit, IStockSplitRepository>, IStockSplitService
    {
        public StockSplitService(IStockSplitRepository repository, IMapper mapper) : base(repository, mapper)
        {
        }

        public IEnumerable<StockSplitAccumulated> GetSplitAccumulated()
        {
            var accumulatedData = this.repository.FindAll()
                .GroupBy(s => s.StockTickerId)
                .Select(g => g.OrderBy(s => s.SplitTimeStamp).Select(e => new StockSplitAccumulated
                {
                    Id = e.Id,
                    StockTickerId = e.StockTickerId,
                    SpliDateTime = e.SplitTimeStamp,
                    SplitAccumulatedCoeficient = e.SplitCoefficient
                }).ToList())
                .ToList();

            foreach (var group in accumulatedData)
            {
                for (int i = 1; i < group.Count; i++)
                {
                    group[i].SplitAccumulatedCoeficient *= group[i - 1].SplitAccumulatedCoeficient;
                }
            }

            return accumulatedData.SelectMany(d => d);
        }
    }
}
