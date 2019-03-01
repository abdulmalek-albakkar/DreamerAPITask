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
    public class CartController : DreamerBaseController
    {
        private ICartRepository CartRepository { get; }
        public CartController(ICartRepository cartRepository)
        {
            CartRepository = cartRepository;
        }
        [HttpPut]
        public IActionResult AddProductToCart([FromBody]ProductCartDto productCartModel)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }

            if (CartRepository.SaveProductToCart(productCartModel.ProductId, UserId, productCartModel.Quantity))
                return Ok();
            else
                return BadRequest();
        }
        [HttpPut]
        public IActionResult EditCartItemQuantity([FromBody]ProductCartDto productCartModel)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }

            if (CartRepository.SaveProductToCart(productCartModel.ProductId, UserId, productCartModel.Quantity))
                return Ok();
            else
                return BadRequest();
        }
        /// <summary>
        /// All current user's will be moved to new order and the user will be notified on his email about the order details
        /// The cart will be cleared and the delivery will happen in 24 hours
        /// </summary>
        [HttpPut]
        public IActionResult CheckOutCart([FromBody]CheckOutDetailsDto checkOutDetailsModel)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }

            var result = CartRepository.CheckOutCart(UserId, checkOutDetailsModel.Address);
            if (result == null)
                return BadRequest();
            else
                return Json(result);
        }
        /// <summary>
        /// Get specefic order details including all the products and thier categories, and the order has to belong to the current user or will return Unauthorized
        /// </summary>
        [HttpPost]
        public IActionResult GetOrderStatus([FromBody]GetOrderStatusDto getOrderStatusDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }

            if (UserRole == Models.Enums.DreamerUserRoles.Customer && !CartRepository.HasOrder(getOrderStatusDto.OrderId, UserId))
                return Unauthorized();

            var result = CartRepository.GetOrderDetails(getOrderStatusDto.OrderId);
            if (result == null)
                return BadRequest();

            return Json(result);
        }
        /// <summary>
        /// Get all current user's orders details including all the products and thier categories
        /// </summary>
        [HttpPost]
        public IActionResult GetMyOrders()
        {
            var result = CartRepository.GetUserOrders(UserId);
            if (result == null)
                return BadRequest();

            return Json(result);
        }

        /// <summary>
        /// Get all current user's cart items including all the products and thier categories
        /// </summary>
        [HttpPost]
        public IActionResult GetMyCartItems()
        {
            var result = CartRepository.GetUserCartItems(UserId);
            if (result == null)
                return BadRequest();

            return Json(result);
        }

        [HttpDelete]
        public IActionResult DeleteProductFromCart([FromBody]DeleteCartItemDto deleteCartItemModal)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }

            if (CartRepository.DeleteProductFromCart(deleteCartItemModal.ProductId, UserId))
                return Ok();
            else
                return BadRequest();
        }
        [HttpDelete]
        public IActionResult ClearCart()
        {
            if (CartRepository.ClearCart(UserId))
                return Ok();
            else
                return BadRequest();
        }
    }
}
