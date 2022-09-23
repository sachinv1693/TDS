using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using TDS_Coordinator_Application.TaskCoordinator.DB.Entities;

namespace TDS_Coordinator_Application.TaskCoordinator.DB.Repositories
{
    public class ClientRepository : IRepository<ClientData>, IClientRepository
    {
        private readonly IDatabaseManager<SqlDataReader> _databaseManager;

        public ClientRepository(IDatabaseManager<SqlDataReader> databaseManager)
        {
            _databaseManager = databaseManager; 
        }

        public void Add(ClientData client)
        {
            try
            {
                string query = $"INSERT INTO Client(ClientIpAddress, ClientPort, HostName, ClientGuid) VALUES ('{client.ClientIpAddress}', '{client.ClientPort}', '{client.HostName}', '{client.ClientGuid}')";
                _databaseManager.ExecuteNonQuery(query);
                Console.WriteLine("Client has been added to database.");
            }
            catch (Exception exception)
            {
                throw new Exception(exception.Message);
            }
            finally
            {
                _databaseManager.CloseConnection();
            }
        }

        public void DeleteByGuid(string guid)
        {
            try
            {
                string query = $"DELETE FROM Client WHERE ClientGuid = '{guid}'";
                _databaseManager.ExecuteNonQuery(query);
                Console.WriteLine($"Client with guid {guid} has been deleted from database.");
            }
            catch (Exception exception)
            {
                throw new Exception(exception.Message);
            }
            finally
            {
                _databaseManager.CloseConnection();
            }
        }

        public int GetClientIdByClientNetworkAddress(string clientIpAddress, string clientPort)
        {
            string query = $"SELECT * FROM Client WHERE ClientIpAddress = '{clientIpAddress}' AND ClientPort = '{clientPort}'";
            ClientData client = GetClient(query);
            return (client != null) ? client.Id : -1;
        }

        private ClientData GetClient(string query)
        {
            ClientData client = null;
            try
            {
                var clientList = GetClientList(query);
                if (clientList.Count != 0)
                {
                    client = clientList[0];
                }
                else
                {
                    Console.WriteLine($"Client was not found in database.");
                }
            }
            catch (Exception exception)
            {
                Console.WriteLine("Exception: " + exception.ToString());
                Console.WriteLine($"Failed to retrieve client from database");
            }
            finally
            {
                _databaseManager.CloseConnection();
            }
            return client;
        }

        public ClientData GetClientById(int clientId)
        {
            string query = $"SELECT * FROM Client WHERE Id = {clientId}";
            ClientData client = GetClient(query);
            return client;
        }

        public IEnumerable<ClientData> GetAllClients()
        {
            IEnumerable<ClientData> clientList = null;
            try
            {
                string query = "SELECT * FROM Client";
                clientList = GetClientList(query);
            }
            catch (Exception exception)
            {
                Console.WriteLine("Exception: " + exception.ToString());
                Console.WriteLine($"Failed to retrieve all clients from database");
            }
            finally
            {
                _databaseManager.CloseConnection();
            }
            return clientList;
        }

        private List<ClientData> GetClientList(string query)
        {
            List<ClientData> clientList = new List<ClientData>();
            SqlDataReader reader = _databaseManager.ExecuteSelectQuery(query);
            if (reader != null)
            {
                while (reader.Read())
                {
                    ClientData client = new ClientData()
                    {
                        Id = (reader["Id"].GetType() != typeof(DBNull)) ? Convert.ToInt32(reader["Id"]) : -1,
                        ClientIpAddress = (reader["ClientIpAddress"].GetType() != typeof(DBNull)) ? reader["ClientIpAddress"].ToString() : "",
                        ClientPort = (reader["ClientPort"].GetType() != typeof(DBNull)) ? reader["ClientPort"].ToString() : "",
                        HostName = (reader["HostName"].GetType() != typeof(DBNull)) ? reader["HostName"].ToString() : "",
                        NoOfTasksGenerated = (reader["ClientPort"].GetType() != typeof(DBNull)) ? Convert.ToInt32(reader["NoOfTasksGenerated"]) : -1,
                        ClientGuid = (reader["ClientGuid"].GetType() != typeof(DBNull)) ? reader["ClientGuid"].ToString() : ""
                    };
                    clientList.Add(client);
                }
            }
            return clientList;
        }
    }
}
