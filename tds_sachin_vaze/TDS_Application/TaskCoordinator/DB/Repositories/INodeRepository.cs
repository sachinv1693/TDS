using System.Collections.Generic;
using TDS_Coordinator_Application.TaskCoordinator.DB.Entities;
using TDS_Coordinator_Application.TaskCoordinator.DB.Enums;

namespace TDS_Coordinator_Application.TaskCoordinator.DB.Repositories
{
    public interface INodeRepository
    {
        // Methods specific to NodeData repository
        void SetNodeStatus(NodeData node, NodeStatus nodeStatus);
        IEnumerable<NodeData> GetAllNodes();
        IEnumerable<NodeData> GetAllAvailableNodes();
    }
}
