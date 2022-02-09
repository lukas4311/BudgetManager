using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BudgetManager.Data.DataModels.EntityConfiguration
{
    internal class ComodityTradeHistoryConfiguration : IEntityTypeConfiguration<ComodityTradeHistory>
    {
        public void Configure(EntityTypeBuilder<ComodityTradeHistory> builder)
        {
            builder.Property(i => i.ComodityTypeId)
                        .IsRequired();

            builder.HasOne(e => e.ComodityType)
               .WithMany(e => e.ComodityTradeHistory)
               .HasForeignKey(e => e.ComodityTypeId);

            builder.HasOne(e => e.CurrencySymbol)
               .WithMany(e => e.ComodityTradeHistory)
               .HasForeignKey(e => e.CurrencySymbolId);

            builder.HasOne(e => e.UserIdentity)
                .WithMany(e => e.ComodityTradeHistory)
                .HasForeignKey(e => e.UserIdentityId);
        }
    }
}
