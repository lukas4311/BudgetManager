using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BudgetManager.Data.DataModels.EntityConfiguration
{
    public class CurrencySymbolConfiguration : IEntityTypeConfiguration<CurrencySymbol>
    {
        private const int SymbolMaxLenght = 20;

        public void Configure(EntityTypeBuilder<CurrencySymbol> builder)
        {
            builder.Property(b => b.Symbol)
                .HasMaxLength(SymbolMaxLenght)
                .IsRequired();

            builder.HasIndex(p => p.Symbol)
                .IsUnique();
        }
    }
}
