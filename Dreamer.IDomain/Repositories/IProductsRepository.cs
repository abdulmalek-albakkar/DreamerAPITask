using Dreamer.Dto.Item;
using System;
using X.PagedList;

namespace Dreamer.IDomain.Repositories
{
    public interface IProductsRepository
    {
        /// <summary>
        /// Get products based on 2 filters, and will return the result with no filteration if the (first) 2 parameters are null
        /// </summary>
        /// <param name="nameFilter">Whether product name contains this string</param>
        /// <param name="categoryId">Whether product this category as one of its categories</param>
        /// <param name="pageIndex">Starts from 1</param>
        /// <returns>Paged list which has also other information about all pages count, all results count, isFirstPage, isLastPage.. etc</returns>
        IPagedList<ProductDto> GetAvailableProducts(string nameFilter, Guid? categoryId, int pageIndex, int pageSize);
    }
}
