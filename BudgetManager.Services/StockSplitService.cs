using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using BudgetManager.Data.DataModels;
using BudgetManager.Domain.DTOs;
using BudgetManager.Repository;
using BudgetManager.Services.Contracts;

namespace BudgetManager.Services
{
    public class StockSplitService : BaseService<StockSplitModel, StockSplit, IStockSplitRepository>, IStockSplitService
    {
        public StockSplitService(IStockSplitRepository repository, IMapper mapper) : base(repository, mapper)
        {
        }

        public IEnumerable<StockSplitAccumulated> GetSplitAccumulated()
        {
            IEnumerable<List<StockSplitAccumulated>> data = this.GetSplitsMappedToModel().Select(t => t.Splits.ToList());
            IEnumerable<List<StockSplitAccumulated>> accumulatedData = this.AccumulateSplits(data);
            return accumulatedData.SelectMany(d => d);
        }

        public IEnumerable<(int tickerId, List<StockSplitAccumulated> splits)> GetGrupedAccumulatedSplits()
        {
            IEnumerable<List<StockSplitAccumulated>> data = this.GetSplitsMappedToModel().Select(t => t.Splits.ToList());
            IEnumerable<List<StockSplitAccumulated>> accumulatedData = this.AccumulateSplits(data);
            return accumulatedData.Select(g => (g[0].StockTickerId, g));
        }

        public IEnumerable<StockSplitAccumulated> GetTickerSplits(int tickerId)
            => this.GetSplitsMappedToModel().SingleOrDefault(s => s.TickerId == tickerId)?.Splits;

        private IEnumerable<List<StockSplitAccumulated>> AccumulateSplits(IEnumerable<List<StockSplitAccumulated>> accumulatedData)
        {
            foreach (var group in accumulatedData)
                for (int i = 1; i < group.Count(); i++)
                    group[i].SplitAccumulatedCoeficient *= group[i - 1].SplitAccumulatedCoeficient;

            return accumulatedData;
        }

        private IEnumerable<GroupedStockAccumulatedSpits> GetSplitsMappedToModel()
        {
            return this.repository.FindAll()
                .ToList()
                .GroupBy(s => s.StockTickerId)
                .Select(g => new GroupedStockAccumulatedSpits
                (g.Key, g.OrderBy(s => s.SplitTimeStamp).Select(e => new StockSplitAccumulated
                {
                    Id = e.Id,
                    StockTickerId = e.StockTickerId,
                    SpliDateTime = e.SplitTimeStamp,
                    SplitAccumulatedCoeficient = e.SplitCoefficient
                })));
        }
    }
}
