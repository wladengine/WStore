using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System.Collections.Generic;
using System.Security.Claims;
using WStoreDataManagement.Library.DataAccess;
using WStoreDataManagement.Library.Models;

namespace WStoreAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class SaleController : ControllerBase
    {
        private readonly IConfiguration _configuration;

        public SaleController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        [Authorize(Roles = "Cashier")]
        public void Post(SaleModel model)
        {
            string userId = User.FindFirstValue(ClaimTypes.NameIdentifier); //new way to get the user principal id

            SaleData data = new SaleData(_configuration);

            data.SaveSale(model, userId);
        }

        // GET api/Sale/GetSalesReport
        [Authorize(Roles = "Manager,Admin")]
        [Route("GetSalesReport")]
        public List<SaleReportModel> GetSalesReport()
        {
            //bool isAdmin = RequestContext.Principal.IsInRole("Admin");
            SaleData data = new SaleData(_configuration);

            return data.GetSaleReport();
        }
    }
}
