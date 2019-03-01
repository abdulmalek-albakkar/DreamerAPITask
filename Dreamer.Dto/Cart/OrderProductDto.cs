using Dreamer.Dto.Item;
using System;
using System.Collections.Generic;

namespace Dreamer.Dto.Cart
{
    public class OrderProductDto
    {
        public Guid ProductId { get; set; }
        public string Name { get; set; }
        public double OneItemPrice { get; set; }
        public int Quantity { get; set; }
        public int DiscountPercentage { get; set; }
        public double DiscountValue { get; set; }
        public double NetPrice { get; set; }
        public IEnumerable<CategoryDto> ProductCategories { get; set; }
    }
}
