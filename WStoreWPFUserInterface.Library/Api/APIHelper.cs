using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using WStoreWPFUserInterface.Library.Models;

namespace WStoreWPFUserInterface.Library.Api
{
    public class APIHelper : IAPIHelper
    {
        private HttpClient _apiClient;
        private ILoggedInUserModel _loggedInUser;

        public HttpClient HttpClient
        {
            get { return _apiClient; }
        }

        public APIHelper(ILoggedInUserModel loggedInUser)
        {
            InitializeClient();
            _loggedInUser = loggedInUser;
        }

        private void InitializeClient()
        {
            string apiUrl = ConfigurationManager.AppSettings["api"];

            _apiClient = new HttpClient
            {
                BaseAddress = new Uri(apiUrl)
            };

            _apiClient.DefaultRequestHeaders.Accept.Clear();
            _apiClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        }

        public void LogOffUser()
        {
            _apiClient.DefaultRequestHeaders.Clear();
        }

        public async Task<AuthenticatedUser> AuthenticateAsync(string userName, string password)
        {
            var data = new FormUrlEncodedContent(new[]
            {
                new KeyValuePair<string, string>("grant_type", "password"),
                new KeyValuePair<string, string>("username", userName),
                new KeyValuePair<string, string>("password", password),
            });

            using (HttpResponseMessage message = await _apiClient.PostAsync("/token", data))
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

        public async Task GetLoggedInUserInfo(string token)
        {
            _apiClient.DefaultRequestHeaders.Clear();
            _apiClient.DefaultRequestHeaders.Accept.Clear();
            _apiClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            _apiClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {token}");

            using (HttpResponseMessage message = await _apiClient.GetAsync("/api/User"))
            {
                if (message.IsSuccessStatusCode)
                {
                    var response = await message.Content.ReadAsAsync<LoggedInUserModel>();

                    // we making mapping because working with singleton
                    _loggedInUser.Token = token;
                    _loggedInUser.Id = response.Id;
                    _loggedInUser.FirstName = response.FirstName;
                    _loggedInUser.LastName = response.LastName;
                    _loggedInUser.EmailAddress = response.EmailAddress;
                    _loggedInUser.CreatedDate = response.CreatedDate;
                }
                else
                {
                    throw new Exception(message.ReasonPhrase);
                }
            }
        }
    }
}
