using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using WStoreDataManagement.Library.DataAccess;
using WStoreDataManagement.Library.Models;

namespace WStoreAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "Cashier")]
    public class ProductController : ControllerBase
    {
        public List<ProductModel> Get()
        {
            //TODO: make a DI against the direct dependency
            ProductData data = new ProductData();

            var output = data.GetProducts();

            return output;
        }
    }
}
