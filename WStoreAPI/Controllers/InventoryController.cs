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
        private readonly IConfiguration _configuration;

        public InventoryController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        // GET api/Sale/GetSalesReport
        [Route("GetInventoryData")]
        [Authorize(Roles = "Manager,Admin")]
        public List<InventoryModel> GetInventoryData()
        {
            InventoryData data = new InventoryData(_configuration);

            return data.GetInventory();
        }

        [Authorize(Roles = "Admin")]
        [Authorize(Roles = "WarehouseWorker")]
        public void Post(InventoryModel model)
        {
            InventoryData data = new InventoryData(_configuration);
            data.SaveInventoryRecord(model);
        }
    }
}
