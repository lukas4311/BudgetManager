using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BudgetManager.Data.DataModels.EntityConfiguration
{
    internal class TickerAdjustedInfoConfiguration : IEntityTypeConfiguration<TickerAdjustedInfo>
    {
        private const int MaxLengthSymbol = 10;

        public void Configure(EntityTypeBuilder<TickerAdjustedInfo> modelBuilder)
        {
            modelBuilder.Property(b => b.PriceTicker)
                .HasMaxLength(MaxLengthSymbol)
                .IsRequired();

            modelBuilder.Property(b => b.CompanyInfoTicker)
                .HasMaxLength(MaxLengthSymbol)
                .IsRequired();
        }
    }
}
