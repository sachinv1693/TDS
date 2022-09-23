using System;
using System.Data.SqlClient;
using System.Net;
using System.Net.Sockets;
using System.Text;
using TDS_Coordinator_Application.TaskCoordinator.DB;
using TDS_Coordinator_Application.TaskCoordinator.DB.Enums;
using TDS_Coordinator_Application.TaskCoordinator.DB.RepoFactory;

namespace TDS_Coordinator_Application.CommunicationLayer
{
    // State object for receiving data from remote device.
    public class StateObject
    {
        // Client socket.  
        public Socket workSocket = null;
        
        // Socket IpEndpoints
        public IPEndPoint remoteIpEndPoint = null;
        public IPEndPoint localIpEndPoint = null;
        
        // Current Client ID
        public int ClientID { get; set; }

        // Size of receive buffer.
        public int BufferSize;

        // Receive buffer.  
        public byte[] buffer;

        // Received data string.  
        public StringBuilder stringBuilder = new StringBuilder();

        public void SetClientId()
        {
            try
            {
                string clientIp = remoteIpEndPoint.Address.ToString();
                string clientPort = remoteIpEndPoint.Port.ToString();
                IDatabaseManager<SqlDataReader> databaseManager = new SqlDatabaseManager();
                dynamic dao = RepoFactory.GetRepository(ReopsitoryType.CLIENT, databaseManager);
                ClientID = dao.GetClientIdByClientNetworkAddress(clientIp, clientPort);
            }
            catch (Exception exception)
            {
                Console.WriteLine("Failed to fetch client ID details: " + exception.Message);
                ClientID = -1;
            }
        }
    }
}
