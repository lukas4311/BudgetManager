using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Data.DataModels.EntityConfiguration
{
    internal class InterestRateConfiguration : IEntityTypeConfiguration<InterestRate>
    {
        public void Configure(EntityTypeBuilder<InterestRate> builder)
        {
            builder.Property(i => i.BankAccountId)
                .IsRequired();

            builder.HasOne(e => e.BankAccount)
               .WithMany(e => e.InterestRates)
               .HasForeignKey(e => e.BankAccountId);
        }
    }
}
