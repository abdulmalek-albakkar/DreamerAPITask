using Dreamer.IDomain.Repositories;
using Microsoft.AspNetCore.Mvc.Filters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Dreamer.API.Filters
{
    /// <summary>
    /// Will update user last activity date when autherized user is trying to do API request
    /// </summary>
    public class LastActivityFilter : IActionFilter
    {
        private ISecurityRepository SecurityRepository { get; set; }
        public LastActivityFilter(ISecurityRepository securityRepository)
        {
            SecurityRepository = securityRepository;
        }
        public void OnActionExecuting(ActionExecutingContext context)
        {
            var currentUserId = context.HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!string.IsNullOrEmpty(currentUserId))
                SecurityRepository.UpdateLastActivity(Guid.Parse(currentUserId));
        }

        public void OnActionExecuted(ActionExecutedContext context)
        {
        }
    }
}
