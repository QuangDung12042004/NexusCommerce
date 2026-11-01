using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NexusCommerce.Domain.Entities;

namespace NexusCommerce.Infrastructure.Persistence.Configurations;

public class InventoryConfiguration : IEntityTypeConfiguration<Inventory>
{
    public void Configure(EntityTypeBuilder<Inventory> builder)
    {
        builder.ToTable("Inventories");

        builder.HasKey(i => i.id);

        // ===== CONCURRENCY TOKEN =====
        // Inventory là nơi xảy ra tranh chấp nhiều nhất (2 người cùng mua),
        // nên RowVersion ở đây CỰC KỲ quan trọng.
        builder.Property(i => i.RowVersion)
            .IsRowVersion();

        // ===== INDEX =====
        // Unique index trên ProductId → mỗi sản phẩm chỉ có 1 dòng inventory.
        builder.HasIndex(i => i.ProductId)
            .IsUnique();

        // Quan hệ ngược về Product đã được cấu hình bên ProductConfiguration,
        // nên ở đây không cần lặp lại (tránh conflict).
    }
}