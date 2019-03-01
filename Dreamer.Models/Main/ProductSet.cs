using Dreamer.Models.Base;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Dreamer.Models.Main
{
    [Table("Products", Schema = "Main")]
    public class ProductSet : BaseEntity
    {
        [Required]
        public string Name { get; set; }
        public double Price { get; set; }
        /// <summary>
        /// Discount 1->100
        /// </summary>
        public int DiscountPercentage { get; set; }
        public bool FreeDelivary { get; set; }

        public IEnumerable<ProductCategorySet> ProductCategories { get; set; }
        public IEnumerable<CartItemSet> CartItems { get; set; }
    }
}
