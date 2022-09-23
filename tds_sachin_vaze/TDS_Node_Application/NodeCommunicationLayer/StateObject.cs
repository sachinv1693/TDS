using System.Net;
using System.Net.Sockets;
using System.Text;

namespace TDS_Node_Application.NodeCommunicationLayer
{
    // State object for receiving data from remote device.
    public class StateObject
    {
        // Client socket.  
        public Socket workSocket = null;

        // Socket IpEndpoints
        public IPEndPoint remoteIpEndPoint = null;
        public IPEndPoint localIpEndPoint = null;

        // Size of receive buffer.
        public int BufferSize;

        // Receive buffer.  
        public byte[] buffer;

        // Received data string.  
        public StringBuilder stringBuilder = new StringBuilder();
    }
}
