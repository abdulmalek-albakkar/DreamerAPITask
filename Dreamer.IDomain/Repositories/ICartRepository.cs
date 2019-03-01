using Dreamer.Dto.Cart;
using System;
using System.Collections.Generic;

namespace Dreamer.IDomain.Repositories
{
    public interface ICartRepository
    {
        /// <summary>
        /// Add or edit product in specefic user's cart
        /// </summary>
        bool SaveProductToCart(Guid productId, Guid userId, int quantity);

        /// <summary>
        /// Checkout user's item, which will do the following:
        /// 1- Move the items from the cart to a new order that has PAID status, and calculate the net prices/delivery costs/discounts
        /// 2- Clear the cart
        /// 3- Send notification to the user's email that contains the details about his order
        /// 4- Enqueue shipping order after 24 hours
        /// </summary>
        /// <param name="address">The address to be shipped to, which will be used to calculate the shipping costs</param>
        DetailedOrderDto CheckOutCart(Guid userId, string address);


        /// <summary>
        /// Validate that a specefic user has a specefic order
        /// </summary>
        bool HasOrder(Guid orderId, Guid userId);


        DetailedOrderDto GetOrderDetails(Guid orderId);
        IEnumerable<DetailedOrderDto> GetUserOrders(Guid userId);
        /// <summary>
        /// Get specefic user's cart items so they can check thier items
        /// </summary>
        IEnumerable<CartProductDto> GetUserCartItems(Guid userId);
        bool DeleteProductFromCart(Guid productId, Guid userId);
        bool ClearCart(Guid userId);

        /// <summary>
        /// 1- Change order status
        /// 2- Set shipping date
        /// </summary>
        bool MarkOrderAsDelivered(Guid orderId);
    }
}
