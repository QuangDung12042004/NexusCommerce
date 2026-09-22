using NexusCommerce.Domain.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NexusCommerce.Domain.Entities
{
    public class Inventory
    {
        public Guid id { get; private set; }
        public Guid ProductId { get; private set; }
        public int StockQuantity { get; private set; }
        public DateTime UpdateAt { get; private set; }
        public byte[]? RowVersion { get; set; } = null!;
        public Product Product { get; private set; } = null!;
        public Inventory() { }
        public Inventory(Guid productId, int initialStock)
        {
            if (productId == Guid.Empty)
            {
                throw new DomainException("ProductId không được để trống.");
            }
            if (initialStock < 0)
            {
                throw new DomainException("Số lượng tồn kho không được âm.");
            }
            id = Guid.NewGuid();
            ProductId = productId;
            StockQuantity = initialStock;
            UpdateAt = DateTime.UtcNow;
        }
        public void IncreasesStock(int quantity)
        {
            if (quantity <= 0)
            {
                throw new DomainException("Số lượng nhập kho phải lớn hơn 0.");
            }
            StockQuantity += quantity;
            UpdateAt = DateTime.UtcNow;
        }
        public void DecreasesStock(int quantity)
        {
            if (quantity <= 0)
            {
                throw new DomainException("Số lượng xuất kho phải lớn hơn 0.");
            }
            if (quantity > StockQuantity)
            {
                throw new DomainException("Số lượng xuất kho không được vượt quá số lượng tồn kho.");
            }
            StockQuantity -= quantity;
            UpdateAt = DateTime.UtcNow;
        }
    }
}
