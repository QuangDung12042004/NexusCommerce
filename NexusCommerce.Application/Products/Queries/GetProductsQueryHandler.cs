using MediatR;
using Microsoft.EntityFrameworkCore;
using NexusCommerce.Application.Common.Interfaces;
using NexusCommerce.Application.Common.Models;
using NexusCommerce.Application.Products.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NexusCommerce.Application.Products.Queries
{
    public class GetProductsQueryHandler : IRequestHandler<GetProductsQuery, PagedResult<ProductListDto>>
    {
        private readonly IApplicationDbContext _db;
        public GetProductsQueryHandler(IApplicationDbContext db)
        {
            _db = db;
        }
        public async Task<PagedResult<ProductListDto>> Handle(GetProductsQuery request, CancellationToken cancellationToken)
        {
            IQueryable<Domain.Entities.Product> query = _db.Products.AsNoTracking();
            if (!string.IsNullOrWhiteSpace(request.SearchTerm))
            {
                var term = request.SearchTerm.Trim();
                query = query.Where(p => p.Name.Contains(term) || p.Description.Contains(term));
            }
            if (request.MinPrice.HasValue)
            {
                query = query.Where(p => p.Price >= request.MinPrice.Value);
            }
            if (request.MaxPrice.HasValue)
            {
                query = query.Where(p => p.Price <= request.MaxPrice.Value);
            }
            var totalCount = await query.CountAsync(cancellationToken);
            var items = await query
                .OrderBy(p => p.Name)
                .Skip((request.PageIndex - 1) * request.PageSize)
                .Take(request.PageSize)
                .Select(p => new ProductListDto
                {
                    Id = p.Id,
                    Name = p.Name,
                    Slug = p.Slug,
                    Price = p.Price
                })
                .ToListAsync(cancellationToken);
            return new PagedResult<ProductListDto>(items, request.PageIndex, request.PageSize, totalCount);

        }
    }
}
