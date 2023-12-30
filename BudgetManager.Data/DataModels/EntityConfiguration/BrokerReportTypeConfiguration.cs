using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BudgetManager.Data.DataModels.EntityConfiguration
{
    public class BrokerReportTypeConfiguration : IEntityTypeConfiguration<BrokerReportType>
    {
        private const int MaxLengthCode = 20;
        private const int MaxLengthName = 100;

        public void Configure(EntityTypeBuilder<BrokerReportType> builder)
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
