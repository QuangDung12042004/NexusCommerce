using MediatR;
using NexusCommerce.Application.Common.Models;
using NexusCommerce.Application.Products.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NexusCommerce.Application.Products.Queries
{
    public record GetProductsQuery : IRequest<PagedResult<ProductListDto>>
    {
        public int PageIndex { get; init; } = 1;
        public int PageSize { get; init; } = 10;
        public string? SearchTerm { get; init; }
        public decimal? MinPrice { get; init; }
        public decimal? MaxPrice { get; init; }
    }
}
