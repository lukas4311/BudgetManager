using BudgetManager.Data.DataModels;
using Microsoft.EntityFrameworkCore;

namespace BudgetManager.Data
{
    public class DataContext : DbContext
    {
        public DataContext(DbContextOptions<DataContext> options) : base(options)
        { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(DataContext).Assembly);
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

        public DbSet<Budget> Budget { get; set; }

        public DbSet<OtherInvestment> OtherInvestment {get; set;}

        public DbSet<ComodityUnit> ComodityUnit { get; set; }

        public DbSet<ComodityType> ComodityType { get; set; }

        public DbSet<ComodityTradeHistory> ComodityTradeHistory { get; set; }

        public DbSet<Address> Address { get; set; }

        public DbSet<CompanyProfile> CompanyProfile { get; set; }
    }
}
