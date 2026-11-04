using NexusCommerce.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;

namespace NexusCommerce.Application.Orders.Dtos
{
    public class OrderDto
    {
        public Guid Id { get; init; }
        public string OrderCode { get; init; } = string.Empty;
        public string CustomerName { get; init; } = string.Empty;
        public string ShippingAddress { get; init; } = string.Empty;
        public decimal Subtotal { get; init; }
        public decimal DiscountAmount { get; init; }
        public decimal ShippingFee { get; init; }
        public decimal TotalAmount { get; init; }
        public OrderStatus Status { get; init; }
        public DateTime CreatedAt { get; init; }
        public IReadOnlyCollection<OrderItemDto> OrderItems { get; init; } =
            Array.Empty<OrderItemDto>();

        public static OrderDto FromEntity(Domain.Entities.Order order)
        {
            return new OrderDto
            {
                Id = order.Id,
                OrderCode = order.OrderCode,
                CustomerName = order.CustomerName,
                ShippingAddress = order.ShippingAddress,
                Subtotal = order.Subtotal,
                DiscountAmount = order.DiscountAmount,
                ShippingFee = order.ShippingFee,
                TotalAmount = order.TotalAmount,
                Status = order.Status,
                CreatedAt = order.CreatedAt,
                OrderItems = order.Items
                    .Select(OrderItemDto.FromEntity)
                    .ToList()
            };
        }
    }
}
