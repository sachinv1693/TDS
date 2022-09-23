using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using TDS_Coordinator_Application.TaskCoordinator.DB.Enums;
using TDS_Coordinator_Application.TaskCoordinator.DB.Entities;

namespace TDS_Coordinator_Application.TaskCoordinator.DB.Repositories
{
    public class NodeRepository : IRepository<NodeData>, INodeRepository
    {
        private readonly IDatabaseManager<SqlDataReader> _databaseManager;

        public NodeRepository(IDatabaseManager<SqlDataReader> databaseManager)
        {
            _databaseManager = databaseManager;
        }

        public void Add(NodeData node)
        {
            try
            {
                string query = $"INSERT INTO Node(NodeIpAddress, NodePort, HostName, NodeGuid) VALUES ('{node.NodeIpAddress}', '{node.NodePort}', '{node.HostName}', '{node.NodeGuid}')";
                _databaseManager.ExecuteNonQuery(query);
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
                string query = $"DELETE FROM Node WHERE NodeGuid = '{guid}'";
                _databaseManager.ExecuteNonQuery(query);
                Console.WriteLine($"Node with guid {guid} has been deleted from database.");
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

        public IEnumerable<NodeData> GetAllNodes()
        {
            IEnumerable<NodeData> nodeList = null;
            try
            {
                string query = "SELECT * FROM Node";
                nodeList = GetNodeList(query);
            }
            catch (Exception exception)
            {
                throw new Exception(exception.Message);
            }
            finally
            {
                _databaseManager.CloseConnection();
            }
            return nodeList;
        }

        public void SetNodeStatus(NodeData node, NodeStatus nodeStatus)
        {
            try
            {
                string query = $"UPDATE Node SET NodeStatus = {(int)nodeStatus} WHERE Id = {node.Id}";
                _databaseManager.ExecuteNonQuery(query);
                Console.WriteLine($"Node {node.NodeIpAddress}: {node.NodePort} has been marked as {nodeStatus}");
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

        public IEnumerable<NodeData> GetAllAvailableNodes()
        {
            IEnumerable<NodeData> nodeList = null;
            try
            {
                string query = $"SELECT * FROM Node WHERE NodeStatus = {(int)NodeStatus.AVAILABLE}";
                nodeList = GetNodeList(query);
            }
            catch (Exception exception)
            {
                throw new Exception(exception.Message);
            }
            finally
            {
                _databaseManager.CloseConnection();
            }
            return nodeList;
        }

        private List<NodeData> GetNodeList(string query)
        {
            SqlDataReader reader = _databaseManager.ExecuteSelectQuery(query);
            List<NodeData> nodeList = new List<NodeData>();
            if (reader != null)
            {
                while (reader.Read())
                {
                    NodeData node = new NodeData()
                    {
                        Id = (reader["Id"].GetType() != typeof(DBNull)) ? Convert.ToInt32(reader["Id"]) : -1,
                        NodeIpAddress = (reader["NodeIpAddress"].GetType() != typeof(DBNull)) ? reader["NodeIpAddress"].ToString() : "",
                        NodePort = (reader["NodePort"].GetType() != typeof(DBNull)) ? reader["NodePort"].ToString() : "",
                        HostName = (reader["HostName"].GetType() != typeof(DBNull)) ? reader["HostName"].ToString() : "",
                        NodeStatus = (reader["NodeStatus"].GetType() != typeof(DBNull)) ? Convert.ToInt32(reader["NodeStatus"]) : -1,
                        NodeGuid = (reader["NodeGuid"].GetType() != typeof(DBNull)) ? reader["NodeGuid"].ToString() : "",
                        NodeExecutorType = (reader["NodeExecutorType"].GetType() != typeof(DBNull)) ? Convert.ToInt32(reader["NodeExecutorType"]) : -1
                    };
                    nodeList.Add(node);
                }
            }
            return nodeList;
        }
    }
}
