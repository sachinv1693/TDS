using TDS_Coordinator_Application.TaskCoordinator.DB.Entities;
using TDS_Coordinator_Application.TaskCoordinator.TaskCoordinatorQueue;

namespace TDS_Coordinator_Application.Core.TaskScheduler
{
    public class TaskScheduler
    {
        public void StartScheduler(TaskData task)
        {
            // Enqueue task in the queue and make sure to update taskStatus
            CoordinatorQueue.queue.Enqueue(task);
        }
    }
}
