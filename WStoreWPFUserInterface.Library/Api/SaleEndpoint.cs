using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using WStoreWPFUserInterface.Library.Models;

namespace WStoreWPFUserInterface.Library.Api
{
    public class SaleEndpoint : ISaleEndpoint
    {
        private IAPIHelper _apiHelper;
        public SaleEndpoint(IAPIHelper apiHelper)
        {
            _apiHelper = apiHelper;
        }

        public async Task<string> PostSale(SaleModel sale)
        {
            using (HttpResponseMessage message = await _apiHelper.HttpClient.PostAsJsonAsync<SaleModel>("/api/Sale", sale))
            {
                if (message.IsSuccessStatusCode)
                {
                    var response = await message.Content.ReadAsAsync<string>();
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
