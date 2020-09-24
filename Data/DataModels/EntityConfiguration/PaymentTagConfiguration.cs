using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Data.DataModels.EntityConfiguration
{
    internal class PaymentTagConfiguration : IEntityTypeConfiguration<PaymentTag>
    {
        public void Configure(EntityTypeBuilder<PaymentTag> builder)
        {
            builder.HasOne<Payment>(sc => sc.Payment)
                .WithMany(s => s.PaymentTags)
                .HasForeignKey(sc => sc.PaymentId);

            builder.HasOne<Tag>(sc => sc.Tag)
                .WithMany(s => s.PaymentTags)
                .HasForeignKey(sc => sc.TagId);
        }
    }
}
