using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NexusCommerce.Domain.Entities;

namespace NexusCommerce.Infrastructure.Persistence.Configurations;

public class OrderConfiguration : IEntityTypeConfiguration<Order>
{
    public void Configure(EntityTypeBuilder<Order> builder)
    {
        builder.ToTable("Orders");

        builder.HasKey(o => o.Id);

        // ===== PROPERTIES =====
        builder.Property(o => o.OrderCode)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(o => o.CustomerName)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(o => o.ShippingAddress)
            .IsRequired()
            .HasMaxLength(500);

        builder.Property(o => o.Note)
            .HasMaxLength(1000);

        // Tất cả cột tiền đều dùng decimal(18,2)
        builder.Property(o => o.Subtotal)
            .HasColumnType("decimal(18,2)");

        builder.Property(o => o.DiscountAmount)
            .HasColumnType("decimal(18,2)");

        builder.Property(o => o.ShippingFee)
            .HasColumnType("decimal(18,2)");

        builder.Property(o => o.TotalAmount)
            .HasColumnType("decimal(18,2)");

        // ===== ENUM =====
        // Lưu OrderStatus dưới dạng string để dễ đọc trong DB,
        // thay vì số (0,1,2...) khó hiểu khi xem data.
        builder.Property(o => o.Status)
            .HasConversion<string>()
            .HasMaxLength(20);

        // ===== CONCURRENCY TOKEN =====
        builder.Property(o => o.RowVersion)
            .IsRowVersion();

        // ===== INDEXES =====
        // Unique index trên OrderCode → tra cứu đơn nhanh, không trùng mã.
        builder.HasIndex(o => o.OrderCode)
            .IsUnique();

        // Index trên Status + CreatedAt → admin lọc đơn theo trạng thái + mới nhất.
        builder.HasIndex(o => new { o.Status, o.CreatedAt });

        // ===== RELATIONSHIP: Order - OrderItem (1-n) =====
        // Một Order có nhiều OrderItem.
        // _items là private field, EF cần cấu hình qua navigation.
        builder.HasMany(o => o.Items)
            .WithOne()
            .HasForeignKey("OrderId")
            .OnDelete(DeleteBehavior.Cascade);
    }
}