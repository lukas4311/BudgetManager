using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BudgetManager.Data.DataModels.EntityConfiguration
{
    internal class OtherInvestmentConfiguration : IEntityTypeConfiguration<OtherInvestment>
    {
        public void Configure(EntityTypeBuilder<OtherInvestment> builder)
        {
            builder.Property(i => i.Code)
                .IsRequired();

            builder.HasIndex(p => p.Code)
                .IsUnique();

            builder.Property(i => i.Created)
                .IsRequired();

            builder.Property(p => p.OpeningBalance)
                .IsRequired();

            builder.HasOne(e => e.UserIdentity)
                .WithMany(e => e.OtherInvestments)
                .HasForeignKey(e => e.UserIdentityId);
        }
    }
}
