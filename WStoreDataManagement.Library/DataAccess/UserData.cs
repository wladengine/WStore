using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WStoreDataManagement.Library.Internal.DataAccess;
using WStoreDataManagement.Library.Models;

namespace WStoreDataManagement.Library.DataAccess
{
    public class UserData
    {
        public List<UserModel> GetUserById(string Id)
        {
            //TODO: create a dependency injection against theese direct dependencies
            SQLDataAccess sql = new SQLDataAccess();

            var p = new { Id = Id };

            var output = sql.LoadData<UserModel, dynamic>("dbo.spUserLookup", p, "WStoreData");
            
            return output;
        }
    }
}
