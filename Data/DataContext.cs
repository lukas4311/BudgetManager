using Data.DataModels;
using Data.DataModels.EntityConfiguration;
using Microsoft.EntityFrameworkCore;

namespace Data
{
    public class DataContext : DbContext
    {
        public DataContext(DbContextOptions<DataContext> options) : base(options)
        { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(DataContext).Assembly);

            //modelBuilder.ConfigureBankAccount();
            //modelBuilder.ConfigurePaymentCategory();
            //modelBuilder.ConfigurePayment();
            //modelBuilder.ConfigureInterestRate();
            //modelBuilder.ConfigurePaymentTag();
            //modelBuilder.ConfigureUserData();
            //modelBuilder.ConfigureTax();
        }

        public DbSet<UserIdentity> UserIdentity { get; set; }

        public DbSet<UserData> UserData { get; set; }

        public DbSet<InterestRate> BankAccount { get; set; }

        public DbSet<Payment> Payment { get; set; }

        public DbSet<PaymentType> PaymentType { get; set; }

        public DbSet<PaymentCategory> PaymentCategory { get; set; }

        public DbSet<TaxSetting> TaxSetting { get; set; }

        public DbSet<InterestRate> InterestRate { get; set; }

        public DbSet<Tag> Tag { get; set; }

        public DbSet<PaymentTag> PaymentTag { get; set; }
    }
}
