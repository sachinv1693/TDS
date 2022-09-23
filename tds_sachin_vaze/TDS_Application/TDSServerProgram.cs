using System;
using System.Net;
using TDS_Coordinator_Application.CommunicationLayer;
using System.Configuration;
using TDS_Coordinator_Application.TaskCoordinator.TaskDispatcher;
using System.Threading;

namespace TDS_Coordinator_Application
{
    class TDSServerProgram
    {
        static void Main()
        {
            string tdsServerIpAddress = ConfigurationManager.AppSettings["tdsServerIpAddress"];
            int tdsServerPort = Convert.ToInt32(ConfigurationManager.AppSettings["tdsServerPort"]);

            //Keep task dispatcher running on a separate thread
            TaskDispatcher dispatcher = new TaskDispatcher();
            Thread dispatcherThread = new Thread(dispatcher.DispatchTasks);
            dispatcherThread.Start();

            TDSServer server = new TDSServer(IPAddress.Parse(tdsServerIpAddress), tdsServerPort);
            server.StartListening();
        }
    }
}
