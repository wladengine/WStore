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
        private readonly ISaleData _saleData;

        public SaleController(ISaleData saleData)
        {
            _saleData = saleData;
        }

        // POST api/Sale
        [HttpPost]
        [Authorize(Roles = "Cashier")]
        public void Post(SaleModel model)
        {
            string userId = User.FindFirstValue(ClaimTypes.NameIdentifier); //new way to get the user principal id

            _saleData.SaveSale(model, userId);
        }

        // GET api/Sale/GetSalesReport
        [HttpGet]
        [Authorize(Roles = "Manager,Admin")]
        [Route("GetSalesReport")]
        public List<SaleReportModel> GetSalesReport()
        {
            return _saleData.GetSaleReport();
        }
    }
}
