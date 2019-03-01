using System.ComponentModel.DataAnnotations;

namespace Dreamer.API.Dtos.Security
{
    public class GetTokenModel
    {
        [EmailAddress]
        public string Email { get; set; }
        public string Password { get; set; }
    }
}
