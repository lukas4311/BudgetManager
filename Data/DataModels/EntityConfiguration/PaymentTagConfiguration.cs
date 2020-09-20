using Microsoft.EntityFrameworkCore;

namespace Data.DataModels.EntityConfiguration
{
    internal static class PaymentTagConfiguration
    {
        private const int MaxNameLength = 100;

        internal static void ConfigurePaymentTag(this ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<PaymentTag>()
                .HasOne<Payment>(sc => sc.Payment)
                .WithMany(s => s.PaymentTags)
                .HasForeignKey(sc => sc.PaymentId);

            modelBuilder.Entity<PaymentTag>()
                .HasOne<Tag>(sc => sc.Tag)
                .WithMany(s => s.PaymentTags)
                .HasForeignKey(sc => sc.TagId);
        }
    }
}
