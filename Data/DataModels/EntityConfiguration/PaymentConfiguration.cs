using Microsoft.EntityFrameworkCore;

namespace Data.DataModels.EntityConfiguration
{
    internal static class PaymentConfiguration
    {
        private const int MaxNameLength = 100;

        internal static void ConfigurePayment(this ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Payment>()
                .HasOne(e => e.BankAccount)
                .WithMany(e => e.Payments)
                .HasForeignKey(e => e.BankAccountId);

            modelBuilder.Entity<Payment>()
                .HasOne(e => e.PaymentCategory)
                .WithMany(e => e.Payments)
                .HasForeignKey(e => e.PaymentCategoryId);

            modelBuilder.Entity<Payment>()
                .HasOne(e => e.PaymentType)
                .WithMany(e => e.Payments)
                .HasForeignKey(e => e.PaymentTypeId);

            modelBuilder.Entity<Payment>()
                .Property(p => p.Name)
                .HasMaxLength(MaxNameLength)
                .IsRequired();
        }
    }
}
