using Microsoft.EntityFrameworkCore;

namespace Data.DataModels.EntityConfiguration
{
    internal static class TaxSettingConfiguration
    {
        private const int TaxTypeMaxLength = 50;

        internal static void ConfigureTax(this ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<TaxSetting>()
                .Property(b => b.TaxType)
                .HasMaxLength(TaxTypeMaxLength)
                .IsRequired();

            modelBuilder.Entity<TaxSetting>()
                .HasIndex(b => b.TaxType)
                .IsUnique();
        }
    }
}
