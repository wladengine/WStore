using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using WStoreDataManagement.Library.DataAccess;
using WStoreDataManagement.Library.Models;

namespace WStoreAPI.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class InventoryController : ControllerBase
    {
        // GET api/Sale/GetSalesReport
        [Route("GetInventoryData")]
        [Authorize(Roles = "Manager,Admin")]
        public List<InventoryModel> GetInventoryData()
        {
            InventoryData data = new InventoryData();

            return data.GetInventory();
        }

        [Authorize(Roles = "Admin")]
        [Authorize(Roles = "WarehouseWorker")]
        public void Post(InventoryModel model)
        {
            InventoryData data = new InventoryData();
            data.SaveInventoryRecord(model);
        }
    }
}
