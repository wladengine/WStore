using System.Threading.Tasks;
using WStoreWPFUserInterface.Models;

namespace WStoreWPFUserInterface.Helpers
{
    public interface IAPIHelper
    {
        Task<AuthenticatedUser> AuthenticateAsync(string userName, string password);
    }
}