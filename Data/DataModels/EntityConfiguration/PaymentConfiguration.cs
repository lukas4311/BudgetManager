using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Data.DataModels.EntityConfiguration
{
    internal class PaymentConfiguration : IEntityTypeConfiguration<Payment>
    {
        private const int MaxNameLength = 100;

        public void Configure(EntityTypeBuilder<Payment> builder)
        {
            builder.HasOne(e => e.BankAccount)
                .WithMany(e => e.Payments)
                .HasForeignKey(e => e.BankAccountId);

            builder.HasOne(e => e.PaymentCategory)
                .WithMany(e => e.Payments)
                .HasForeignKey(e => e.PaymentCategoryId);

            builder.HasOne(e => e.PaymentType)
                .WithMany(e => e.Payments)
                .HasForeignKey(e => e.PaymentTypeId);

            builder.Property(p => p.Name)
                .HasMaxLength(MaxNameLength)
                .IsRequired();
        }
    }
}
