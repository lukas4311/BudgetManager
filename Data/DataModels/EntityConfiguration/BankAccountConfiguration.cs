using Microsoft.EntityFrameworkCore;

namespace Data.DataModels.EntityConfiguration
{
    internal static class BankAccountConfigurationExtension
    {
        private const int MaxLengthCode = 20;

        internal static void ConfigureBankAccount(this ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<BankAccount>()
                .HasOne(e => e.UserIdentity)
                .WithMany(e => e.BankAccounts)
                .HasForeignKey(e => e.UserIdentityId);

            modelBuilder.Entity<BankAccount>()
                .Property(b => b.Code)
                .HasMaxLength(MaxLengthCode)
                .IsRequired();

            modelBuilder.Entity<BankAccount>()
                .Property(b => b.UserIdentityId)
                .HasDefaultValue();
        }
    }
}
