using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BudgetManager.Data.DataModels.EntityConfiguration
{
    public class ComodityUnitConfiguration : IEntityTypeConfiguration<ComodityUnit>
    {
        private const int MaxLengthCode = 20;
        private const int MaxLengthName = 100;

        public void Configure(EntityTypeBuilder<ComodityUnit> builder)
        {
            builder.HasIndex(a => a.Code)
                .IsUnique();

            builder.Property(a => a.Code)
                .HasMaxLength(MaxLengthCode);

            builder.Property(a => a.Name)
                .HasMaxLength(MaxLengthName);
        }
    }
}
