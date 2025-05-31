using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace BudgetManager.Data.DataModels
{
    public class DataContextDesignTimeFactory : IDesignTimeDbContextFactory<DataContext>
    {
        public DataContext CreateDbContext(string[] args)
        {
            var optionsBuilder = new DbContextOptionsBuilder<DataContext>();
            optionsBuilder.UseSqlServer("Server=NTB-DAK-164-700\\DEV;Database=BudgetManager;Trusted_Connection=True;TrustServerCertificate=True;");

            return new DataContext(optionsBuilder.Options);
        }
    }
}
