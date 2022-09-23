using System.Collections.Generic;
using TDS_Coordinator_Application.TaskCoordinator.DB.Enums;
using TDS_Coordinator_Application.TaskCoordinator.DB.Entities;

namespace TDS_Coordinator_Application.TaskCoordinator.DB.Repositories
{
    interface ITaskRepository
    {
        void SetTaskStatus(string taskGuid, TaskStatus TaskStatus);
        void SetTaskResult(string taskGuid, string taskResult);
        IEnumerable<TaskData> GetTasksByClientId(int ClientId);
        TaskData GetTaskByGuid(string taskId);
        IEnumerable<TaskData> GetTasksByStatus(TaskStatus TaskStatus);
        IEnumerable<TaskData> GetTasksByNodeId(int nodeId);
        void AssignNode(NodeData node, TaskData task);
    }
}
