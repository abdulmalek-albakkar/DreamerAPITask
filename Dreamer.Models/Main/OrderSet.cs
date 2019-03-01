using Dreamer.Models.Base;
using Dreamer.Models.Enums;
using Dreamer.Models.Security;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace Dreamer.Models.Main
{
    [Table("Orders", Schema = "Main")]
    public class OrderSet : BaseEntity
    {
        public DateTimeOffset PaymentDateTime { get; set; }
        public double NetItemsPrice { get; set; }
        public double NetItemsDiscountValue { get; set; }

        /// <summary>
        /// Total = NetItems + Delivery
        /// </summary>
        public double Total { get; set; }
        public OrderStatus Status { get; set; } = OrderStatus.Paid;
        public int SerialNumber { get; set; }
        public bool EmailNotified { get; set; }
        /// <summary>
        /// When one of the order products is Free, would be enough to make this value true
        /// </summary>
        public bool FreeDelivery { get; set; }

        public Guid DreamerUserId { get; set; }
        public DreamerUserSet DreamerUser { get; set; }
        public Guid OrderShipmentSetId { get; set; }
        public OrderShipmentSet OrderShipment { get; set; }
        public IEnumerable<OrderProductSet> OrderProducts { get; set; }
    }
}
