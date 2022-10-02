using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BudgetManager.Data.DataModels.EntityConfiguration
{
    public class StockTradeHistoryConfiguration : IEntityTypeConfiguration<StockTradeHistory>
    {
        public void Configure(EntityTypeBuilder<StockTradeHistory> builder)
        {
            builder.Property(i => i.StockTickerId)
                        .IsRequired();

            builder.HasOne(e => e.StockTicker)
               .WithMany()
               .HasForeignKey(e => e.StockTickerId);

            builder.HasOne(e => e.CurrencySymbol)
               .WithMany()
               .HasForeignKey(e => e.CurrencySymbolId);

            builder.HasOne(e => e.UserIdentity)
                .WithMany()
                .HasForeignKey(e => e.UserIdentityId);
        }
    }
}
