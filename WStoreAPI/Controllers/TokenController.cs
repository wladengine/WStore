using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using WStoreAPI.Data;

using System.Linq;
using System.Security.Claims;
using System.IdentityModel.Tokens.Jwt;
using System;
using System.Collections.Generic;
using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace WStoreAPI.Controllers
{
    public class TokenController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;

        public TokenController(ApplicationDbContext context, UserManager<IdentityUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // POST /api/token
        [HttpPost]
        [Route("/token")]
        public async Task<IActionResult> Create(string userName, string password, string grant_type)
        {
            if (await IsValidCredentials(userName, password))
            {
                return new ObjectResult(await GenerateToken(userName));
            }
            else
            {
                return BadRequest();
            }
        }

        private async Task<bool> IsValidCredentials(string userName, string password)
        {
            var user = await _userManager.FindByEmailAsync(userName);
            return await _userManager.CheckPasswordAsync(user, password);
        }

        private async Task<dynamic> GenerateToken(string userName)
        {
            var user = await _userManager.FindByEmailAsync(userName);

            var roles = from usr_roles in _context.UserRoles
                        join rol in _context.Roles on usr_roles.RoleId equals rol.Id
                        where usr_roles.UserId == user.Id
                        select new { usr_roles.UserId, usr_roles.RoleId, RoleName = rol.Name };

            var claims = new List<Claim>()
            {
                new Claim(ClaimTypes.Name, userName),
                new Claim(ClaimTypes.NameIdentifier, user.Id),
                new Claim(JwtRegisteredClaimNames.Nbf, // Nbf = "not before"
                    new DateTimeOffset(DateTime.Now).ToUnixTimeSeconds().ToString()), // JWT works with UNIX time seconds
                new Claim(JwtRegisteredClaimNames.Exp, // Exp = "expiration"
                    new DateTimeOffset(DateTime.Now.AddHours(1)).ToUnixTimeSeconds().ToString()),
            };

            foreach (var role in roles)
            {
                claims.Add(new Claim(ClaimTypes.Role, role.RoleName));
            }

            var token = new JwtSecurityToken
                (
                    header: new JwtHeader(
                        new SigningCredentials(
                            // TODO: make the key vault to protect the key
                            key: new SymmetricSecurityKey(Encoding.UTF8.GetBytes("Some_very_secret_key")), 
                            algorithm: SecurityAlgorithms.HmacSha256
                        )
                    ),
                    payload: new JwtPayload(claims)
                );

            var output = new 
            {
                Access_Token = new JwtSecurityTokenHandler().WriteToken(token),
                UserName = userName,
            };

            return output;
        }
    }
}
