using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NexusCommerce.Application.Products.Dtos
{
    public class ProductListDto
    {
        public Guid Id { get; init; }
        public string Name { get; init; } = string.Empty;
        public string Slug { get; init; } = string.Empty;
        public decimal Price { get; init; }
        public static ProductListDto FromEntity(Domain.Entities.Product product)
        {
            return new ProductListDto
            {
                Id = product.Id,
                Name = product.Name,
                Slug = product.Slug,
                Price = product.Price
            };
        }
    }
}
