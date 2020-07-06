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

            modelBuilder.Entity<BankAccount>()
                .HasOne(e => e.UserData)
                .WithMany(e => e.BankAccounts)
                .HasForeignKey(e => e.UserDataId);

            modelBuilder.Entity<Payment>()
                .HasOne(e => e.BankAccount)
                .WithMany(e => e.Payments)
                .HasForeignKey(e => e.BankAccountId);
        }

        public DbSet<UserIdentity> UserIdentity { get; set; }

        public DbSet<UserData> UserData { get; set; }

        public DbSet<BankAccount> BankAccount { get; set; }
    }
}
