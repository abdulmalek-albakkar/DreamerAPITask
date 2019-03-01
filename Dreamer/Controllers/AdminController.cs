using Dreamer.API.Dtos.Admin;
using Dreamer.Dto.Security;
using Dreamer.IDomain.Repositories;
using Dreamer.Models.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;

namespace Dreamer.API.Controllers
{
    /// <summary>
    /// Admin Area
    /// </summary>
    [Authorize(Roles = nameof(DreamerUserRoles.Administrator))]
    [Route("[controller]/[action]")]
    [ApiController]
    public class AdminController : DreamerBaseController
    {
        private ICartRepository CartRepository { get; }
        private ISecurityRepository SecurityRepository { get; }
        public AdminController(ICartRepository cartRepository,ISecurityRepository securityRepository)
        {
            CartRepository = cartRepository;
            SecurityRepository = securityRepository;
        }
        /// <summary>
        /// Get specefic user's detailed orders
        /// </summary>
        [HttpPost]
        public IActionResult GetUserOrders(GetUserOrdersDto getUserOrdersDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }

            var result = CartRepository.GetUserOrders(getUserOrdersDto.UserId);
            if (result == null)
                return BadRequest();
            else
                return Json(result);
        }
        /// <summary>
        /// Used to set order status : "Delivered"
        /// </summary>
        [HttpPut]
        public IActionResult MarkOrderAsDelivered(MarkOrderAsDeliveredDto markOrderAsDeliveredDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }

            if (CartRepository.MarkOrderAsDelivered(markOrderAsDeliveredDto.OrderId))
                return Ok();
            else
                return BadRequest();
        }
        [HttpPut]
        public IActionResult AddNewUser(DreamerUserDto user)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }

            var savedUser = SecurityRepository.AddNewUser(user.Email, user.Password, user.Name, user.Role);
            if (savedUser == null)
                return BadRequest();
            else
                return Json(savedUser);
        }
    }
}