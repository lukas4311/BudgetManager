using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BudgetManager.Data.DataModels.EntityConfiguration
{
    internal class TaxSettingConfiguration : IEntityTypeConfiguration<TaxSetting>
    {
        private const int TaxTypeMaxLength = 50;

        public void Configure(EntityTypeBuilder<TaxSetting> builder)
        {
            builder.Property(b => b.TaxType)
                .HasMaxLength(TaxTypeMaxLength)
                .IsRequired();

            builder.HasIndex(b => b.TaxType)
                .IsUnique();
        }
    }
}
