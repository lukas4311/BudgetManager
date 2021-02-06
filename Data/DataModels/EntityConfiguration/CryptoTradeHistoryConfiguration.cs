using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Data.DataModels.EntityConfiguration
{
    public class CryptoTradeHistoryConfiguration : IEntityTypeConfiguration<CryptoTradeHistory>
    {
        public void Configure(EntityTypeBuilder<CryptoTradeHistory> builder)
        {
            builder.Property(i => i.CryptoTickerId)
                        .IsRequired();

            builder.HasOne(e => e.CryptoTicker)
               .WithMany(e => e.CryptoTradeHistories)
               .HasForeignKey(e => e.CryptoTickerId);

            builder.HasOne(e => e.CurrencySymbol)
               .WithMany(e => e.CryptoTradeHistory)
               .HasForeignKey(e => e.CurrencySymbolId);

            builder.HasOne(e => e.UserIdentity)
                .WithMany(e => e.CryptoTradesHistory)
                .HasForeignKey(e => e.UserIdentityId);
        }
    }
}
