using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dapper;
using Microsoft.Extensions.Configuration;

namespace WStoreDataManagement.Library.Internal.DataAccess
{
    public class SQLDataAccess : IDisposable, ISQLDataAccess
    {
        private readonly IConfiguration _configuration;
        public SQLDataAccess(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public string GetConnectionString(string connectionStringName)
        {
            return _configuration.GetConnectionString(connectionStringName);
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

        public List<T> LoadDataInTransaction<T, U>(string storedProcedure, U parameters)
        {
            if (_dbTransaction != null && _dbConnection != null)
            {
                List<T> rows = _dbConnection.Query<T>(storedProcedure, parameters,
                    commandType: CommandType.StoredProcedure,
                    transaction: _dbTransaction).ToList();

                return rows;
            }
            else
                throw new NullReferenceException("Transaction is not opened");
        }
        public void SaveDataInTransation<T>(string storedProcedure, T parameters)
        {
            if (_dbTransaction != null && _dbConnection != null)
            {
                _dbConnection.Execute(storedProcedure, parameters,
                    commandType: CommandType.StoredProcedure, transaction: _dbTransaction);
            }
        }

        public IDbConnection _dbConnection { get; set; }
        public IDbTransaction _dbTransaction { get; set; }
        private bool _dbTransactionIsClosed = false;


        public void StartTransaction(string connectionStringName)
        {
            string connetionString = GetConnectionString(connectionStringName);

            _dbConnection = new SqlConnection(connetionString);
            _dbConnection.Open();

            _dbTransaction = _dbConnection.BeginTransaction();

            _dbTransactionIsClosed = false;
        }

        public void CommitTransaction()
        {
            _dbTransaction?.Commit();
            _dbConnection?.Close();

            _dbTransactionIsClosed = true;
        }

        public void RollbackTransaction()
        {
            _dbTransaction?.Rollback();
            _dbConnection?.Close();

            _dbTransactionIsClosed = true;
        }

        public void Dispose()
        {
            if (!_dbTransactionIsClosed)
            {
                try
                {
                    CommitTransaction();
                }
                catch
                {
                    // TODO: log this exception
                }
            }
        }
    }
}
