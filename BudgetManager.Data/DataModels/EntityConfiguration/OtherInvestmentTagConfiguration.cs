using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BudgetManager.Data.DataModels.EntityConfiguration
{
    public class OtherInvestmentTagConfiguration : IEntityTypeConfiguration<OtherInvestmentTag>
    {
        public void Configure(EntityTypeBuilder<OtherInvestmentTag> builder)
        {
            builder.HasOne<OtherInvestment>(sc => sc.OtherInvestment)
                .WithMany(s => s.OtherInvestmentTags)
                .HasForeignKey(sc => sc.OtherInvestmentId);

            builder.HasOne<Tag>(sc => sc.Tag)
                .WithMany(s => s.OtherInvestmentTags)
                .HasForeignKey(sc => sc.TagId);
        }
    }
}
