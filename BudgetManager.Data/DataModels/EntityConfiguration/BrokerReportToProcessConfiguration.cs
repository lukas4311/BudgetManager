using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BudgetManager.Data.DataModels.EntityConfiguration
{
    internal class BrokerReportToProcessConfiguration : IEntityTypeConfiguration<BrokerReportToProcess>
    {
        public void Configure(EntityTypeBuilder<BrokerReportToProcess> builder)
        {
            builder.HasOne(e => e.BrokerReportToProcessState)
                .WithMany(e => e.BrokerReportsToProcess)
                .HasForeignKey(e => e.BrokerReportToProcessStateId);

            builder.HasOne(e => e.UserIdentity)
                .WithMany(e => e.BrokerReportsToProcess)
                .HasForeignKey(e => e.UserIdentityId);

            builder.HasOne(e => e.BrokerReportType)
               .WithMany(e => e.BrokerReportsToProcess)
               .HasForeignKey(e => e.BrokerReportTypeId);
        }
    }
}
