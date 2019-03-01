using Dreamer.Models.Base;
using Dreamer.Models.Security;
using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace Dreamer.Models.Main
{
    [Table("CartItems", Schema = "Main")]
    public class CartItemSet : BaseEntity
    {
        public Guid ProductId { get; set; }
        public ProductSet Product { get; set; }
        public Guid DreamerUserId { get; set; }
        public DreamerUserSet DreamerUser { get; set; }
        public int Quantity { get; set; }
    }
}
