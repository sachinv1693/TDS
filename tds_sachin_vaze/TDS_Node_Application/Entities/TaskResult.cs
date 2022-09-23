using TDS_Node_Application.Core.CommProtocol.Enums;

namespace TDS_Node_Application.Entities
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