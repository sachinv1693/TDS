using System.Collections.Generic;
using TDS_Coordinator_Application.TaskCoordinator.DB.Entities;

namespace TDS_Coordinator_Application.TaskCoordinator.DB.Repositories
{
    public interface IClientRepository
    {
        ClientData GetClientById(int clientId);
        IEnumerable<ClientData> GetAllClients();
    }
}
