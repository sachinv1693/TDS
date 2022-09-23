namespace TDS_Coordinator_Application.TaskCoordinator.DB.Entities
{
    public class ClientData
    {
        public int Id { get; set; }
        public string ClientIpAddress { get; set; }
        public string ClientPort { get; set; }
        public int NoOfTasksGenerated { get; set; }
        public string HostName { get; set; }
        public string ClientGuid { get; set; }
    }
}
