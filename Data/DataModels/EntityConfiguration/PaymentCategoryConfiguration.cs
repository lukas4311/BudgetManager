using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Data.DataModels.EntityConfiguration
{
    internal class PaymentCategoryConfiguration : IEntityTypeConfiguration<PaymentCategory>
    {
        private const int MaxLengthCode = 20;
        private const int MaxLengthName = 100;

        public void Configure(EntityTypeBuilder<PaymentCategory> modelBuilder)
        {
            modelBuilder.Property(b => b.Code)
                .HasMaxLength(MaxLengthCode)
                .IsRequired();

            modelBuilder.Property(b => b.Icon)
                .HasMaxLength(MaxLengthCode)
                .IsRequired();

            modelBuilder.Property(b => b.Name)
                .HasMaxLength(MaxLengthName)
                .IsRequired();
        }
    }
}
