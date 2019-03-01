using Dreamer.Models.Base;
using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace Dreamer.Models.Main
{
    [Table("OrderProducts", Schema = "Main")]
    public class OrderProductSet : BaseEntity
    {
        public Guid OrderId { get; set; }
        public OrderSet Order { get; set; }
        public Guid ProductId { get; set; }
        public ProductSet Product { get; set; }
        public double OneItemPrice { get; set; }
        public int Quantity { get; set; }
        /// <summary>
        /// Discount 1->100
        /// </summary>
        public int DiscountPercentage { get; set; }
        public double DiscountValue { get; set; }
        public double NetPrice { get; set; }
    }
}
