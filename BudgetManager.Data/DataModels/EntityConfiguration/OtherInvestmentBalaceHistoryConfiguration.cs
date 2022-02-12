using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BudgetManager.Data.DataModels.EntityConfiguration
{
    internal class OtherInvestmentBalaceHistoryConfiguration : IEntityTypeConfiguration<OtherInvestmentBalaceHistory>
    {
        public void Configure(EntityTypeBuilder<OtherInvestmentBalaceHistory> builder)
        {
            builder.Property(i => i.Date)
                .IsRequired();

            builder.Property(p => p.Balance)
                .IsRequired();

            builder.HasOne(e => e.OtherInvestment)
                .WithMany(e => e.OtherInvestmentBalaceHistory)
                .HasForeignKey(e => e.OtherInvestmentId);
        }
    }
}
