using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NexusCommerce.Application.Products.Dtos
{
    public class ProductDto
    {
        public Guid Id { get; init; }
        public string Name { get; init; } = string.Empty;
        public string Slug { get; init; } = string.Empty;
        public string Description { get; init; } = string.Empty;
        public decimal Price { get; init; }
        public bool IsActive { get; init; }
        public DateTime CreatedAt { get; init; }
        public int? StockQuantity { get; init; }
        public static ProductDto FromEntity(Domain.Entities.Product product)
        {
            return new ProductDto
            {
                Id = product.Id,
                Name = product.Name,
                Slug = product.Slug,
                Description = product.Description,
                Price = product.Price,
                IsActive = product.IsActive,
                CreatedAt = product.CreatedAt,
                StockQuantity = product.Inventory?.StockQuantity
            };
        }
    }
}
