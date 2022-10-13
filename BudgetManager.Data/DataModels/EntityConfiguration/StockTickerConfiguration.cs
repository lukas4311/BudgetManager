using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BudgetManager.Data.DataModels.EntityConfiguration
{
    public class StockTickerConfiguration : IEntityTypeConfiguration<StockTicker>
    {
        private const int NameMaxLenght = 100;
        private const int TickerMaxLenght = 20;

        public void Configure(EntityTypeBuilder<StockTicker> builder)
        {
            builder.Property(b => b.Ticker)
                .HasMaxLength(TickerMaxLenght)
                .IsRequired();

            builder.HasIndex(a => a.Ticker)
                .IsUnique();
        }
    }
}
