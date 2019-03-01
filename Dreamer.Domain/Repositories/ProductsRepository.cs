using Dreamer.Dto.Item;
using Dreamer.IDomain.Repositories;
using Dreamer.SharedContext.Repository;
using Dreamer.SqlServer.Database;
using Microsoft.EntityFrameworkCore;
using Serilog;
using System;
using System.Linq;
using X.PagedList;

namespace Dreamer.Domain.Repositories
{
    public class ProductsRepository : DreamerRepository, IProductsRepository
    {
        public ProductsRepository(DreamerDbContext dreamerDbContext) : base(dreamerDbContext)
        {

        }

        public IPagedList<ProductDto> GetAvailableProducts(string nameFilter, Guid? categoryId, int pageIndex, int pageSize)
        {
            try
            {
                var querable = DreamerDbContext.Products
                    .Include(product => product.ProductCategories).ThenInclude(pc => pc.Category)
                    .Where(product => product.IsValid);

                if (!string.IsNullOrEmpty(nameFilter))
                    querable = querable.Where(product => product.Name.Contains(nameFilter));

                if (categoryId.HasValue)
                    querable = querable.Where(product => product.ProductCategories.Any(pc => pc.CategoryId == categoryId));


                return querable.ToPagedList(pageIndex, pageSize).Select(product => new ProductDto
                {
                    Id = product.Id,
                    DiscountPercentage = product.DiscountPercentage,
                    FreeDelivary = product.FreeDelivary,
                    Name = product.Name,
                    Price = product.Price,
                    ProductCategories = product.ProductCategories?.Select(pc => new CategoryDto { Id = pc.CategoryId, Name = pc.Category.Name })
                });
            }
            catch (Exception ex)
            {
                Log.Error(ex.Message);
                return null;
            }
        }
    }
}
