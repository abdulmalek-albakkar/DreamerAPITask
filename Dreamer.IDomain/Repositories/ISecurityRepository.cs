using Dreamer.Dto.Security;
using Dreamer.Models.Enums;
using System;

namespace Dreamer.IDomain.Repositories
{
    public interface ISecurityRepository
    {
        DreamerUserDto Login(string email, string password);
        DreamerUserDto AddNewUser(string email, string password, string name, DreamerUserRoles role);
        void UpdateLastActivity(Guid userId);
    }
}
