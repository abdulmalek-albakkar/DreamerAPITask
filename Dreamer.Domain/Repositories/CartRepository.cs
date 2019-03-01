using Dreamer.Dto.Cart;
using Dreamer.EmailNotifier.IService;
using Dreamer.IDomain.Repositories;
using Dreamer.InternalHangfire.IService;
using Dreamer.Models.Main;
using Dreamer.SharedContext.Repository;
using Dreamer.ShippingCostCalculator.IService;
using Dreamer.SqlServer.Database;
using Microsoft.EntityFrameworkCore;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace Dreamer.Domain.Repositories
{
    public class CartRepository : DreamerRepository , ICartRepository 
    {
        private IEmailNotifierService EmailNotifierService { get; }
        private IShippingCostCalculatorService ShippingCostCalculatorService { get; }
        private IInternalHangfireService InternalHangfireService { get; }
        public CartRepository(DreamerDbContext dreamerDbContext, IEmailNotifierService emailNotifierService
            , IShippingCostCalculatorService shippingCostCalculatorService, IInternalHangfireService internalHangfireService) : base(dreamerDbContext)
        {
            EmailNotifierService = emailNotifierService;
            ShippingCostCalculatorService = shippingCostCalculatorService;
            InternalHangfireService = internalHangfireService;
        }
        public bool SaveProductToCart(Guid productId, Guid userId, int quantity)
        {
            try
            {
                Expression<Func<CartItemSet, bool>> filterCartItem = ci => ci.IsValid && ci.DreamerUserId == userId && ci.ProductId == productId;

                if (DreamerDbContext.CartItems.Any(filterCartItem))
                {
                    var cartItemSet = DreamerDbContext.CartItems.First(filterCartItem);
                    cartItemSet.Quantity = quantity;
                }
                else
                    DreamerDbContext.CartItems.Add(new CartItemSet { DreamerUserId = userId, ProductId = productId, Quantity = quantity });

                DreamerDbContext.SaveChanges();
                return true;
            }
            catch (Exception ex)
            {
                Log.Error(ex.Message);
                return false;
            }
        }

        public DetailedOrderDto CheckOutCart(Guid userId, string address)
        {
            try
            {
                OrderSet orderSet;
                var orderProducts = new List<OrderProductSet>();
                var ShipmentToAddressCost = ShippingCostCalculatorService.Calculate(address);

                //Moving the user cart to new "PAID" order
                var userCart = DreamerDbContext.CartItems.Where(ci => ci.IsValid && ci.DreamerUserId == userId).Include(ci => ci.Product).AsEnumerable();
                if (!userCart.Any()) return null;
                foreach (var item in userCart)
                {
                    var orderProductSet = new OrderProductSet
                    {
                        ProductId = item.ProductId,
                        DiscountPercentage = item.Product.DiscountPercentage,
                        OneItemPrice = item.Product.Price,
                        Quantity = item.Quantity,
                        DiscountValue = item.Product.Price * item.Quantity * item.Product.DiscountPercentage / 100
                    };
                    orderProductSet.NetPrice = (orderProductSet.OneItemPrice * orderProductSet.Quantity) - orderProductSet.DiscountValue;
                    orderProducts.Add(orderProductSet);
                }
                orderSet = new OrderSet
                {
                    PaymentDateTime = DateTimeOffset.UtcNow,
                    DreamerUserId = userId,
                    Status = Models.Enums.OrderStatus.Paid,
                    NetItemsPrice = orderProducts.Sum(op => op.NetPrice),
                    NetItemsDiscountValue = orderProducts.Sum(op => op.DiscountValue),
                    OrderProducts = orderProducts,
                    OrderShipment = new OrderShipmentSet { Address = address, ShipmentCost = ShipmentToAddressCost },
                    FreeDelivery = userCart.Any(ci => ci.Product.FreeDelivary)
                };

                //One free-shipping product in the card is enough to make the whole card shipment for free
                if (orderSet.FreeDelivery)
                    orderSet.OrderShipment.ShipmentCost = 0;
                else
                    orderSet.OrderShipment.ShipmentCost = ShipmentToAddressCost;

                //Calculating the total for the order
                orderSet.Total = orderSet.NetItemsPrice + orderSet.OrderShipment.ShipmentCost;

                DreamerDbContext.Orders.Add(orderSet);
                DreamerDbContext.SaveChanges();

                //Clearing the cart
                ClearCart(userId);

                //Send email notification
                var userEmail = DreamerDbContext.DreamerUsers.Where(user => user.Id == userId).Select(c => c.Email).Single();
                if (EmailNotifierService.SendPaidSuccessfully(userEmail, orderSet))
                {
                    orderSet.EmailNotified = true;
                    DreamerDbContext.SaveChanges();
                }

                //Set as shipped the next day
                InternalHangfireService.EnqueueMarkOrderAsShipped(orderSet.Id);

                return GetOrderDetails(orderSet.Id);
            }
            catch (Exception ex)
            {
                Log.Error(ex.Message);
                return null;
            }
        }

        public bool HasOrder(Guid orderId,Guid userId)
        {
            return DreamerDbContext.Orders.Any(order => order.Id == orderId && order.IsValid && order.DreamerUserId == userId);
        }

        /// <summary>
        /// Expresssion convertor to be used purely with EF core in multiple cases (as below)
        /// </summary>
        Expression<Func<OrderSet, DetailedOrderDto>> detailedOrderConvertor = order => new DetailedOrderDto
        {
            Id = order.Id,
            PaymentDateTime = order.PaymentDateTime,
            EmailNotified = order.EmailNotified,
            FreeDelivery = order.FreeDelivery,
            NetItemsPrice = order.NetItemsPrice,
            SerialNumber = order.SerialNumber,
            Status = order.Status,
            Total = order.Total,
            NetItemsDiscountValue = order.NetItemsDiscountValue,
            OrderProducts = order.OrderProducts.Select(op => new Dto.Cart.OrderProductDto
            {
                ProductId = op.ProductId,
                DiscountPercentage = op.DiscountPercentage,
                DiscountValue = op.DiscountValue,
                NetPrice = op.NetPrice,
                Name = op.Product.Name,
                OneItemPrice = op.OneItemPrice,
                Quantity = op.Quantity,
                ProductCategories = op.Product.ProductCategories.Select(pc => new Dto.Item.CategoryDto
                {
                    Id = pc.CategoryId,
                    Name = pc.Category.Name
                })
            }),
            OrderShipmentDetails = new ShipmentDetailsDto
            {
                Address = order.OrderShipment.Address,
                DeliveryTime = order.OrderShipment.DeliveryTime,
                ShippedTime = order.OrderShipment.ShippedTime,
                ShipmentCost = order.OrderShipment.ShipmentCost
            }
        };
        public DetailedOrderDto GetOrderDetails(Guid orderId)
        {
            try
            {
                if (!DreamerDbContext.Orders.Any(order => order.IsValid && order.Id == orderId))
                    return null;

                return DreamerDbContext.Orders
                    .Where(order => order.IsValid && order.Id == orderId)
                    .Select(detailedOrderConvertor).Single();
            }
            catch (Exception ex)
            {
                Log.Error(ex.Message);
                return null;
            }
        }
        public IEnumerable<DetailedOrderDto> GetUserOrders(Guid userId)
        {
            try
            {
                return DreamerDbContext.Orders
                .Where(order => order.IsValid && order.DreamerUserId == userId)
                .Select(detailedOrderConvertor).AsEnumerable();
            }
            catch (Exception ex)
            {
                Log.Error(ex.Message);
                return null;
            }
        }
        public IEnumerable<CartProductDto> GetUserCartItems(Guid userId)
        {
            try
            {
                return DreamerDbContext.CartItems
                .Where(ci => ci.IsValid && ci.DreamerUserId == userId)
                .Select(ci => new CartProductDto
                {
                    ProductId = ci.ProductId,
                    Quantity = ci.Quantity,
                    Product = new Dto.Item.ProductDto
                    {
                        DiscountPercentage = ci.Product.DiscountPercentage,
                        FreeDelivary = ci.Product.FreeDelivary,
                        Id = ci.Product.Id,
                        Name = ci.Product.Name,
                        Price = ci.Product.Price,
                        ProductCategories = ci.Product.ProductCategories.Select(pc=> new Dto.Item.CategoryDto
                        {
                            Id = pc.CategoryId,
                            Name = pc.Category.Name
                        })
                    }
                }).AsEnumerable();
            }
            catch (Exception ex)
            {
                Log.Error(ex.Message);
                return null;
            }
        }
        public bool DeleteProductFromCart(Guid productId, Guid userId)
        {
            try
            {
                var cartItemSet = DreamerDbContext.CartItems.FirstOrDefault(ci => ci.IsValid && ci.DreamerUserId == userId && ci.ProductId == productId);
                if (cartItemSet != null)
                {
                    cartItemSet.IsValid = false;
                    DreamerDbContext.SaveChanges();
                    return true;
                }
                else
                    return false;
            }
            catch (Exception ex)
            {
                Log.Error(ex.Message);
                return false;
            }
        }
        public bool ClearCart(Guid userId)
        {
            try
            {
                foreach (var Id in DreamerDbContext.CartItems.Where(ci => ci.DreamerUserId == userId).Select(ci => ci.Id))
                {
                    //Soft delete without calling the item, modify one column
                    var cartItem = new CartItemSet { Id = Id, IsValid = false };
                    DreamerDbContext.Entry(cartItem).Property(ci => ci.IsValid).IsModified = true;
                }
                DreamerDbContext.SaveChanges();
                return true;
            }
            catch (Exception ex)
            {
                Log.Error(ex.Message);
                return false;
            }
        }

        public bool MarkOrderAsDelivered(Guid orderId)
        {
            try
            {
                var orderSet = DreamerDbContext.Orders.Include(order => order.OrderShipment).Include(order => order.DreamerUser).Where(order => order.Id == orderId && order.IsValid).FirstOrDefault();
                if (orderSet == null || orderSet.Status == Models.Enums.OrderStatus.Delivered) return false;
                orderSet.Status = Models.Enums.OrderStatus.Delivered;
                orderSet.OrderShipment.DeliveryTime = DateTimeOffset.UtcNow;
                DreamerDbContext.SaveChanges();

                EmailNotifierService.SendDeliveredSuccessfully(orderSet.DreamerUser.Email, orderSet.SerialNumber);
                return true;
            }
            catch (Exception ex)
            {
                Log.Error(ex.Message);
                return false;
            }
        }
    }
}
