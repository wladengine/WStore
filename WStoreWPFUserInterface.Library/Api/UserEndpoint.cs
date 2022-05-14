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
    }
}
