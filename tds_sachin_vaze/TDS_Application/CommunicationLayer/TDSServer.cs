using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using TDS_Coordinator_Application.Core.CommProtocol;
using TDS_Coordinator_Application.Core.CommProtocol.Enums;
using TDS_Coordinator_Application.Core.CommProtocol.SerializationFactory;
using TDS_Coordinator_Application.Core.CommProtocol.TDSSerialization;
using TDS_Coordinator_Application.TaskCoordinator.Controllers;
using TDS_Coordinator_Application.TaskCoordinator.RequestDispatcher;

namespace TDS_Coordinator_Application.CommunicationLayer
{
    public class TDSServer
    {
        public ManualResetEvent allDone = new ManualResetEvent(false); // Thread signal.

        public ITDSSerializer _serializer;
        public IPEndPoint _localEndPoint;
        public Socket _serverSocket;
      
        public TDSServer(IPAddress serverIpAddress, int port)
        {
            _serializer = SerializationFactory.GetSerializer(SerializerType.JSON);
            _localEndPoint = new IPEndPoint(serverIpAddress, port);
            _serverSocket = new Socket(serverIpAddress.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
        }

        public void StartListening()
        {
            try
            {
                _serverSocket.Bind(_localEndPoint); // Bind the socket to the local endpoint and listen for incoming connections.
                _serverSocket.Listen(100);
                while (true)
                {
                    allDone.Reset(); // Set the event to nonsignaled state.
                    Console.WriteLine("TDS Server waiting for a connection...");
                    _serverSocket.BeginAccept(new AsyncCallback(AcceptCallback), _serverSocket); // Start an asynchronous socket to listen for connections. 
                    allDone.WaitOne(); // Wait until a connection is made before continuing.
                }
            }
            catch (Exception exception)
            {
                throw new Exception(exception.Message);
            }
        }

        public void AcceptCallback(IAsyncResult asyncResult)
        {
            allDone.Set(); // Signal the main thread to continue.

            // Get the socket that handles the client request.  
            Socket listener = (Socket)asyncResult.AsyncState;
            Socket handler = listener.EndAccept(asyncResult);
              
            StateObject state = new StateObject() // Create the state object.
            {
                workSocket = handler,
                remoteIpEndPoint = handler.RemoteEndPoint as IPEndPoint,
                localIpEndPoint = handler.LocalEndPoint as IPEndPoint,
                buffer = new byte[sizeof(int)]
            };
            state.SetClientId(); // Fetch the client ID from DB for given client's network credentials
            Console.WriteLine($"Connected to client IP: {state.remoteIpEndPoint.Address}, Port: {state.remoteIpEndPoint.Port}. Client ID: {GetStateClientId(state)}");
            handler.BeginReceive(state.buffer, 0, sizeof(int), 0, new AsyncCallback(ReadCallback), state);
        }

        private string GetStateClientId(StateObject state)
        {
            return (state.ClientID == -1) ? "-1 (Unknown Client)" : state.ClientID.ToString();
        }

        public void ReadCallback(IAsyncResult asyncResult)
        {
            TDSResponse response = new TDSResponse();
            string serializedRequest = string.Empty;
            string serializedResponse = string.Empty;

            // Retrieve the state object and the handler socket from the asynchronous state object.  
            StateObject state = (StateObject)asyncResult.AsyncState;
            Socket handler = state.workSocket;
            
            int bytesRead = handler.EndReceive(asyncResult); // Read data from the client socket.

            if (bytesRead == sizeof(int) && state.ClientID != -1)
            {
                state.BufferSize = BitConverter.ToInt32(state.buffer, 0);
                state.buffer = new byte[state.BufferSize];
                // Not all data received. Get more.
                handler.BeginReceive(state.buffer, 0, state.BufferSize, 0, new AsyncCallback(ReadCallback), state);
            }
            else if (bytesRead > 0 && state.ClientID != -1)
            {
                // There  might be more data, so store the data received so far.  
                state.stringBuilder.Append(Encoding.ASCII.GetString(state.buffer, 0, bytesRead));

                // Check for end-of-file tag. If it is not there, read more data.  
                serializedRequest = state.stringBuilder.ToString();
                 
                if (serializedRequest.IndexOf("<EOF>") > -1)
                {
                    // All the data has been read from the client. Display it on the console.  
                    serializedRequest = serializedRequest.Replace("<EOF>", string.Empty);
                    Console.WriteLine($"Read {serializedRequest.Length} bytes from socket. \nRequest Data : {serializedRequest}");
                    TDSRequest request = _serializer.DeSerializeAsRequest(serializedRequest);
                    
                    ITDSController controller = RequestDispatcher.GetController(request);
                    if (controller != null)
                    {
                        response = controller.ProcessRequest(request, state);
                    }
                    serializedResponse = _serializer.Serialize(response);
                    SendSizeOfSerializedData(handler, serializedResponse);

                    Send(handler, serializedResponse);
                    ReleaseHandlerSocket(handler);
                }
                else
                {
                    handler.BeginReceive(state.buffer, 0, state.BufferSize, 0, new AsyncCallback(ReadCallback), state);
                }
            }
            else
            {
                Console.WriteLine($"{(int)TDSStatusCode.UNAUTHORIZED}: {TDSStatusCode.UNAUTHORIZED} TDS client.");
                ReleaseHandlerSocket(handler);
            }
        }

        private void SendSizeOfSerializedData(Socket handler, string serializedData)
        {
            int noOfBytes = Encoding.ASCII.GetByteCount(serializedData);
            byte[] bytes = BitConverter.GetBytes(noOfBytes);
            Send(handler, Encoding.UTF8.GetString(bytes));
        }

        public void Send(Socket handler, string data)
        {
            byte[] byteData = Encoding.ASCII.GetBytes(data); // Convert the string data to byte data using ASCII encoding.
            handler.BeginSend(byteData, 0, byteData.Length, 0, new AsyncCallback(SendCallback), handler); // Begin sending the data to the remote device.
        }

        public void SendCallback(IAsyncResult asyncResult)
        {
            try
            {
                Socket handler = (Socket)asyncResult.AsyncState; // Retrieve the socket from the state object. 

                int bytesSent = handler.EndSend(asyncResult); // Complete sending the data to the remote device.
                Console.WriteLine($"Sent {bytesSent} bytes to client.");
            }
            catch (Exception exception)
            {
                throw new Exception(exception.Message);
            }
        }

        public void ReleaseHandlerSocket(Socket handler)
        {
            handler.Shutdown(SocketShutdown.Both);
            handler.Close();
        }
    }
}
