using Dreamer.API.Dtos.Products;
using Dreamer.IDomain.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Dreamer.API.Controllers
{
    [Authorize]
    [Produces("application/json")]
    [Route("[controller]/[action]")]
    [ApiController]
    public class ProductController : DreamerBaseController
    {
        private IProductsRepository ProductsRepository { get; }
        public ProductController(IProductsRepository productsRepository)
        {
            ProductsRepository = productsRepository;
        }
        /// <summary>
        /// Get available products, same methods for all cases, in case thier are filters the result will be filtered, or it will return all the results
        /// The result is paged and 
        /// </summary>
        [HttpPost]
        public IActionResult GetProducts([FromBody]GetProductsDto productModel)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }

            var products = ProductsRepository.GetAvailableProducts(productModel.Name, productModel.CategoryId, productModel.PageIndex, productModel.PageSize);
            if (products == null)
                return BadRequest();

            var result = new ProductPagingDto()
            {
                Products = products,
                HasNextPage = products.HasNextPage,
                HasPreviousPage = products.HasPreviousPage,
                IsFirstPage = products.IsFirstPage,
                IsLastPage = products.IsLastPage,
                PageCount = products.PageCount,
                PageNumber = products.PageNumber,
                PageSize = products.PageSize,
                TotalItemCount = products.TotalItemCount
            };
            return Json(result);

        }
    }
}