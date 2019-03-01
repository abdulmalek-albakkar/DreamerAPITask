using Dreamer.Models.Base;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Dreamer.Models.Main
{
    [Table("Categories", Schema = "Main")]
    public class CategorySet : BaseEntity
    {
        [Required]
        public string Name { get; set; }


        public IEnumerable<ProductCategorySet> ProductCategories { get; set; }
    }
}
