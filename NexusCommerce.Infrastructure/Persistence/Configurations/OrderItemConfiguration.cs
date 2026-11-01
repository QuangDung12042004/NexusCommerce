using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NexusCommerce.Domain.Entities;

namespace NexusCommerce.Infrastructure.Persistence.Configurations;

public class OrderItemConfiguration : IEntityTypeConfiguration<OrderItems>
{
    public void Configure(EntityTypeBuilder<OrderItems> builder)
    {
        builder.ToTable("OrderItems");

        builder.HasKey(oi => oi.Id);

        // ===== PROPERTIES =====
        // SNAPSHOT: lưu tên và giá tại thời điểm mua.
        builder.Property(oi => oi.ProductName)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(oi => oi.UnitPrice)
            .HasColumnType("decimal(18,2)");

        builder.Property(oi => oi.LineTotal)
            .HasColumnType("decimal(18,2)");

        // ===== INDEX =====
        // Index trên ProductId → thống kê "sản phẩm này đã bán bao nhiêu lần".
        builder.HasIndex(oi => oi.ProductID);
    }
}