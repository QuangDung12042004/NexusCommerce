using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using NexusCommerce.Application.Common;
using NexusCommerce.Application.Common.Interfaces;
using NexusCommerce.Application.Products.Queries;
using NexusCommerce.Domain.Entities;
using NexusCommerce.Infrastructure.Persistence;

Console.WriteLine("=== DAY 10: GETPRODUCTSQUERY CHẠY THẬT VÀO POSTGRESQL ===\n");

var services = new ServiceCollection();

// Đăng ký DbContext (scoped) với connection string dev local
services.AddDbContext<ApplicationDbContext>(opt =>
    opt.UseNpgsql("Host=localhost;Port=5433;Database=nexus_commerce;Username=nexus_user;Password=nexus_secret_123"));

// Cho handler dùng ApplicationDbContext qua interface
services.AddScoped<IApplicationDbContext>(sp =>
    sp.GetRequiredService<ApplicationDbContext>());

services.AddLogging();

// Đăng ký MediatR (quét assembly Application)
services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(PingQuery).Assembly));

var provider = services.BuildServiceProvider();

// ===== SCOPE 1: SEED DATA NẾU DB TRỐNG =====
using (var scope = provider.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

    if (!await db.Products.AnyAsync())
    {
        Console.WriteLine("DB trống → seed 7 sản phẩm mẫu...");
        var products = new List<Product>();
        for (int i = 1; i <= 7; i++)
        {
            products.Add(new Product($"San pham test {i}", $"Mo ta cho san pham {i}", 100000m * i));
        }
        db.Products.AddRange(products);
        await db.SaveChangesAsync();

        foreach (var p in products)
            db.Inventories.Add(new Inventory(p.Id, 10));
        await db.SaveChangesAsync();

        Console.WriteLine("✅ Seed xong 7 sản phẩm + inventory.");
    }
    else
    {
        Console.WriteLine("DB đã có dữ liệu → bỏ qua seed.");
    }
}

// ===== SCOPE 2: GỬI QUERY =====
using (var scope = provider.CreateScope())
{
    var sender = scope.ServiceProvider.GetRequiredService<ISender>();

    // TEST 1: Trang 1, mỗi trang 3
    var page1 = await sender.Send(new GetProductsQuery { PageIndex = 1, PageSize = 3 });
    Console.WriteLine($"\n[Trang {page1.PageIndex}/{page1.TotalPages}] Tổng: {page1.TotalCount} | HasNext: {page1.HasNextPage}");
    foreach (var item in page1.Items)
        Console.WriteLine($"   - {item.Name} | {item.Price}");

    // TEST 2: Trang 2
    var page2 = await sender.Send(new GetProductsQuery { PageIndex = 2, PageSize = 3 });
    Console.WriteLine($"\n[Trang {page2.PageIndex}/{page2.TotalPages}] Tổng: {page2.TotalCount} | HasNext: {page2.HasNextPage}");
    foreach (var item in page2.Items)
        Console.WriteLine($"   - {item.Name} | {item.Price}");

    // TEST 3: Lọc giá >= 500000
    var filtered = await sender.Send(new GetProductsQuery { MinPrice = 500000m, PageSize = 10 });
    Console.WriteLine($"\n[Lọc MinPrice >= 500000] Tổng thỏa: {filtered.TotalCount}");
    foreach (var item in filtered.Items)
        Console.WriteLine($"   - {item.Name} | {item.Price}");

    // TEST 4: Tìm kiếm theo từ khóa
    var searched = await sender.Send(new GetProductsQuery { SearchTerm = "test 5", PageSize = 10 });
    Console.WriteLine($"\n[Tìm 'test 5'] Tổng thỏa: {searched.TotalCount}");
    foreach (var item in searched.Items)
        Console.WriteLine($"   - {item.Name} | {item.Price}");
}

Console.WriteLine("\n=== HOÀN TẤT DAY 10 ===");