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
                .Include(s => s.StockTicker)
                .OrderBy(e => e.StockTickerId)
                .ThenBy(e => e.SplitTimeStamp)
                .GroupBy(e => e.StockTickerId)
                .SelectMany(group => group
                    .OrderBy(e => e.SplitTimeStamp)
                    .Select((e, index) => new StockSplitAccumulated(e.Id, e.StockTickerId, e.SplitTimeStamp, group
                        .Where(subE => subE.SplitTimeStamp <= e.SplitTimeStamp)
                        .Sum(subE => subE.SplitCoefficient))
                    ));

            return accumulatedData;
        }
    }
}
