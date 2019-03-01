using System;
using System.Collections.Generic;

namespace Dreamer.Dto.Item
{
    public class ProductDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public double Price { get; set; }
        public int DiscountPercentage { get; set; }
        public bool FreeDelivary { get; set; }
        public IEnumerable<CategoryDto> ProductCategories { get; set; }
    }
}
