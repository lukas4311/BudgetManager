using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BudgetManager.Data.DataModels.EntityConfiguration
{
    internal class EnumItemConfiguration : IEntityTypeConfiguration<EnumItem>
    {
        private const int MaxLengthCode = 40;
        private const int MaxLengthName = 100;

        public void Configure(EntityTypeBuilder<EnumItem> modelBuilder)
        {
            modelBuilder.Property(b => b.Code)
                .HasMaxLength(MaxLengthCode)
                .IsRequired();

            modelBuilder.Property(b => b.Name)
                .HasMaxLength(MaxLengthName)
                .IsRequired();

            modelBuilder.HasIndex(b => new { b.Code, b.EnumItemTypeId })
                .IsUnique();

            modelBuilder.Property(b => b.EnumItemTypeId)
                .IsRequired();
        }
    }
}
