using System;

namespace Dreamer.API.Dtos.Products
{
    public class ProductCartDto
    {
        public Guid ProductId { get; set; }
        public int Quantity { get; set; }
    }
}
