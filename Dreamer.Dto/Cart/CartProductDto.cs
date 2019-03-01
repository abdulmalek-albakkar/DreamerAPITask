using Dreamer.Dto.Item;
using System;
using System.Collections.Generic;
using System.Text;

namespace Dreamer.Dto.Cart
{
    public class CartProductDto
    {
        public Guid ProductId { get; set; }
        public ProductDto Product { get; set; }
        public int Quantity { get; set; }
    }
}
