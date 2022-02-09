using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BudgetManager.Data.DataModels.EntityConfiguration
{
    internal class ComodityTypeConfiguration : IEntityTypeConfiguration<ComodityType>
    {
        private const int MaxLengthCode = 20;
        private const int MaxLengthName = 100;

        public void Configure(EntityTypeBuilder<ComodityType> builder)
        {
            builder.HasIndex(a => a.Code)
                .IsUnique();

            builder.Property(a => a.Code)
                .HasMaxLength(MaxLengthCode);

            builder.Property(a => a.Name)
                .HasMaxLength(MaxLengthName);

            builder.Property(a => a.ComodityUnitId)
                .IsRequired();

            builder.HasOne(a => a.ComodityUnit)
                .WithMany(c => c.ComodityTypes)
                .HasForeignKey(e => e.ComodityUnitId);
        }
    }
}
