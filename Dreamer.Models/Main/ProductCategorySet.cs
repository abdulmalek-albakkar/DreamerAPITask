using Dreamer.Models.Base;
using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace Dreamer.Models.Main
{
    [Table("ProductCategories", Schema = "Main")]
    public class ProductCategorySet : BaseEntity
    {
        public Guid ProductId { get; set; }
        public ProductSet Product { get; set; }
        public Guid CategoryId { get; set; }
        public CategorySet Category { get; set; }
    }
}
