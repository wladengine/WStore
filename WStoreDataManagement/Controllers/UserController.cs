﻿using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using WStoreDataManagement.Library.DataAccess;
using WStoreDataManagement.Library.Models;
using WStoreDataManagement.Models;

namespace WStoreDataManagement.Controllers
{
    [Authorize]
    [RoutePrefix("api/User")]
    public class UserController : ApiController
    {
        // GET: User/Details/5
        [HttpGet]
        public UserModel GetById()
        {
            string id = RequestContext.Principal.Identity.GetUserId();

            //TODO: make a DI against the direct dependency
            UserData data = new UserData();

            var output = data.GetUserById(id).First();

            return output;
        }


        // GET api/User/Admin/GetAllUsers
        [Authorize(Roles="Admin")]
        [HttpGet]
        [Route("Admin/GetAllUsers")]
        public List<ApplicationUserModel> GetAllUsers()
        {
            List<ApplicationUserModel> output = new List<ApplicationUserModel>();

            // Using the ASP.NET EF Application DB Context
            // Microsoft.AspNet.Identity.EntityFramework

            using (var context = new ApplicationDbContext())
            {
                var userStore = new UserStore<ApplicationUser>(context);
                var userManager = new UserManager<ApplicationUser>(userStore);

                var users = userManager.Users.ToList();
                var roles = context.Roles.ToList();

                foreach (var user in users)
                {
                    ApplicationUserModel model = new ApplicationUserModel()
                    {
                        UserId = user.Id,
                        Email = user.Email,
                        Roles = user.Roles.ToDictionary(k => k.RoleId, v => roles.First(x => x.Id == v.RoleId).Name),
                    };

                    output.Add(model);
                }

                return output;
            }
        }

        // GET api/User/Admin/GetAllRoles
        [Authorize(Roles = "Admin")]
        [HttpGet]
        [Route("Admin/GetAllRoles")]
        public Dictionary<string, string> GetAllRoles()
        {
            using (var context = new ApplicationDbContext())
            {
                var roles = context.Roles.ToDictionary(k => k.Id, v => v.Name);
                return roles;
            }
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        [Route("Admin/AddRoleToUser")]
        public void AddRoleToUser(UserRolePairModel model)
        {
            using (var context = new ApplicationDbContext())
            {
                var userStore = new UserStore<ApplicationUser>(context);
                var userManager = new UserManager<ApplicationUser>(userStore);

                userManager.AddToRole(model.UserId, model.RoleName);
            }
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        [Route("Admin/RemoveRoleFromUser")]
        public void RemoveRoleFromUser(UserRolePairModel model)
        {
            using (var context = new ApplicationDbContext())
            {
                var userStore = new UserStore<ApplicationUser>(context);
                var userManager = new UserManager<ApplicationUser>(userStore);

                userManager.RemoveFromRole(model.UserId, model.RoleName);
            }
        }
    }
}
