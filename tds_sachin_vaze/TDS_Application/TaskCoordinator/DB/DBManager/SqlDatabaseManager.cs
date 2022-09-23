using System;
using System.Configuration;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;

namespace TDS_Coordinator_Application.TaskCoordinator.DB
{
    public class SqlDatabaseManager : IDatabaseManager<SqlDataReader>
    {
        private readonly string _dataProvider;
        private readonly DbProviderFactory _dbProviderFactory;
        private readonly DbConnection _connection;

        public SqlDatabaseManager()
        {
            DbProviderFactories.RegisterFactory("System.Data.SqlClient", SqlClientFactory.Instance); // Configuring here as it's not globally configured in .NET
            _dataProvider = ConfigurationManager.AppSettings["sqlDataProvider"];
            _dbProviderFactory = DbProviderFactories.GetFactory(_dataProvider);
            _connection = _dbProviderFactory.CreateConnection();
            _connection.ConnectionString = ConfigurationManager.AppSettings["sqlServerConnectionString"];
        }

        private void OpenConnection()
        {
            if (_connection != null && _connection.State == ConnectionState.Closed)
                _connection.Open();
        }

        public void CloseConnection()
        {
            if (_connection != null && _connection.State == ConnectionState.Open)
                _connection.Close();
        }

        public void ExecuteNonQuery(string query)
        {
            try
            {
                OpenConnection();
                var command = _dbProviderFactory.CreateCommand();
                SetCommand(command, query);
                command.ExecuteNonQuery();
            }
            catch (Exception exception)
            {
                throw new Exception(exception.Message);
            }
        }

        public SqlDataReader ExecuteSelectQuery(string query)
        {
            try
            {
                OpenConnection();
                var command = _dbProviderFactory.CreateCommand();
                SetCommand(command, query);
                return (SqlDataReader)command.ExecuteReader();
            }
            catch (Exception exception)
            {
                throw new Exception(exception.Message);
            }
        }

        private void SetCommand(DbCommand command, string query)
        {
            command.Connection = _connection;
            command.CommandText = query;
        }
    }
}
