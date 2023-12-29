using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BudgetManager.Data.DataModels.EntityConfiguration
{
    internal class BrokerReportToProcessConfiguration : IEntityTypeConfiguration<BrokerReportToProcess>
    {
        private const int MaxLengthCode = 20;

        public void Configure(EntityTypeBuilder<BrokerReportToProcess> builder)
        {
            builder.HasOne(e => e.BrokerReportToProcessState)
                .WithMany(e => e.BrokerReportsToProcess)
                .HasForeignKey(e => e.BrokerReportToProcessStateId);
        }
    }
}
