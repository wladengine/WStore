﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using WStoreDataManagement.Library.DataAccess;
using WStoreDataManagement.Library.Models;

namespace WStoreDataManagement.Controllers
{
    [Authorize]
    public class InventoryController : ApiController
    {
        // GET api/Sale/GetSalesReport
        [Route("GetInventoryData")]
        public List<InventoryModel> GetInventoryData()
        {
            InventoryData data = new InventoryData();

            return data.GetInventory();
        }

        public void Post(InventoryModel model)
        {
            InventoryData data = new InventoryData();
            data.SaveInventoryRecord(model);
        }
    }
}
