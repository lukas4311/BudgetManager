using Microsoft.EntityFrameworkCore;

namespace Data.DataModels.EntityConfiguration
{
    internal static class UserDataConfiguration
    {
        private const int NameMaxLenght = 100;

        internal static void ConfigureUserData(this ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<UserData>()
                .Property(u => u.FirstName)
                .HasMaxLength(NameMaxLenght)
                .IsRequired();

            modelBuilder.Entity<UserData>()
                .Property(u => u.LastName)
                .HasMaxLength(NameMaxLenght)
                .IsRequired();

            modelBuilder.Entity<UserData>()
                .Property(u => u.Phone)
                .HasMaxLength(9)
                .IsFixedLength()
                .IsRequired();

            modelBuilder.Entity<UserData>()
                .HasOne(p => p.UserIdentity)
                .WithOne(p => p.UserData)
                .HasForeignKey<UserData>(d => d.UserIdentityId);
        }
    }
}
