namespace TDS_Coordinator_Application.TaskCoordinator.DB.Entities
{
    public class NodeData
    {
        public int Id { get; set; }
        public string NodeIpAddress { get; set; }
        public string NodePort { get; set; }
        public int NodeStatus { get; set; }
        public string HostName { get; set; }
        public string NodeGuid { get; set; }

        public int NodeExecutorType { get; set; }
    }
}
