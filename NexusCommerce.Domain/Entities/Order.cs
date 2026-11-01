using NexusCommerce.Domain.Enums;
using NexusCommerce.Domain.Exceptions;

namespace NexusCommerce.Domain.Entities;

/// <summary>
/// Entity đại diện cho một đơn hàng.
/// Là AGGREGATE ROOT: quản lý collection OrderItem và trạng thái đơn hàng.
/// </summary>
public class Order
{
    // ===== PROPERTIES =====

    public Guid Id { get; private set; }

    /// <summary>
    /// Mã đơn hàng dễ đọc cho con người (VD: NX20260922-ABC1).
    /// Thực tế nên do service generate để đảm bảo unique + sequential.
    /// </summary>
    public string OrderCode { get; private set; } = null!;

    public string CustomerName { get; private set; } = null!;
    public string ShippingAddress { get; private set; } = null!;
    public string? Note { get; private set; }

    /// <summary>
    /// Tổng tiền hàng (chưa gồm giảm giá, phí ship).
    /// </summary>
    public decimal Subtotal { get; private set; }

    /// <summary>
    /// Số tiền được giảm.
    /// </summary>
    public decimal DiscountAmount { get; private set; }

    /// <summary>
    /// Phí vận chuyển.
    /// </summary>
    public decimal ShippingFee { get; private set; }

    /// <summary>
    /// Tổng cuối cùng khách phải trả.
    /// Công thức: Subtotal - DiscountAmount + ShippingFee.
    /// Được LƯU LẠI (không tính dynamic) để đảm bảo nhất quán.
    /// </summary>
    public decimal TotalAmount { get; private set; }

    public OrderStatus Status { get; private set; }

    public DateTime CreatedAt { get; private set; }
    public DateTime? UpdatedAt { get; private set; }

    /// <summary>
    /// RowVersion cho optimistic concurrency.
    /// </summary>
    public byte[] RowVersion { get; set; } = null!;


    // ===== COLLECTION: ENCAPSULATED =====

    /// <summary>
    /// List THẬT — giấu kín, chỉ mình Order được sửa.
    /// </summary>
    private readonly List<OrderItems> _items = new();

    /// <summary>
    /// View CHỈ ĐỌC cho bên ngoài. Không thể Add/Remove/Clear.
    /// </summary>
    public IReadOnlyCollection<OrderItems> Items => _items.AsReadOnly();


    // ===== CONSTRUCTORS =====

    /// <summary>
    /// Constructor rỗng cho EF Core.
    /// </summary>
    private Order() { }

    /// <summary>
    /// Tạo đơn hàng mới. Mặc định ở trạng thái Pending.
    /// </summary>
    public Order(string customerName, string shippingAddress, string? note = null)
    {
        if (string.IsNullOrWhiteSpace(customerName))
            throw new DomainException("Tên khách hàng không được để trống.");

        if (string.IsNullOrWhiteSpace(shippingAddress))
            throw new DomainException("Địa chỉ giao hàng không được để trống.");

        Id = Guid.NewGuid();
        OrderCode = GenerateOrderCode();
        CustomerName = customerName.Trim();
        ShippingAddress = shippingAddress.Trim();
        Note = note;
        Status = OrderStatus.Pending;  // Đơn mới luôn bắt đầu ở Pending
        DiscountAmount = 0;
        ShippingFee = 0;
        CreatedAt = DateTime.UtcNow;
    }


    // ===== BEHAVIOR: QUẢN LÝ ITEMS =====

    /// <summary>
    /// THÊM một sản phẩm vào đơn hàng.
    /// Đây là cách DUY NHẤT để thêm item (bên ngoài không chạm được _items).
    /// Áp dụng SNAPSHOT: nhận productName và unitPrice tại thời điểm gọi.
    /// </summary>
    public void AddItem(Guid productId, string productName, decimal unitPrice, int quantity)
    {
        // Chỉ được thêm item khi đơn chưa bị hủy/giao
        if (Status == OrderStatus.Canceled)
            throw new DomainException("Không thể thêm item vào đơn đã hủy.");

        if (Status == OrderStatus.Shipper || Status == OrderStatus.Delivered)
            throw new DomainException("Không thể sửa đơn đã giao.");

        var newItem = new OrderItems(productId, productName, unitPrice, quantity);
        _items.Add(newItem);
        RecalculateTotal();  // Luôn tính lại tổng sau khi thay đổi items
    }


    // ===== BEHAVIOR: THANH TOÁN & PHÍ =====

    /// <summary>
    /// Áp dụng giảm giá. Kiểm tra không vượt quá tiền hàng.
    /// </summary>
    public void ApplyDiscount(decimal amount)
    {
        if (amount < 0)
            throw new DomainException("Số tiền giảm giá không được âm.");

        if (amount > Subtotal)
            throw new DomainException("Giảm giá không được vượt quá tiền hàng.");

        DiscountAmount = amount;
        RecalculateTotal();
    }

    /// <summary>
    /// Set phí vận chuyển.
    /// </summary>
    public void SetShippingFee(decimal fee)
    {
        if (fee < 0)
            throw new DomainException("Phí vận chuyển không được âm.");

        ShippingFee = fee;
        RecalculateTotal();
    }


    // ===== BEHAVIOR: TRẠNG THÁI (STATE TRANSITIONS) =====

    /// <summary>
    /// Đánh dấu đã thanh toán. Chỉ hợp lệ khi đang Pending.
    /// </summary>
    public void MarkAsPaid()
    {
        if (Status != OrderStatus.Pending)
            throw new DomainException($"Chỉ đơn Pending mới được thanh toán. Trạng thái hiện tại: {Status}.");

        Status = OrderStatus.Paid;
        UpdatedAt = DateTime.UtcNow;
    }

    /// <summary>
    /// Bàn giao cho vận chuyển. Chỉ hợp lệ khi đã Paid.
    /// </summary>
    public void Ship()
    {
        if (Status != OrderStatus.Paid)
            throw new DomainException($"Chỉ đơn đã thanh toán mới được giao. Trạng thái hiện tại: {Status}.");

        Status = OrderStatus.Shipper;
        UpdatedAt = DateTime.UtcNow;
    }

    /// <summary>
    /// Xác nhận đã giao thành công.
    /// </summary>
    public void Deliver()
    {
        if (Status != OrderStatus.Shipper)
            throw new DomainException($"Chỉ đơn đang giao mới được xác nhận hoàn tất. Trạng thái hiện tại: {Status}.");

        Status = OrderStatus.Delivered;
        UpdatedAt = DateTime.UtcNow;
    }

    /// <summary>
    /// Hủy đơn. Chỉ được hủy khi chưa giao.
    /// </summary>
    public void Cancel()
    {
        if (Status == OrderStatus.Shipper || Status == OrderStatus.Delivered)
            throw new DomainException("Không thể hủy đơn đã giao.");

        Status = OrderStatus.Canceled;
        UpdatedAt = DateTime.UtcNow;
    }


    // ===== PRIVATE HELPERS =====

    /// <summary>
    /// Tính lại tổng tiền từ items + discount + shipping.
    /// Private vì chỉ Order mới được quyền tính lại.
    /// </summary>
    private void RecalculateTotal()
    {
        Subtotal = _items.Sum(item => item.LineTotal);
        TotalAmount = Subtotal - DiscountAmount + ShippingFee;
    }

    /// <summary>
    /// Sinh mã đơn hàng dễ đọc.
    /// Thực tế nên dùng service để đảm bảo unique tuyệt đối.
    /// </summary>
    private static string GenerateOrderCode()
    {
        var timestamp = DateTime.UtcNow.ToString("yyyyMMddHHmmss");
        var random = Guid.NewGuid().ToString("N")[..4].ToUpper();
        return $"NX{timestamp}-{random}";
    }
}