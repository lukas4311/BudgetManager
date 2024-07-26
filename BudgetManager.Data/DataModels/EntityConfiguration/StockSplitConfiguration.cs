using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BudgetManager.Data.DataModels.EntityConfiguration
{
    public class StockSplitConfiguration : IEntityTypeConfiguration<StockSplit>
    {
        public void Configure(EntityTypeBuilder<StockSplit> builder)
        {
            builder.Property(i => i.TickerId)
                .IsRequired();

            builder.HasOne(e => e.Ticker)
                .WithMany()
                .HasForeignKey(e => e.TickerId);
        }
    }
}