using Dreamer.Dto.Item;
using System.Collections.Generic;

namespace Dreamer.API.Dtos.Products
{
    public class ProductPagingDto
    {
        public IEnumerable<ProductDto> Products { get; set; }
        public int PageCount { get; set; }
        public int TotalItemCount { get; set; }
        public int PageNumber { get; set; }
        public int PageSize { get; set;  }
        public bool HasPreviousPage { get; set; }
        public bool HasNextPage { get; set; }
        public bool IsFirstPage { get; set; }
        public bool IsLastPage { get; set; }
    }
}
