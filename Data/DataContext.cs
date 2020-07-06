using Data.DataModels;
using Microsoft.EntityFrameworkCore;

namespace Data
{
    public class DataContext : DbContext
    {
        public DataContext(DbContextOptions<DataContext> options) : base(options)
        { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<UserIdentity>()
                .HasOne(p => p.UserData)
                .WithOne(p => p.UserIdentity)
                .HasForeignKey<UserData>(d => d.UserIdentityId);

            modelBuilder.Entity<UserData>()
                .HasOne(p => p.UserIdentity)
                .WithOne(p => p.UserData)
                .HasForeignKey<UserIdentity>(d => d.UserDataId);
        }

        public DbSet<UserIdentity> UserIdentity { get; set; }

        public DbSet<UserData> UserData { get; set; }
    }
}
