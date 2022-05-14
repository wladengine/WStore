using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WStoreDataManagement.Library.Internal.DataAccess;
using WStoreDataManagement.Library.Models;

namespace WStoreDataManagement.Library.DataAccess
{
    public class InventoryData
    {
        public List<InventoryModel> GetInventory()
        {
            SQLDataAccess sql = new SQLDataAccess();

            var output = sql.LoadData<InventoryModel, dynamic>("dbo.spInventory_GetAll", new { }, "WStoreData");

            return output;
        }

        public void SaveInventoryRecord(InventoryModel model)
        {
            SQLDataAccess sql = new SQLDataAccess();

            sql.SaveData("dbo.spInventory_Insert", model, "WStoreData");
        }
    }
}
