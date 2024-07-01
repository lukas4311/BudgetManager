using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BudgetManager.Data.DataModels.EntityConfiguration
{
    public class TradeConfiguration : IEntityTypeConfiguration<Trade>
    {
        public void Configure(EntityTypeBuilder<Trade> builder)
        {
            builder.HasOne(e => e.Ticker)
               .WithMany()
               .HasForeignKey(e => e.TickerId)
               .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(e => e.TradeCurrencySymbol)
               .WithMany()
               .HasForeignKey(e => e.TradeCurrencySymbolId)
               .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(e => e.UserIdentity)
                .WithMany()
                .HasForeignKey(e => e.UserIdentityId);
        }
    }
}
