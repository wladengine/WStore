using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using WStoreWPFUserInterface.Models;

namespace WStoreWPFUserInterface.Helpers
{
    public class APIHelper : IAPIHelper
    {
        private HttpClient apiClient;

        public APIHelper()
        {
            InitializeClient();
        }

        private void InitializeClient()
        {
            string apiUrl = ConfigurationManager.AppSettings["api"];

            apiClient = new HttpClient
            {
                BaseAddress = new Uri(apiUrl)
            };

            apiClient.DefaultRequestHeaders.Accept.Clear();
            apiClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        }

        public async Task<AuthenticatedUser> AuthenticateAsync(string userName, string password)
        {
            var data = new FormUrlEncodedContent(new[]
            {
                new KeyValuePair<string, string>("grant_type", "password"),
                new KeyValuePair<string, string>("username", userName),
                new KeyValuePair<string, string>("password", password),
            });

            using (HttpResponseMessage message = await apiClient.PostAsync("/token", data))
            {
                if (message.IsSuccessStatusCode)
                {
                    var response = await message.Content.ReadAsAsync<AuthenticatedUser>();
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
