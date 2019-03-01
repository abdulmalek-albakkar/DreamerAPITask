using Dreamer.Models.Enums;
using System;

namespace Dreamer.Dto.Security
{
    public class DreamerUserDto
    {
        public Guid Id { get; set; }
        public string Email { get; set; }
        public string Name { get; set; }
        public string Password { get; set; }
        public DreamerUserRoles Role { get; set; }
    }
}
