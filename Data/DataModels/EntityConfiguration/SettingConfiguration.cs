using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BudgetManager.Data.DataModels.EntityConfiguration
{
    public class SettingConfiguration : IEntityTypeConfiguration<Setting>
    {
        private const int MaxCodeLength = 100;

        public void Configure(EntityTypeBuilder<Setting> builder)
        {
            builder.Property(p => p.Code)
                .HasMaxLength(MaxCodeLength)
                .IsRequired();

            builder.Property(p => p.JsonSetting)
                .IsRequired();

            builder.HasIndex(a => a.Code)
                .IsUnique();
        }
    }
}
