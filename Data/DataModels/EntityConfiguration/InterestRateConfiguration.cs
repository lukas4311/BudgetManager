using Microsoft.EntityFrameworkCore;

namespace Data.DataModels.EntityConfiguration
{
    internal static class InterestRateConfiguration
    {
        internal static void ConfigureInterestRate(this ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<InterestRate>()
                .Property(i => i.BankAccountId)
                .IsRequired();

            modelBuilder.Entity<InterestRate>()
               .HasOne(e => e.BankAccount)
               .WithMany(e => e.InterestRates)
               .HasForeignKey(e => e.BankAccountId);
        }
    }
}
