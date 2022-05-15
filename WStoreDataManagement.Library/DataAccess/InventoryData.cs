using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WStoreDataManagement.Library.Internal.DataAccess;
using WStoreDataManagement.Library.Models;

namespace WStoreDataManagement.Library.DataAccess
{
    public class InventoryData : IInventoryData
    {
        private readonly IConfiguration _configuration;
        private readonly ISQLDataAccess _sql;

        public InventoryData(IConfiguration configuration, ISQLDataAccess sql)
        {
            _configuration = configuration;
            _sql = sql;
        }

        public List<InventoryModel> GetInventory()
        {
            var output = _sql.LoadData<InventoryModel, dynamic>("dbo.spInventory_GetAll", new { }, "WStoreData");

            return output;
        }

        public void SaveInventoryRecord(InventoryModel model)
        {
            _sql.SaveData("dbo.spInventory_Insert", model, "WStoreData");
        }
    }
}
