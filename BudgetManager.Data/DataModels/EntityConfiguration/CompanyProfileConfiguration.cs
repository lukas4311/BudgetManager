using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BudgetManager.Data.DataModels.EntityConfiguration
{
    public class CompanyProfileConfiguration : IEntityTypeConfiguration<CompanyProfile>
    {
        public void Configure(EntityTypeBuilder<CompanyProfile> builder)
        {
            builder.Property(p => p.Symbol).HasMaxLength(10).IsRequired();
            builder.Property(p => p.Currency).HasMaxLength(10).IsRequired();
            builder.Property(p => p.CompanyName).HasMaxLength(50).IsRequired();

            builder.HasOne<Address>().WithMany().HasForeignKey(p => p.AddressId);
        }
    }
}
