using Dreamer.Models.Base;
using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace Dreamer.Models.Main
{
    [Table("OrderShipments", Schema = "Main")]
    public class OrderShipmentSet : BaseEntity
    {
        public string Address { get; set; }
        public DateTimeOffset? ShippedTime { get; set; }
        public DateTimeOffset? DeliveryTime { get; set; }
        public double ShipmentCost { get; set; }

        public OrderSet Order { get; set; }
    }
}
