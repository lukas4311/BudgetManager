﻿using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BudgetManager.Data.DataModels.EntityConfiguration
{
    internal class EnumItemTypeConfiguration : IEntityTypeConfiguration<EnumItemType>
    {
        private const int MaxLengthCode = 40;
        private const int MaxLengthName = 100;

        public void Configure(EntityTypeBuilder<EnumItemType> modelBuilder)
        {
            modelBuilder.Property(b => b.Code)
                .HasMaxLength(MaxLengthCode)
                .IsRequired();

            modelBuilder.Property(b => b.Name)
                .HasMaxLength(MaxLengthName)
                .IsRequired();

            modelBuilder.HasIndex(b => b.Code)
                .IsUnique();
        }
    }
}
