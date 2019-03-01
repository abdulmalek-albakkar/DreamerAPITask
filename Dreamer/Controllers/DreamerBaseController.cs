using Dreamer.Models.Enums;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Linq;
using System.Security.Claims;

namespace Dreamer.API.Controllers
{
    public abstract class DreamerBaseController : Controller
    {
        public Guid UserId
        {
            get
            {
                var val = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;
                return val == null ? Guid.Empty : Guid.Parse(val);
            }
        }
        public string UserEmail
        {
            get
            {
                return User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Email)?.Value;
            }
        }
        public string UserName
        {
            get
            {
                return User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Name)?.Value;
            }
        }
        public DreamerUserRoles UserRole
        {
            get
            {
                return Enum.Parse<DreamerUserRoles>(User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Role)?.Value);
            }
        }
    }
}
