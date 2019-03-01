using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Dreamer.API.Configs;
using Dreamer.API.Dtos.Security;
using Dreamer.API.Filters;
using Dreamer.Dto.Security;
using Dreamer.IDomain.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace Dreamer.API.Controllers
{
    [AllowAnonymous]
    [Produces("application/json")]
    [Route("[controller]/[action]")]
    [ApiController]
    public class TokenController : DreamerBaseController
    {
        private ISecurityRepository SecurityRepository { get; }
        private TokenConfig TokenConfig { get; }
        public TokenController(ISecurityRepository securityRepository, IOptionsMonitor<TokenConfig> tokenConfig)
        {
            SecurityRepository = securityRepository;
            TokenConfig = tokenConfig.CurrentValue;
        }
        /// <summary>
        /// Validating the sent info and return token which will be used in all other requests
        /// </summary>
        [HttpPost]
        public IActionResult GetToken([FromBody] GetTokenModel getTokenModel)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }
            var dreamerUser = SecurityRepository.Login(getTokenModel.Email, getTokenModel.Password);

            if (dreamerUser == null)
            {
                return Unauthorized();
            }
            return Json(BuildToken(dreamerUser));
        }

        private TokenResult BuildToken(DreamerUserDto dreamerUser)
        {
            var claims = new[] {
                    new Claim(ClaimTypes.NameIdentifier, dreamerUser.Id.ToString()),
                    new Claim(ClaimTypes.Email, dreamerUser.Email),
                    new Claim(ClaimTypes.Name, dreamerUser.Name),
                    new Claim(ClaimTypes.Role, dreamerUser.Role.ToString())
                };
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(TokenConfig.TokenKey));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: TokenConfig.Domain,
                audience: TokenConfig.Domain,
                claims: claims,
                expires: DateTime.Now.AddHours(TokenConfig.ExpiredInHours),
                signingCredentials: creds);
            return new TokenResult()
            {
                Token = new JwtSecurityTokenHandler().WriteToken(token),
                Issuer = token.Issuer,
                Expires = token.ValidTo,
                Role = dreamerUser.Role
            };
        }

    }
}