using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BudgetManager.Data.DataModels.EntityConfiguration;

public class NotificationConfiguration : IEntityTypeConfiguration<Notification>
{
    public void Configure(EntityTypeBuilder<Notification> builder)
    {
        builder.ToTable("UserNotifications");

        builder.HasKey(n => n.Id);
        builder.Property(n => n.Id).HasColumnName("NotificationId").IsRequired().ValueGeneratedOnAdd();

        builder.Property(n => n.UserId).HasColumnName("UserId").IsRequired();
        builder.Property(n => n.Heading).HasColumnName("Heading").IsRequired();
        builder.Property(n => n.Content).HasColumnName("Content").IsRequired();
        builder.Property(n => n.IsDisplayed).HasColumnName("IsDisplayed").IsRequired().HasDefaultValue(false);
        builder.Property(n => n.Timestamp).HasColumnName("Timestamp").IsRequired();
        builder.Property(n => n.AttachmentUrl).HasColumnName("AttachmentUrl");
        // You might want to configure further properties here

        // Configure the relationship with UserIdentity
        builder.HasOne(n => n.User)
            .WithMany(u => u.Notifications)
            .HasForeignKey(n => n.UserId)
            .IsRequired()
            .OnDelete(DeleteBehavior.Cascade);
    }
}