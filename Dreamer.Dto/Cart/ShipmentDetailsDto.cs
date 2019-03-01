using System;

namespace Dreamer.Dto.Cart
{
    public class ShipmentDetailsDto
    {
        public string Address { get; set; }
        public DateTimeOffset? DeliveryTime { get; set; }
        public double ShipmentCost { get; set; }
        public DateTimeOffset? ShippedTime { get; set; }
    }
}
