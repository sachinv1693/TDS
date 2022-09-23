using TDS_Coordinator_Application.Core.CommProtocol;
using TDS_Coordinator_Application.TaskCoordinator.Controllers;

namespace TDS_Coordinator_Application.TaskCoordinator.RequestDispatcher
{
    public class RequestDispatcher
    {
		public static ITDSController GetController(TDSRequest request)
		{
			if (IsRequestMethodStartingWith(request, "node-"))
			{
				return new NodeController();
			}
			else if (IsRequestMethodStartingWith(request, "client-"))
            {
				return new ClientController();
            }
			else if (IsRequestMethodStartingWith(request, "task-"))
			{
				return new TaskController();
			}
			else
            {
				return null;
            }
		}

		private static bool IsRequestMethodStartingWith(TDSRequest request, string method)
        {
			return request.GetMethod().StartsWith(method);
		}
	}
}
