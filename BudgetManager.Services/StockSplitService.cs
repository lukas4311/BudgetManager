using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using BudgetManager.Data.DataModels;
using BudgetManager.Domain.DTOs;
using BudgetManager.Repository;
using BudgetManager.Services.Contracts;

namespace BudgetManager.Services
{
    /// <inheritdoc/>
    public class StockSplitService : BaseService<StockSplitModel, StockSplit, IStockSplitRepository>, IStockSplitService
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="StockSplitService"/> class.
        /// </summary>
        /// <param name="repository">The repository for stock splits.</param>
        /// <param name="mapper">The mapper for mapping between models.</param>
        public StockSplitService(IStockSplitRepository repository, IMapper mapper) : base(repository, mapper)
        {
        }

        /// <inheritdoc/>
        public IEnumerable<StockSplitAccumulated> GetSplitAccumulated()
        {
            IEnumerable<List<StockSplitAccumulated>> data = this.GetSplitsMappedToModel().Select(t => t.Splits.ToList());
            IEnumerable<List<StockSplitAccumulated>> accumulatedData = this.AccumulateSplits(data);
            return accumulatedData.SelectMany(d => d);
        }

        /// <inheritdoc/>
        public IEnumerable<(int tickerId, List<StockSplitAccumulated> splits)> GetGrupedAccumulatedSplits()
        {
            IEnumerable<List<StockSplitAccumulated>> data = this.GetSplitsMappedToModel().Select(t => t.Splits.ToList());
            IEnumerable<List<StockSplitAccumulated>> accumulatedData = this.AccumulateSplits(data);
            return accumulatedData.Select(g => (g[0].StockTickerId, g));
        }

        /// <inheritdoc/>
        public IEnumerable<StockSplitAccumulated> GetTickerSplits(int tickerId)
            => this.GetSplitsMappedToModel().SingleOrDefault(s => s.TickerId == tickerId)?.Splits;

        /// <inheritdoc/>
        public IEnumerable<List<StockSplitAccumulated>> AccumulateSplits(IEnumerable<List<StockSplitAccumulated>> accumulatedData)
        {
            foreach (List<StockSplitAccumulated> group in accumulatedData)
                for (int i = 1; i < group.Count(); i++)
                    group[i].SplitAccumulatedCoeficient *= group[i - 1].SplitAccumulatedCoeficient;

            return accumulatedData;
        }

        /// <inheritdoc/>
        public double GetAccumulatedCoefficient(IEnumerable<StockSplitModel> accumulatedData)
        {
            double accumulatedCoefficient = 1;
            foreach (StockSplitModel model in accumulatedData)
                accumulatedCoefficient *= model.SplitCoefficient;

            return accumulatedCoefficient;
        }

        /// <summary>
        /// Retrieves stock splits mapped to grouped stock accumulated splits.
        /// </summary>
        /// <returns>An enumerable of grouped stock accumulated splits.</returns>
        private IEnumerable<GroupedStockAccumulatedSpits> GetSplitsMappedToModel()
        {
            return this.repository.FindAll()
                .ToList()
                .GroupBy(s => s.StockTickerId)
                .Select(g => new GroupedStockAccumulatedSpits
                (
                    g.Key,
                    g.OrderBy(s => s.SplitTimeStamp)
                     .Select(e => new StockSplitAccumulated
                     {
                         Id = e.Id,
                         StockTickerId = e.StockTickerId,
                         SpliDateTime = e.SplitTimeStamp,
                         SplitAccumulatedCoeficient = e.SplitCoefficient
                     })
                ));
        }
    }

}
