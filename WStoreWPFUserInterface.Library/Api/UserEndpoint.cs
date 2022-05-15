using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using WStoreWPFUserInterface.Library.Models;

namespace WStoreWPFUserInterface.Library.Api
{
    public class UserEndpoint : IUserEndpoint
    {
        private IAPIHelper _apiHelper;
        public UserEndpoint(IAPIHelper apiHelper)
        {
            _apiHelper = apiHelper;
        }

        public async Task<List<UserModel>> GetAllAsync()
        {
            using (HttpResponseMessage message = await _apiHelper.HttpClient.GetAsync("/api/User/Admin/GetAllUsers"))
            {
                if (message.IsSuccessStatusCode)
                {
                    var response = await message.Content.ReadAsAsync<List<UserModel>>();
                    return response;
                }
                else
                {
                    throw new Exception(message.ReasonPhrase);
                }
            }
        }

        public async Task<Dictionary<string, string>> GetAllRolesAsync()
        {
            using (HttpResponseMessage message = await _apiHelper.HttpClient.GetAsync("/api/User/Admin/GetAllRoles"))
            {
                if (message.IsSuccessStatusCode)
                {
                    var response = await message.Content.ReadAsAsync<Dictionary<string, string>>();
                    return response;
                }
                else
                {
                    throw new Exception(message.ReasonPhrase);
                }
            }
        }

        public async Task AddUserToRole(string userId, string roleName)
        {
            var data = new { userId, roleName };
            using (HttpResponseMessage message = 
                await _apiHelper.HttpClient.PostAsJsonAsync("/api/User/Admin/AddRoleToUser", data))
            {
                if (!message.IsSuccessStatusCode)
                    throw new Exception(message.ReasonPhrase);
            }
        }

        public async Task RemoveUserFromRole(string userId, string roleName)
        {
            var data = new { userId, roleName };
            using (HttpResponseMessage message =
                await _apiHelper.HttpClient.PostAsJsonAsync("/api/User/Admin/RemoveRoleFromUser", data))
            {
                if (!message.IsSuccessStatusCode)
                    throw new Exception(message.ReasonPhrase);
            }
        }
    }
}
