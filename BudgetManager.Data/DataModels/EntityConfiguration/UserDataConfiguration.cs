using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BudgetManager.Data.DataModels.EntityConfiguration
{
    internal class UserDataConfiguration : IEntityTypeConfiguration<UserData>
    {
        private const int NameMaxLenght = 100;

        public void Configure(EntityTypeBuilder<UserData> builder)
        {
            builder.Property(u => u.FirstName)
                    .HasMaxLength(NameMaxLenght)
                    .IsRequired();

            builder.Property(u => u.LastName)
                .HasMaxLength(NameMaxLenght)
                .IsRequired();

            builder.Property(u => u.Phone)
                .HasMaxLength(10)
                .IsFixedLength();

            builder.HasOne(p => p.UserIdentity)
                .WithOne(p => p.UserData)
                .HasForeignKey<UserData>(d => d.UserIdentityId);
        }
    }
}
