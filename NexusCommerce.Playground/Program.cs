using MediatR;
using Microsoft.Extensions.DependencyInjection;
using NexusCommerce.Application.Common;
using NexusCommerce.Domain.Entities;
using NexusCommerce.Domain.Exceptions;

Console.WriteLine("=== TEST RICH DOMAIN MODEL ===\n");

// TEST 1: Tạo sản phẩm hợp lệ
var product = new Product("Áo Thun Nam", "Áo thun cotton cao cấp", 199000);
Console.WriteLine($"✅ Tạo thành công: {product.Name} | Slug: {product.Slug} | Giá: {product.Price}");

// TEST 2: Tạo sản phẩm với giá ÂM → phải ném exception
try
{
    var badProduct = new Product("Quần Jean", "Mô tả", -5000);
    Console.WriteLine("❌ LỖI: Đáng lẽ phải ném exception!");
}
catch (DomainException ex)
{
    Console.WriteLine($"✅ Chặn được giá âm: {ex.Message}");
}

// TEST 3: Không thể gán giá trực tiếp (bỏ comment dòng dưới → sẽ COMPILE ERROR)
// product.Price = -999;  // ← Bỏ comment để thấy lỗi compile. Đây là sức mạnh của private set!

// TEST 4: Đổi giá qua method hợp lệ
product.UpdatePrice(250000);
Console.WriteLine($"✅ Đổi giá qua method: {product.Price}");

// TEST 5: Inventory không cho bán quá số lượng
var inventory = new Inventory(product.Id, 5);
try
{
    inventory.DecreasesStock(10);  // Chỉ có 5 mà đòi bán 10
}
catch (DomainException ex)
{
    Console.WriteLine($"✅ Chặn được bán quá số lượng: {ex.Message}");
}

Console.WriteLine("\n=== TẤT CẢ TEST HOÀN TẤT ===");

Console.WriteLine("\n=== SMOKE TEST MEDIATR ===\n");

// 1. Tạo "container" chứa các service
var services = new ServiceCollection();

// Đăng ký logging mà MediatR cần dùng
services.AddLogging();

// 2. Đăng ký MediatR: quét assembly chứa PingQuery (= Application assembly)
//    → tự động tìm và đăng ký MỌI Handler trong assembly đó
services.AddMediatR(cfg =>
    cfg.RegisterServicesFromAssembly(typeof(PingQuery).Assembly));

// 3. Build container → thành Service Provider
var provider = services.BuildServiceProvider();

// 4. Lấy Mediator ra
var sender = provider.GetRequiredService<ISender>();

// 5. Gửi request → Mediator tự route tới PingQueryHandler
var result = await sender.Send(new PingQuery());

Console.WriteLine($"✅ Mediator trả về: {result}");
Console.WriteLine("\n=== SMOKE TEST HOÀN TẤT ===");