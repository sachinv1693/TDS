using TDS_Coordinator_Application.CommunicationLayer;
using TDS_Coordinator_Application.Core.CommProtocol;

namespace TDS_Coordinator_Application.TaskCoordinator.Controllers
{
    public interface ITDSController
    {
        public TDSResponse ProcessRequest(TDSRequest request, StateObject state);
    }
}
