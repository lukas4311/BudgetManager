using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BudgetManager.Data.DataModels.EntityConfiguration
{
    public class BrokerReportToProcessStateConfiguration : IEntityTypeConfiguration<BrokerReportToProcessState>
    {
        private const int MaxLengthCode = 20;
        private const int MaxLengthName = 100;

        public void Configure(EntityTypeBuilder<BrokerReportToProcessState> builder)
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
