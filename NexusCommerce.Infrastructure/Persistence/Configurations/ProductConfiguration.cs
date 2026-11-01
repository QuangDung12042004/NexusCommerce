using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NexusCommerce.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace NexusCommerce.Infrastructure.Persistence.Configurations
{
    public class ProductConfiguration : IEntityTypeConfiguration<Product>
    {
        public void Configure(EntityTypeBuilder<Product> builder)
        {
            // ===== TABLE NAME =====
            builder.ToTable("Products");

            // ===== PRIMARY KEY =====
            builder.HasKey(p => p.Id);

            // ===== PROPERTIES =====
            builder.Property(p => p.Name)
                .IsRequired()
                .HasMaxLength(200);

            builder.Property(p => p.Slug)
                .IsRequired()
                .HasMaxLength(250);

            builder.Property(p => p.Description)
                .HasMaxLength(2000);

            // Tiền bạc PHẢI dùng decimal với độ chính xác cụ thể.
            // decimal(18,2): tối đa 18 chữ số, 2 chữ số thập phân.
            builder.Property(p => p.Price)
                .HasColumnType("decimal(18,2)");

            // ===== CONCURRENCY TOKEN =====
            // RowVersion dùng cho Optimistic Concurrency.
            // PostgreSQL dùng xmin, nhưng ta cấu hình IsRowVersion để EF tự xử lý.
            builder.Property(p => p.RowVersion)
                .IsConcurrencyToken();

            // ===== INDEXES =====
            // Unique index trên Slug → không trùng slug, tìm kiếm nhanh.
            builder.HasIndex(p => p.Slug)
                .IsUnique();

            // Index trên IsActive + IsDeleted → tăng tốc query lọc sản phẩm hiển thị.
            builder.HasIndex(p => new { p.IsActive, p.IsDeleted });

            // ===== RELATIONSHIP: Product - Inventory (1-1) =====
            // Một Product có một Inventory.
            builder.HasOne(p => p.Inventory)
                .WithOne(i => i.Product)
                .HasForeignKey<Inventory>(i => i.ProductId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
