using System.Net.Http;
using System.Threading.Tasks;
using WStoreWPFUserInterface.Library.Models;

namespace WStoreWPFUserInterface.Library.Api
{
    public interface IAPIHelper
    {
        HttpClient HttpClient { get; }
        Task<AuthenticatedUser> AuthenticateAsync(string userName, string password);
        Task GetLoggedInUserInfo(string token);
        void LogOffUser();
    }
}