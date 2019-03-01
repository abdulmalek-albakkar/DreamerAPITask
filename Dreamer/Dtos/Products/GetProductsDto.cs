using System;
using System.ComponentModel.DataAnnotations;

namespace Dreamer.API.Dtos.Products
{
    public class GetProductsDto
    {
        [Range(1, Int32.MaxValue)]
        public int PageIndex { get; set; } = 1;
        [Range(1, Int32.MaxValue)]
        public int PageSize { get; set; } = 20;
        public string Name { get; set; }
        public Guid? CategoryId { get; set; }
    }
}
