using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BudgetManager.Data.DataModels.EntityConfiguration
{
    public class CryptoTickerConfiguration : IEntityTypeConfiguration<CryptoTicker>
    {
        private const int NameMaxLenght = 100;
        private const int TickerMaxLenght = 20;

        public void Configure(EntityTypeBuilder<CryptoTicker> builder)
        {
            builder.Property(b => b.Name)
                .HasMaxLength(NameMaxLenght)
                .IsRequired();

            builder.Property(b => b.Ticker)
                .HasMaxLength(TickerMaxLenght)
                .IsRequired();

            builder.HasIndex(a => a.Ticker)
                .IsUnique();
        }
    }
}
