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
            modelBuilder.Entity<UserData>()
                .HasOne(p => p.UserIdentity)
                .WithOne(p => p.UserData)
                .HasForeignKey<UserData>(d => d.UserIdentityId);

            modelBuilder.Entity<BankAccount>()
                .HasOne(e => e.UserIdentity)
                .WithMany(e => e.BankAccounts)
                .HasForeignKey(e => e.UserIdentityId);

            modelBuilder.Entity<Payment>()
                .HasOne(e => e.BankAccount)
                .WithMany(e => e.Payments)
                .HasForeignKey(e => e.BankAccountId);

            modelBuilder.Entity<Payment>()
                .HasOne(e => e.PaymentCategory)
                .WithMany(e => e.Payments)
                .HasForeignKey(e => e.PaymentCategoryId);

            modelBuilder.Entity<Payment>()
                .HasOne(e => e.PaymentType)
                .WithMany(e => e.Payments)
                .HasForeignKey(e => e.PaymentTypeId);
        }

        public DbSet<UserIdentity> UserIdentity { get; set; }

        public DbSet<UserData> UserData { get; set; }

        public DbSet<BankAccount> BankAccount { get; set; }

        public DbSet<Payment> Payment { get; set; }

        public DbSet<PaymentType> PaymentType { get; set; }

        public DbSet<PaymentCategory> PaymentCategory { get; set; }
    }
}
