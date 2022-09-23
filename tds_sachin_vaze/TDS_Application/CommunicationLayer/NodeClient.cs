using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using TDS_Coordinator_Application.Core.CommProtocol.Enums;
using TDS_Coordinator_Application.Core.CommProtocol.SerializationFactory;
using TDS_Coordinator_Application.Core.CommProtocol.TDSSerialization;
using TDS_Coordinator_Application.TaskCoordinator.DB.Entities;

namespace TDS_Coordinator_Application.CommunicationLayer
{
    public class NodeClient
    {
        public ITDSSerializer _serializer;
        public IPEndPoint _remoteEP;
        public Socket _clientSocket;

        public IPEndPoint _remoteIpEndPoint;
        public IPEndPoint _localIpEndPoint;

        private readonly int _connectionTimeoutInMilliseconds = 10000;

        public NodeClient(IPAddress nodeServerIpAddress, int nodeServerPort)
        {
            _serializer = SerializationFactory.GetSerializer(SerializerType.JSON);
            _remoteEP = new IPEndPoint(nodeServerIpAddress, nodeServerPort);
            _clientSocket = GetConfiguredClientSocket(nodeServerIpAddress);
        }

        // ManualResetEvent instances signal completion.  
        public ManualResetEvent connectDone = new ManualResetEvent(false);
        public ManualResetEvent sendDone = new ManualResetEvent(false);
        public ManualResetEvent receiveDone = new ManualResetEvent(false);

        // Get the serialized response from the remote device.  
        public string serverResponse = string.Empty;

        public TaskResult SendTask(TaskData task)
        {
            try
            {
                _clientSocket.BeginConnect(_remoteEP, new AsyncCallback(ConnectCallback), _clientSocket); // Connect to the remote endpoint.  
                if (!connectDone.WaitOne(_connectionTimeoutInMilliseconds))
                {
                    Console.WriteLine("\nSocket connection failed: " + TDSStatusCode.CONNECTION_TIMEOUT_ERROR.ToString());
                }

                string serializedTask = _serializer.Serialize(task);
                serializedTask += "<EOF>";
                
                SendSizeOfSerializedData(_clientSocket, serializedTask);
                sendDone.WaitOne();

                Send(_clientSocket, serializedTask); // Send base64 encoded task executable to the remote device.
                sendDone.WaitOne();

                Receive(_clientSocket); // Receive the serializedResponse from the remote device.
                receiveDone.WaitOne();
                Console.WriteLine($"Serialized Response received : {serverResponse}");

                TaskResult result = _serializer.DeSerializeAsTaskResult(serverResponse);
                serverResponse = string.Empty;
                return result;
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception.Message);
                return null;
            }
        }

        private void SendSizeOfSerializedData(Socket client, string serializedData)
        {
            int noOfBytes = Encoding.ASCII.GetByteCount(serializedData);
            byte[] bytes = BitConverter.GetBytes(noOfBytes);
            Send(client, Encoding.UTF8.GetString(bytes));
            Console.WriteLine($"Sent {sizeof(int)} bytes to Node server. Required buffer size = {noOfBytes} bytes.");
        }

        public Socket GetConfiguredClientSocket(IPAddress serverIpAddress)
        {
            Socket clientSocket = new Socket(serverIpAddress.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
            return clientSocket;
        }

        public void ReleaseClientSocket()
        {
            _clientSocket.Shutdown(SocketShutdown.Both);
            _clientSocket.Close();
        }

        public void ConnectCallback(IAsyncResult asyncResult)
        {
            try
            {
                Socket client = (Socket)asyncResult.AsyncState; // Retrieve the socket from the state object.
                  
                client.EndConnect(asyncResult); // Complete the connection.
                _remoteIpEndPoint = _clientSocket.RemoteEndPoint as IPEndPoint;
                _localIpEndPoint = _clientSocket.LocalEndPoint as IPEndPoint;
                Console.WriteLine($"Connected to server with IP: {_remoteIpEndPoint.Address}, Port: {_remoteIpEndPoint.Port}");

                connectDone.Set(); // Signal that the connection has been made.
            }
            catch (SocketException exception)
            {
                Console.WriteLine(exception.Message);
                Console.WriteLine($"Server did not respond: {(int)TDSStatusCode.SERVICE_UNAVAILABLE}: " + TDSStatusCode.SERVICE_UNAVAILABLE.ToString());
                Console.WriteLine("Trying to reconnect...");
                _clientSocket.BeginConnect(_remoteEP, new AsyncCallback(ConnectCallback), _clientSocket); // Connect to the remote endpoint.
            }
        }

        public void Receive(Socket client)
        {
            try
            {
                StateObject state = new StateObject() // Create the state object.
                {
                    workSocket = client,
                    remoteIpEndPoint = _clientSocket.RemoteEndPoint as IPEndPoint,
                    localIpEndPoint = _clientSocket.LocalEndPoint as IPEndPoint,
                    buffer = new byte[sizeof(int)]
                };
                // Begin receiving the data from the remote device.
                client.BeginReceive(state.buffer, 0, sizeof(int), 0, new AsyncCallback(ReceiveCallback), state);
            }
            catch (Exception exception)
            {
                throw new Exception(exception.Message);
            }
        }

        public void ReceiveCallback(IAsyncResult asyncResult)
        {
            try
            {
                // Retrieve the state object and the client socket from the asynchronous state object.  
                StateObject state = (StateObject)asyncResult.AsyncState;
                Socket client = state.workSocket;

                int bytesRead = client.EndReceive(asyncResult); // Read data from the remote device.

                if (bytesRead == sizeof(int))
                {
                    state.BufferSize = BitConverter.ToInt32(state.buffer, 0);
                    state.buffer = new byte[state.BufferSize];
                    client.BeginReceive(state.buffer, 0, state.BufferSize, 0, new AsyncCallback(ReceiveCallback), state);
                }
                else if (bytesRead > 0)
                {
                    // There might be more data, so store the data received so far.  
                    state.stringBuilder.Append(Encoding.ASCII.GetString(state.buffer, 0, bytesRead));

                    // Get the rest of the data.  
                    client.BeginReceive(state.buffer, 0, state.BufferSize, 0, new AsyncCallback(ReceiveCallback), state);
                }
                else
                {
                    // All the data has arrived; put it in serverResponse.  
                    if (state.stringBuilder.Length > 1)
                    {
                        serverResponse = state.stringBuilder.ToString();
                    }
                    receiveDone.Set(); // Signal that all bytes have been received.
                }
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception.Message);
                throw new Exception(exception.Message);
            }
        }

        public void Send(Socket client, string data)
        {
            byte[] byteData = Encoding.ASCII.GetBytes(data); // Convert the string data to byte data using ASCII encoding.
            client.BeginSend(byteData, 0, byteData.Length, 0, new AsyncCallback(SendCallback), client); // Begin sending the data to the remote device.
        }

        public void SendCallback(IAsyncResult asyncResult)
        {
            try
            {
                Socket client = (Socket)asyncResult.AsyncState; // Retrieve the socket from the state object.
                  
                int bytesSent = client.EndSend(asyncResult); // Complete sending data to the remote device.
                Console.WriteLine($"Sent {bytesSent} bytes to server.");
                sendDone.Set(); // Signal that all bytes have been sent.
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception.ToString());
            }
        }
    }
}
