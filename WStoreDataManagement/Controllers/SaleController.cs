using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using WStoreDataManagement.Library.DataAccess;
using WStoreDataManagement.Library.Models;
using WStoreDataManagement.Models;

namespace WStoreDataManagement.Controllers
{
    [Authorize]
    public class SaleController : ApiController
    {
        [Authorize(Roles = "Cashier")]
        public void Post(SaleModel model)
        {
            string userId = RequestContext.Principal.Identity.GetUserId();

            SaleData data = new SaleData();

            data.SaveSale(model, userId);
        }

        // GET api/Sale/GetSalesReport
        [Authorize(Roles = "Manager,Admin")]
        [Route("GetSalesReport")]
        public List<SaleReportModel> GetSalesReport()
        {
            bool isAdmin = RequestContext.Principal.IsInRole("Admin");
            SaleData data = new SaleData();

            return data.GetSaleReport();
        }
    }
}
