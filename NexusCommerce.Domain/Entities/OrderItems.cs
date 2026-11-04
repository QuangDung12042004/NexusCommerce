using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NexusCommerce.Domain.Exceptions;

namespace NexusCommerce.Domain.Entities
{
    public class OrderItems
    {
        public Guid Id { get; private set; }
        public Guid ProductID { get; private set; }
        public string ProductName { get; private set; } = string.Empty;
        public decimal UnitPrice { get; private set; }
        public int Quantity { get; private set; }
        public decimal LineTotal { get; private set; }

        private OrderItems() { }

        /// <summary>
        /// Tạo một dòng đơn hàng.
        /// Lưu ý: KHÔNG cần OrderId ở đây — OrderItem thuộc về Order,
        /// Order quản lý collection. EF Core sẽ tự map foreign key khi lưu.
        /// </summary>
        public OrderItems(Guid productId, string productName, decimal unitPrice, int quantity)
        {
            if (productId == Guid.Empty)
                throw new DomainException("ProductId không được để trống.");

            if (string.IsNullOrWhiteSpace(productName))
                throw new DomainException("Tên sản phẩm không được để trống.");

            if (unitPrice < 0)
                throw new DomainException("Đơn giá không được để âm.");

            if (quantity <= 0)
                throw new DomainException("Số lượng phải lớn hơn 0.");

            Id = Guid.NewGuid();
            ProductID = productId;
            ProductName = productName.Trim();
            UnitPrice = unitPrice;
            Quantity = quantity;
            LineTotal = unitPrice * quantity;
        }

        internal void IncreaseQuantity(int additionalQuantity)
        {
            if (additionalQuantity <= 0)
                throw new DomainException("Số lượng thêm phải lớn hơn 0.");

            Quantity += additionalQuantity;
            LineTotal = UnitPrice * Quantity;
        }
    }
}
