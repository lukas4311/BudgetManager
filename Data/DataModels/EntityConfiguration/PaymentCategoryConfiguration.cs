using Microsoft.EntityFrameworkCore;

namespace Data.DataModels.EntityConfiguration
{
    internal static class PaymentCategoryConfiguration
    {
        private const int MaxLengthCode = 20;
        private const int MaxLengthName = 100;

        internal static void ConfigurePaymentCategory (this ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<PaymentCategory>()
                .Property(b => b.Code)
                .HasMaxLength(MaxLengthCode)
                .IsRequired();

            modelBuilder.Entity<PaymentCategory>()
                .Property(b => b.Icon)
                .HasMaxLength(MaxLengthCode)
                .IsRequired();

            modelBuilder.Entity<PaymentCategory>()
                .Property(b => b.Name)
                .HasMaxLength(MaxLengthName)
                .IsRequired();
        }
    }
}
