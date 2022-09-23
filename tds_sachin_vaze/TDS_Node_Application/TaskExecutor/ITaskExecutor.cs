using TDS_Node_Application.Entities;

namespace TDS_Node_Application.TaskExecution
{
    public interface ITaskExecutor
    {
        public TaskResult ExecuteTask(TaskData task);
    }
}
