using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Net;
using TDS_Coordinator_Application.TaskCoordinator.TaskCoordinatorQueue;
using TDS_Coordinator_Application.CommunicationLayer;
using TDS_Coordinator_Application.TaskCoordinator.DB;
using TDS_Coordinator_Application.TaskCoordinator.DB.Entities;
using TDS_Coordinator_Application.TaskCoordinator.DB.Enums;
using TDS_Coordinator_Application.TaskCoordinator.DB.RepoFactory;
using TDS_Coordinator_Application.TaskCoordinator.DB.Repositories;
using System.Threading;

namespace TDS_Coordinator_Application.TaskCoordinator.TaskDispatcher
{
    public class TaskDispatcher
    {
        private static IDatabaseManager<SqlDataReader> _databaseManager;
        private static dynamic _dao;

        public TaskDispatcher()
        {
            _databaseManager = new SqlDatabaseManager();
            _dao = RepoFactory.GetRepository(ReopsitoryType.NODE, _databaseManager);
        }
        
        public void DispatchTasks()
        {
            Console.WriteLine("Task Dispatcher is waiting for tasks to arrive in queue...");
            while (true)
            {
                if (CoordinatorQueue.queue.TryDequeue(out TaskData task))
                {
                    List<NodeData> availableNodes = _dao.GetAllAvailableNodes();
                    NodeData node = GetExecutableNode(availableNodes, task);
                    if (node == null)
                    {
                        CoordinatorQueue.queue.Enqueue(task);
                        continue;
                    }
                    Thread taskExecutorThread = new Thread(() => ExecuteTask(node, task));
                    taskExecutorThread.Start();
                }
            }
        }

        private void ExecuteTask(NodeData node, TaskData task)
        {
            _dao.SetNodeStatus(node, NodeStatus.BUSY);  // Update Node status before sending task to excute on it
            TaskRepository taskDAO = new TaskRepository(_databaseManager);
            taskDAO.AssignNode(node, task);
            // Get IP Address and Port No. of this node
            IPAddress nodeServerIpAddress = IPAddress.Parse(node.NodeIpAddress);
            int nodeServerPort = Convert.ToInt32(node.NodePort);

            NodeClient nodeClient = new NodeClient(nodeServerIpAddress, nodeServerPort);
            taskDAO.SetTaskStatus(task.TaskGuid, TaskStatus.EXECUTING); // Update TaskStatus
            TaskResult result = nodeClient.SendTask(task);
            UpdateTaskResultToDB(taskDAO, result);
            _dao.SetNodeStatus(node, NodeStatus.AVAILABLE); // Make the node available again
        }

        private NodeData GetExecutableNode(List<NodeData> availableNodes, TaskData task)
        {
            foreach (NodeData node in availableNodes)
            {
                if (IsTaskType(task, ".py") && IsNodeAbleToExcutePythonFile(node) ||
                    IsTaskType(task, ".cs") && IsNodeAbleToExecuteCSharpFile(node) ||
                    IsTaskType(task, ".exe") && IsNodeAbleToExcuteExeFile(node))
                {
                    return node;
                }
            }
            return null;
        }

        private bool IsNodeAbleToExcuteExeFile(NodeData node)
        {
            return node.NodeExecutorType == (int)NodeExecutionType.SIMPLE_EXECUTABLE_ONLY;
        }

        private bool IsTaskType(TaskData task, string fileType)
        {
            return task.TaskPath.EndsWith(fileType);
        }

        private bool IsNodeAbleToExecuteCSharpFile(NodeData node)
        {
            return 
                node.NodeExecutorType == (int)NodeExecutionType.CSHARP_ONLY ||
                node.NodeExecutorType == (int)NodeExecutionType.CSHARP_AND_PYTHON_ONLY;
        }

        private bool IsNodeAbleToExcutePythonFile(NodeData node)
        {
            return 
                node.NodeExecutorType == (int)NodeExecutionType.PYTHON_ONLY ||
                node.NodeExecutorType == (int)NodeExecutionType.CSHARP_AND_PYTHON_ONLY;
        }

        private static void UpdateTaskResultToDB(TaskRepository taskDAO, TaskResult result)
        {
            taskDAO.SetTaskResult(result.TaskGuid, result.TaskOutcome);
            taskDAO.SetTaskState(result.TaskGuid, result.TaskState);
            taskDAO.SetTaskStatus(result.TaskGuid, result.TaskStatus);
        }
    }
}
