using System.Threading.Tasks;
using WStoreWPFUserInterface.Library.Models;

namespace WStoreWPFUserInterface.Library.Api
{
    public interface ISaleEndpoint
    {
        Task<string> PostSale(SaleModel sale);
    }
}