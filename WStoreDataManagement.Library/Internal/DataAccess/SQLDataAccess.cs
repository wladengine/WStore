using Dapper;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WStoreDataManagement.Library.Internal.DataAccess
{
    internal class SQLDataAccess
    {
        public string GetConnectionString(string connectionStringName)
        {
            return ConfigurationManager.ConnectionStrings[connectionStringName].ConnectionString;
        }

        public List<T> LoadData<T, U>(string storedProcedure, U parameters, string connectionStringName)
        {
            string connetionString = GetConnectionString(connectionStringName);

            using (IDbConnection conn = new SqlConnection(connetionString))
            {
                List<T> rows = conn.Query<T>(storedProcedure, parameters, 
                    commandType: CommandType.StoredProcedure).ToList();

                return rows;
            }
        }

        public void SaveData<T>(string storedProcedure, T parameters, string connectionStringName)
        {
            string connetionString = GetConnectionString(connectionStringName);

            using (IDbConnection conn = new SqlConnection(connetionString))
            {
                conn.Execute(storedProcedure, parameters,
                    commandType: CommandType.StoredProcedure);
            }
        }
    }
}
