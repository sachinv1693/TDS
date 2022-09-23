using System;
using TDS_Coordinator_Application.TaskCoordinator.DB.Enums;
using TDS_Coordinator_Application.TaskCoordinator.DB.Repositories;

namespace TDS_Coordinator_Application.TaskCoordinator.DB.RepoFactory
{
    public class RepoFactory
    {
        public static object GetRepository(ReopsitoryType daoType, dynamic databaseManager)
        {
            return daoType switch
            {
                ReopsitoryType.CLIENT => new ClientRepository(databaseManager),
                ReopsitoryType.TASK => new TaskRepository(databaseManager),
                ReopsitoryType.NODE => new NodeRepository(databaseManager),
                _ => throw new InvalidOperationException("Invalid Reopsitory type specified."),
            };
        }
    }
}
