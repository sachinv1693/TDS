using TDS_Coordinator_Application.TaskCoordinator.DB.Enums;

namespace TDS_Coordinator_Application.TaskCoordinator.DB.Entities
{
    public class TaskResult
    {
        public string ErrorCode { get; set; }
        public string ErrorMessage { get; set; }
        public string TaskGuid { get; set; }
        public string TaskOutcome { get; set; }
        public TaskState TaskState { get; set; }
        public TaskStatus TaskStatus { get; set; }
    }
}