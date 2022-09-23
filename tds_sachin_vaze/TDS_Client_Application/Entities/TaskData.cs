namespace TDS_Client_Application.Entities
{
    public class TaskData
    {
        public int Id { get; set; }
        public int ClientId { get; set; }
        public string TaskPath { get; set; }
        public int TaskStatus { get; set; }
        public string TaskResult { get; set; }
        public bool IsSuccess { get; set; }
        public string TaskGuid { get; set; }

        public string TaskExecutable { get; set; }
    } 
}
