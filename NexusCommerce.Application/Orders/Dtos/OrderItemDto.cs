using System;

namespace NexusCommerce.Application.Orders.Dtos
{
    public class OrderItemDto
    {
        public Guid ProductId { get; init; }
        public string ProductName { get; init; } = string.Empty;
        public decimal UnitPrice { get; init; }
        public int Quantity { get; init; }
        public decimal LineTotal { get; init; }

        public static OrderItemDto FromEntity(Domain.Entities.OrderItems orderItem)
        {
            return new OrderItemDto
            {
                ProductId = orderItem.ProductID,
                ProductName = orderItem.ProductName,
                UnitPrice = orderItem.UnitPrice,
                Quantity = orderItem.Quantity,
                LineTotal = orderItem.LineTotal
            };
        }
    }
}
