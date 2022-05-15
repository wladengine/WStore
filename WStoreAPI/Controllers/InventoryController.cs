using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
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
        private readonly IInventoryData _inventoryData;

        public InventoryController(IInventoryData inventoryData)
        {
            _inventoryData = inventoryData;
        }

        [HttpGet]
        [Authorize(Roles = "Manager,Admin")]
        public List<InventoryModel> GetInventoryData()
        {
            return _inventoryData.GetInventory();
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        [Authorize(Roles = "WarehouseWorker")]
        public void Post(InventoryModel model)
        {
            _inventoryData.SaveInventoryRecord(model);
        }
    }
}
