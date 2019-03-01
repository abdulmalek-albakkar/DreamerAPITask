using Dreamer.Models.Enums;
using System;

namespace Dreamer.API.Dtos.Security
{
    public class TokenResult
    {
        public string Token { get; set; }
        public string Issuer { get; set; }
        public DreamerUserRoles Role { get; set; }
        public DateTime Expires { get; set; }
    }
}
