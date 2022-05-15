using System.Collections.Generic;
using System.Data;

namespace WStoreDataManagement.Library.Internal.DataAccess
{
    public interface ISQLDataAccess
    {
        IDbConnection _dbConnection { get; set; }
        IDbTransaction _dbTransaction { get; set; }

        void CommitTransaction();
        void Dispose();
        string GetConnectionString(string connectionStringName);
        List<T> LoadData<T, U>(string storedProcedure, U parameters, string connectionStringName);
        List<T> LoadDataInTransaction<T, U>(string storedProcedure, U parameters);
        void RollbackTransaction();
        void SaveData<T>(string storedProcedure, T parameters, string connectionStringName);
        void SaveDataInTransation<T>(string storedProcedure, T parameters);
        void StartTransaction(string connectionStringName);
    }
}