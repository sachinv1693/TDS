using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using TDS_Coordinator_Application.TaskCoordinator.DB.Enums;
using TDS_Coordinator_Application.TaskCoordinator.DB.Entities;

namespace TDS_Coordinator_Application.TaskCoordinator.DB.Repositories
{
    public class TaskRepository : IRepository<TaskData>, ITaskRepository
    {
        private readonly IDatabaseManager<SqlDataReader> _databaseManager;

        public TaskRepository(IDatabaseManager<SqlDataReader> databaseManager)
        {
            _databaseManager = databaseManager;
        }

        public void Add(TaskData task)
        {
            try
            {
                // A trigger has been set to increment noOfTasksGenerated field on successful insert
                string query = $"INSERT INTO Task(ClientId, TaskPath, TaskGuid) VALUES ('{task.ClientId}', '{task.TaskPath}', '{task.TaskGuid}')";
                _databaseManager.ExecuteNonQuery(query);
                Console.WriteLine("Task has been added to databsase.");
            }
            catch (Exception exception)
            {
                Console.WriteLine("Exception: " + exception.ToString());
                Console.WriteLine("Failed to insert new task in database");
            }
            finally
            {
                _databaseManager.CloseConnection();
            }
        }

        public void SetTaskResult(string taskGuid, string taskResult)
        {
            try
            {
                string query = $"UPDATE Task SET TaskResult = '{taskResult}' WHERE taskGuid = '{taskGuid}'";
                _databaseManager.ExecuteNonQuery(query);
                Console.WriteLine("Task result has been updated to database.");
            }
            catch (Exception exception)
            {
                Console.WriteLine("Exception: " + exception.ToString());
                Console.WriteLine("Failed to update task status in database");
            }
            finally
            {
                _databaseManager.CloseConnection();
            }
        }

        public void SetTaskState(string taskGuid, TaskState state)
        {
            try
            {
                string query = $"UPDATE Task SET isSuccess = '{(int)state}' WHERE taskGuid = '{taskGuid}'";
                _databaseManager.ExecuteNonQuery(query);
                Console.WriteLine("Task state has been updated to database.");
            }
            catch (Exception exception)
            {
                Console.WriteLine("Exception: " + exception.ToString());
                Console.WriteLine("Failed to update task state in database");
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
                string query = $"DELETE FROM Task WHERE taskGuid = '{guid}'";
                _databaseManager.ExecuteNonQuery(query);
                Console.WriteLine($"Task with guid {guid} has been deleted from database.");
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

        public void SetTaskStatus(string taskGuid, TaskStatus TaskStatus)
        {
            try
            {
                string query = $"UPDATE Task SET TaskStatus = {(int)TaskStatus} WHERE taskGuid = '{taskGuid}'";
                _databaseManager.ExecuteNonQuery(query);
                Console.WriteLine($"Task status has been updated to database.");
            }
            catch (Exception exception)
            {
                Console.WriteLine("Exception: " + exception.ToString());
                Console.WriteLine("Failed to set task status in database");
            }
            finally
            {
                _databaseManager.CloseConnection();
            }
        }

        public IEnumerable<TaskData> GetTasksByClientId(int ClientId)
        {
            IEnumerable<TaskData> tasksList = null;
            try
            {
                string query = $"SELECT * FROM Task WHERE ClientId = {ClientId}";
                tasksList = GetTaskList(query);
            }
            catch (Exception exception)
            {
                Console.WriteLine("Exception: " + exception.ToString());
                Console.WriteLine("Failed to retrieve tasks by client id from database");
            }
            finally
            {
                _databaseManager.CloseConnection();
            }
            return tasksList;
        }

        public TaskData GetTaskByGuid(string taskGuid)
        {
            TaskData task = null;
            try
            {
                string query = $"SELECT * FROM Task WHERE taskGuid = '{taskGuid}'";
                var tasksList = GetTaskList(query);
                if (tasksList.Count != 0)
                {
                    task = tasksList[0];
                }
                else
                {
                    Console.WriteLine($"Invalid task GUID. No task was found for GUID: {taskGuid}");
                }
            }
            catch (Exception exception)
            {
                Console.WriteLine("Exception: " + exception.ToString());
                Console.WriteLine($"Failed to retrieve task with task GUID-{taskGuid} from database");
            }
            finally
            {
                _databaseManager.CloseConnection();
            }
            return task;
        }

        public IEnumerable<TaskData> GetTasksByStatus(TaskStatus taskStatus)
        {
            IEnumerable<TaskData> tasksList = null; 
            try
            {
                string query = $"SELECT * FROM Task WHERE TaskStatus = {(int)taskStatus}";
                tasksList = GetTaskList(query);
            }
            catch (Exception exception)
            {
                Console.WriteLine("Exception: " + exception.ToString());
                Console.WriteLine("Failed to retrieve tasks by status from database");
            }
            finally
            {
                _databaseManager.CloseConnection();
            }
            return tasksList;
        }

        public IEnumerable<TaskData> GetTasksByNodeId(int nodeId)
        {
            IEnumerable<TaskData> tasksList = null; 
            try
            {
                string query = $"SELECT * FROM Task INNER JOIN TaskToNodeManager ON TaskData.Id = TaskToNodeManager.TaskId WHERE TaskToNodeManager.NodeId = {nodeId}";
                tasksList = GetTaskList(query);
            }
            catch (Exception exception)
            {
                Console.WriteLine("Exception: " + exception.ToString());
                Console.WriteLine($"Failed to retrieve tasks by node id - {nodeId} from the database");
            }
            finally
            {
                _databaseManager.CloseConnection();
            }
            return tasksList;
        }

        public void AssignNode(NodeData node, TaskData task)
        {
            try
            {
                if (node.NodeStatus == (int)NodeStatus.AVAILABLE)
                {
                    string query = $"INSERT INTO TaskToNodeManager(TaskId, NodeId) VALUES ('{task.Id}', '{node.Id}')";
                    _databaseManager.ExecuteNonQuery(query);
                    Console.WriteLine($"Assigned task with task id {task.Id} to node with node id {node.Id}.");
                }
                else
                {
                    Console.WriteLine($"Node with node id = {node.Id} is not available. Cannot assign it for the task!");
                }
            }
            catch (Exception exception)
            {
                Console.WriteLine("Exception: " + exception.ToString());
                Console.WriteLine("Failed to assign node for the task!");
            }
            finally
            {
                _databaseManager.CloseConnection();
            }
        }

        private List<TaskData> GetTaskList(string query)
        {
            List<TaskData> taskList = new List<TaskData>();
            SqlDataReader reader = _databaseManager.ExecuteSelectQuery(query);
            if (reader != null)
            {
                while (reader.Read())
                {
                    TaskData task = new TaskData()
                    {
                        Id = (reader["Id"].GetType() != typeof(DBNull)) ? Convert.ToInt32(reader["Id"]) : -1,
                        ClientId = (reader["ClientId"].GetType() != typeof(DBNull)) ? Convert.ToInt32(reader["ClientId"]) : -1,
                        TaskPath = (reader["TaskPath"].GetType() != typeof(DBNull)) ? reader["TaskPath"].ToString() : "",
                        TaskStatus = (reader["TaskStatus"].GetType() != typeof(DBNull)) ? Convert.ToInt32(reader["TaskStatus"]) : -1,
                        TaskResult = (reader["TaskResult"].GetType() != typeof(DBNull)) ? reader["TaskResult"].ToString() : "",
                        IsSuccess = (reader["IsSuccess"].GetType() != typeof(DBNull)) && Convert.ToBoolean(reader["IsSuccess"]),
                        TaskGuid = (reader["TaskGuid"].GetType() != typeof(DBNull)) ? reader["TaskGuid"].ToString() : "",
                    };
                    taskList.Add(task);
                }
            }
            return taskList;
        }
    }
}
