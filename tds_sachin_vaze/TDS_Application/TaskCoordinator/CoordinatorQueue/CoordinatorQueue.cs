using System.Collections.Concurrent;
using TDS_Coordinator_Application.TaskCoordinator.DB.Entities;

namespace TDS_Coordinator_Application.TaskCoordinator.TaskCoordinatorQueue
{
    public class CoordinatorQueue
    {
        // statically mainatained Task cordinator queue
        public static ConcurrentQueue<TaskData> queue = new ConcurrentQueue<TaskData>();
    }
}
