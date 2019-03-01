using Dreamer.Dto.Security;
using Dreamer.IDomain.Repositories;
using Dreamer.Models.Enums;
using Dreamer.Models.Security;
using Dreamer.SharedContext.Repository;
using Dreamer.SqlServer.Database;
using Dreamer.SqlServer.Tools;
using Serilog;
using System;
using System.Linq;

namespace Dreamer.Domain.Repositories
{
    public class SecurityRepository : DreamerRepository, ISecurityRepository
    {
        public SecurityRepository(DreamerDbContext dreamerDbContext) : base(dreamerDbContext)
        {
        }

        public DreamerUserDto Login(string email, string password)
        {
            try
            {
                var hashedPassword = password.ToSHA256();
                var userSet = DreamerDbContext.DreamerUsers.Where(user => user.Email == email && user.Password == hashedPassword).SingleOrDefault();

                if (userSet == null) return null;

                userSet.LastActivityDate = DateTimeOffset.UtcNow;
                DreamerDbContext.SaveChanges();
                return new DreamerUserDto()
                {
                    Id = userSet.Id,
                    Name = userSet.Name,
                    Email = userSet.Email,
                    Role = userSet.Role
                };
            }
            catch (Exception ex)
            {
                Log.Error(ex.Message);
                return null;
            }
        }
        public DreamerUserDto AddNewUser(string email, string password, string name, DreamerUserRoles role)
        {
            try
            {
                if (DreamerDbContext.DreamerUsers.Any(user => user.Email == email)) return null;
                if (!email.IsValidEmail()) return null;

                var userSet = new DreamerUserSet { Email = email, Password = password.ToSHA256(), Name = name, Role = role };
                DreamerDbContext.DreamerUsers.Add(userSet);
                DreamerDbContext.SaveChanges();
                return new DreamerUserDto { Id = userSet.Id, Email = userSet.Email, Name = userSet.Name, Role = userSet.Role };
            }
            catch (Exception ex)
            {
                Log.Error(ex.Message);
                return null;
            }
        }
        public void UpdateLastActivity(Guid userId)
        {
            try
            {
                var userSet = new DreamerUserSet { Id = userId, LastActivityDate = DateTimeOffset.UtcNow };
                DreamerDbContext.Entry(userSet).Property(user => user.LastActivityDate).IsModified = true;
                DreamerDbContext.SaveChanges();
            }
            catch (Exception ex)
            {
                Log.Error(ex.Message);
            }
        }

    }
}
