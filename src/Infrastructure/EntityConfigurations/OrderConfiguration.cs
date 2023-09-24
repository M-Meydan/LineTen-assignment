using Domain.Entities;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore.ValueGeneration;

namespace Infrastructure.EntityConfigurations
{
    public class OrderConfiguration : IEntityTypeConfiguration<Order>
    {
        public void Configure(EntityTypeBuilder<Order> builder)
        {
            builder.HasKey(x => x.Id);
            builder.Property(x => x.Id)
                    .HasColumnOrder(1)
                    .ValueGeneratedOnAdd()
                    .HasValueGenerator<SequentialGuidValueGenerator>();

            builder.Property(o => o.ProductId)
                .HasColumnOrder(2)
                .IsRequired();

            builder.Property(o => o.CustomerId)
                .HasColumnOrder(3)
                .IsRequired();

            builder.Property(o => o.Status)
                .HasColumnOrder(4)
                .IsRequired();

            builder.Property(o => o.CreatedDate)
                .HasColumnOrder(5)
                .IsRequired()
                .HasDefaultValueSql("GETUTCDATE()");

            builder.Property(o => o.UpdatedDate)
                .HasColumnOrder(6)
                .IsRequired()
                .HasDefaultValueSql("GETUTCDATE()");

            builder.HasOne(o => o.Product)
                .WithMany()
                .HasForeignKey(o => o.ProductId);

            builder.HasOne(o => o.Customer)
                .WithMany()
                .HasForeignKey(o => o.CustomerId);
        }
    }
}
