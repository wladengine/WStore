using System.Collections.Generic;
using System.Threading.Tasks;
using WStoreWPFUserInterface.Library.Models;

namespace WStoreWPFUserInterface.Library.Api
{
    public interface IProductEndpoint
    {
        Task<List<ProductModel>> GetAllAsync();
    }
}