using NexusCommerce.Domain.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NexusCommerce.Domain.Entities
{
    public class Product
    {
        public Guid Id { get; private set; }
        public string Name { get; private set; } = null!;
        public string Slug { get; private set; } = null!;
        public string Description { get; private set; } = null!;
        public decimal Price { get; private set; }
        public bool IsActive { get; private set; }
        public bool IsDeleted { get; private set; }
        public DateTime CreatedAt { get; private set; }
        public DateTime? UpdatedAt { get; private set; }
        public byte[]? RowVersion { get; set; } = null!;
        public Inventory? Inventory { get; private set; }
        private Product() { }
        public Product(string name, string description, decimal price)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                throw new DomainException("Tên sản phẩm không được để trống.");
            }
            if (price < 0)
            {
                throw new DomainException("Giá sản phẩm không được âm.");

            }
            Id = Guid.NewGuid();
            Name = name.Trim();
            Slug = GenerateSlug(Name);
            Description = description?.Trim() ?? string.Empty;
            Price = price;
            IsActive = true;
            IsDeleted = false;
            CreatedAt = DateTime.UtcNow;
        }
        public void Update(string name, string description)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                throw new DomainException("Tên sản phẩm không được để trống.");
            }
            Name = name.Trim();
            Slug = GenerateSlug(Name);
            Description = description?.Trim() ?? string.Empty;
            UpdatedAt = DateTime.UtcNow;
        }
        public void UpdatePrice(decimal price)
        {
            if (price < 0)
            {
                throw new DomainException("Giá sản phẩm không được âm.");
            }
            Price = price;
            UpdatedAt = DateTime.UtcNow;
        }
        public void Activate()
        {
            IsActive = true;
            UpdatedAt = DateTime.UtcNow;
        }
        public void Deactivate()
        {
            IsActive = false;
            UpdatedAt = DateTime.UtcNow;
        }

        public void SoftDelete()
        {
            IsDeleted = true;
            IsActive = false;
            UpdatedAt = DateTime.UtcNow;
        }
        private static string GenerateSlug(string input)
        {
            return input.Trim().ToLowerInvariant().Replace(' ', '-');
        }
    }
}
