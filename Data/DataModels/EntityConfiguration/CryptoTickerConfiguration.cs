using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Data.DataModels.EntityConfiguration
{
    public class CryptoTickerConfiguration : IEntityTypeConfiguration<CryptoTicker>
    {
        public void Configure(EntityTypeBuilder<CryptoTicker> builder)
        {
            builder.HasIndex(a => a.Ticker)
                .IsUnique();
        }
    }
}
