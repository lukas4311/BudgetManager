﻿using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Data.DataModels.EntityConfiguration
{
    public class CryptoTradeHistoryConfiguration : IEntityTypeConfiguration<CryptoTradeHistory>
    {
        public void Configure(EntityTypeBuilder<CryptoTradeHistory> builder)
        {
            builder.Property(i => i.CryptoTickerId)
                        .IsRequired();

            builder.HasOne(e => e.CryptoTicker)
               .WithMany(e => e.CryptoTradeHistories)
               .HasForeignKey(e => e.CryptoTickerId);
        }
    }
}