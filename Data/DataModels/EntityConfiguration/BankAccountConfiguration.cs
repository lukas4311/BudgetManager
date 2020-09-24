using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Data.DataModels.EntityConfiguration
{
    internal class BankAccountConfiguration : IEntityTypeConfiguration<BankAccount>
    {
        private const int MaxLengthCode = 20;

        public void Configure(EntityTypeBuilder<BankAccount> builder)
        {
            builder.HasOne(e => e.UserIdentity)
                .WithMany(e => e.BankAccounts)
                .HasForeignKey(e => e.UserIdentityId);

            builder.Property(b => b.Code)
                .HasMaxLength(MaxLengthCode)
                .IsRequired();

            builder.Property(b => b.UserIdentityId)
                .HasDefaultValue();
        }
    }
}
