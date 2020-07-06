using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace Data.DataModels
{
    public class DataContextDesignTimeFactory : IDesignTimeDbContextFactory<DataContext>
    {
        public DataContext CreateDbContext(string[] args)
        {
            var optionsBuilder = new DbContextOptionsBuilder<DataContext>();
            optionsBuilder.UseSqlServer("Server=(localdb)\\mssqllocaldb;Database=TestDb;Trusted_Connection=True;");

            return new DataContext(optionsBuilder.Options);
        }
    }
}
