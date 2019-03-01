using Dreamer.Models.Enums;
using System;
using System.Collections.Generic;

namespace Dreamer.Dto.Cart
{
    public class DetailedOrderDto
    {
        public Guid Id { get; set; }
        public double NetItemsPrice { get; set; }
        public double NetItemsDiscountValue { get; set; }
        public double Total { get; set; }
        public OrderStatus Status { get; set; } = OrderStatus.Paid;
        public int SerialNumber { get; set; }
        public bool EmailNotified { get; set; }
        public bool FreeDelivery { get; set; }

        public ShipmentDetailsDto OrderShipmentDetails { get; set; }
        public IEnumerable<OrderProductDto> OrderProducts { get; set; }
        public DateTimeOffset PaymentDateTime { get; set; }
    }
}
