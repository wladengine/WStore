using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using WStoreAPI.Data;
using WStoreAPI.Models;
using WStoreDataManagement.Library.DataAccess;
using WStoreDataManagement.Library.Models;

namespace WStoreAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    //[RoutePrefix("api/User")]
    public class UserController : ControllerBase
    {
        private readonly ApplicationDbContext _dbContext;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly IUserData _userData;
        private readonly ILogger<UserController> _logger;

        public UserController(ApplicationDbContext dbContext, UserManager<IdentityUser> userManager, IUserData userData, ILogger<UserController> logger)
        {
            _dbContext = dbContext;
            _userManager = userManager;
            _userData = userData;
            _logger = logger;
        }

        // GET: User/GetById/
        [HttpGet]
        public UserModel GetById()
        {
            return GetUserModelById();
        }

        private UserModel GetUserModelById()
        {
            string userId = User.FindFirstValue(ClaimTypes.NameIdentifier); //new way to get the user principal id

            var output = _userData.GetUserById(userId).First();

            return output;
        }


        // GET api/User/Admin/GetAllUsers
        [Authorize(Roles = "Admin")]
        [HttpGet]
        [Route("Admin/GetAllUsers")]
        public List<ApplicationUserModel> GetAllUsers()
        {
            List<ApplicationUserModel> output = new List<ApplicationUserModel>();

            var users = _dbContext.Users.ToList();
            var userRoles = from usrRoles in _dbContext.UserRoles
                            join rol in _dbContext.Roles on usrRoles.RoleId equals rol.Id
                            select new
                            {
                                usrRoles.UserId,
                                usrRoles.RoleId,
                                RoleName = rol.Name
                            };

            foreach (var user in users)
            {
                ApplicationUserModel model = new ApplicationUserModel()
                {
                    UserId = user.Id,
                    Email = user.Email,
                    Roles = userRoles
                        .Where(u => u.UserId == user.Id)
                        .ToDictionary(key => key.RoleId, val => val.RoleName),
                };

                output.Add(model);
            }

            return output;
        }

        // GET api/User/Admin/GetAllRoles
        [Authorize(Roles = "Admin")]
        [HttpGet]
        [Route("Admin/GetAllRoles")]
        public Dictionary<string, string> GetAllRoles()
        {
            var roles = _dbContext.Roles.ToDictionary(k => k.Id, v => v.Name);
            return roles;
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        [Route("Admin/AddRoleToUser")]
        public async Task AddRoleToUser(UserRolePairModel model)
        {
            var adminUserModel = GetUserModelById();

            var usr = await _userManager.FindByIdAsync(model.UserId);

            _logger.LogInformation("Admin {Admin} (id={AdminId}) added user {User} (id = {UserId}) to role {Role}", 
                adminUserModel.EmailAddress, adminUserModel.Id, usr.Email, usr.Id, model.RoleName);

            await _userManager.AddToRoleAsync(usr, model.RoleName);
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        [Route("Admin/RemoveRoleFromUser")]
        public async Task RemoveRoleFromUser(UserRolePairModel model)
        {
            var adminUserModel = GetUserModelById();

            var usr = await _userManager.FindByIdAsync(model.UserId);

            _logger.LogInformation("Admin {Admin} (id={AdminId}) removerd user {User} (id = {UserId}) from role {Role}",
                adminUserModel.EmailAddress, adminUserModel.Id, usr.Email, usr.Id, model.RoleName);

            await _userManager.RemoveFromRoleAsync(usr, model.RoleName);
        }
    }
}
