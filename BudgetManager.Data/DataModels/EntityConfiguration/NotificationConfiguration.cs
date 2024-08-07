using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BudgetManager.Data.DataModels.EntityConfiguration;

public class NotificationConfiguration : IEntityTypeConfiguration<Notification>
{
    public void Configure(EntityTypeBuilder<Notification> builder)
    {
        builder.Property(n => n.UserIdentityId).HasColumnName("UserIdentityId").IsRequired();
        builder.Property(n => n.Heading).HasColumnName("Heading").IsRequired();
        builder.Property(n => n.Content).HasColumnName("Content").IsRequired();
        builder.Property(n => n.IsDisplayed).HasColumnName("IsDisplayed").IsRequired().HasDefaultValue(false);
        builder.Property(n => n.Timestamp).HasColumnName("Timestamp").IsRequired();
        builder.Property(n => n.AttachmentUrl).HasColumnName("AttachmentUrl");
        // You might want to configure further properties here

        // Configure the relationship with UserIdentity
        builder.HasOne(n => n.UserIdentity)
            .WithMany(u => u.Notifications)
            .HasForeignKey(n => n.UserIdentityId)
            .IsRequired()
            .OnDelete(DeleteBehavior.Cascade);
    }
}