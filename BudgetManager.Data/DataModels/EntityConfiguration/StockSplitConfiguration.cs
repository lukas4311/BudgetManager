using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BudgetManager.Data.DataModels.EntityConfiguration
{
    public class StockSplitConfiguration : IEntityTypeConfiguration<StockSplit>
    {
        public void Configure(EntityTypeBuilder<StockSplit> builder)
        {
            builder.Property(i => i.StockTickerId)
                .IsRequired();

            builder.HasOne(e => e.StockTicker)
                .WithMany()
                .HasForeignKey(e => e.StockTickerId);
        }
    }
}