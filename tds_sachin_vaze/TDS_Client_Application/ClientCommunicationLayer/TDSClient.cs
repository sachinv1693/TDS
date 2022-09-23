using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Text;
using TDS_Client_Application.Core.CommProtocol.SerializationFactory;
using TDS_Client_Application.Core.CommProtocol.TDSSerialization;
using TDS_Client_Application.Core.CommProtocol.Enums;
using TDS_Client_Application.Core.CommProtocol;
using System.Configuration;

namespace TDS_Client_Application.ClientCommunicationLayer
{
    public class TDSClient
    {
        public ITDSSerializer _serializer;
        public IPEndPoint _localEP;
        public IPEndPoint _remoteEP;
        public Socket _clientSocket;

        public IPEndPoint _remoteIpEndPoint;
        public IPEndPoint _localIpEndPoint;

        private readonly int _connectionTimeoutInMilliseconds = 10000;

        public TDSClient(IPAddress clientIpAddress, int port)
        {
            _serializer = SerializationFactory.GetSerializer(SerializerType.JSON);
            _localEP = new IPEndPoint(clientIpAddress, port);
        }

        // ManualResetEvent instances signal completion.  
        public ManualResetEvent connectDone = new ManualResetEvent(false);
        public ManualResetEvent sendDone = new ManualResetEvent(false);
        public ManualResetEvent receiveDone = new ManualResetEvent(false);

        public string serverResponse = string.Empty;

        public TDSResponse SendRequest(TDSRequest request)
        {
            try
            {
                IPAddress tdsServerIpAddress = IPAddress.Parse(ConfigurationManager.AppSettings["tdsServerIpAddress"]);
                int tdsServerPort = Convert.ToInt32(ConfigurationManager.AppSettings["tdsServerPort"]);
                _clientSocket = GetConfiguredClientSocket(tdsServerIpAddress);
                
                _remoteEP = new IPEndPoint(tdsServerIpAddress, tdsServerPort);
                _clientSocket.BeginConnect(_remoteEP, new AsyncCallback(ConnectCallback), _clientSocket); // Connect to the remote endpoint.  
                if (!connectDone.WaitOne(_connectionTimeoutInMilliseconds))
                {
                    Console.WriteLine("\nSocket connection failed: " + TDSStatusCode.CONNECTION_TIMEOUT_ERROR.ToString() + "\n");
                }

                request.SetProtocolIpEndPoints(_localIpEndPoint, _remoteIpEndPoint);
                string serializedRequest = _serializer.Serialize(request);
                serializedRequest += "<EOF>";

                SendSizeOfSerializedData(_clientSocket, serializedRequest);
                sendDone.WaitOne();

                Send(_clientSocket, serializedRequest); // Send serialized request data to the remote device.
                Console.WriteLine($"Successfully sent request to the TDS server!");
                sendDone.WaitOne();

                Receive(_clientSocket); // Receive the serializedResponse from the remote device.
                receiveDone.WaitOne();
                // Console.WriteLine($"Serialized Response from: {serverResponse}");

                ReleaseClientSocket(); // Release the socket.

                TDSResponse response = _serializer.DeSerializeAsResponse(serverResponse);
                return response;
            }
            catch
            {
                return null;
            }
        }

        private void SendSizeOfSerializedData(Socket client, string serializedData)
        {
            int noOfBytes = Encoding.ASCII.GetByteCount(serializedData);
            byte[] bytes = BitConverter.GetBytes(noOfBytes);
            Send(client, Encoding.UTF8.GetString(bytes));
            Console.WriteLine($"Sent {sizeof(int)} bytes to TDS server. Required buffer size = {noOfBytes} bytes.");
        }

        public Socket GetConfiguredClientSocket(IPAddress serverIpAddress)
        {
            Socket clientSocket = new Socket(serverIpAddress.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
            clientSocket.Bind(_localEP); // Bind with client's local IP Endpoint, otherwise, a random IP-Port for the client will be used.
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
                StateObject state = (StateObject)asyncResult.AsyncState; // Retrieve the state object and the client socket from the asynchronous state object.
                Socket client = state.workSocket;

                int bytesRead = client.EndReceive(asyncResult); // Read data from the remote device.
                
                if (bytesRead == sizeof(int))
                {
                    state.BufferSize = BitConverter.ToInt32(state.buffer, 0);
                    state.buffer = new byte[state.BufferSize];
                    Console.WriteLine($"Received {sizeof(int)} bytes from TDS Server. Initialized Buffer size for response data = {state.BufferSize} bytes.");
                    client.BeginReceive(state.buffer, 0, state.BufferSize, 0, new AsyncCallback(ReceiveCallback), state);
                }
                else if (bytesRead > 0)
                {
                    state.stringBuilder.Append(Encoding.ASCII.GetString(state.buffer, 0, bytesRead)); // There might be more data, so store the data received so far.
                    client.BeginReceive(state.buffer, 0, state.BufferSize, 0, new AsyncCallback(ReceiveCallback), state); // Get the rest of the data.
                }
                else
                {
                    if (state.stringBuilder.Length > 1) // All the data has arrived; put it in serverResponse.
                    {
                        serverResponse = state.stringBuilder.ToString();
                    }
                    receiveDone.Set(); // Signal that all bytes have been received.
                }
            }
            catch (Exception exception)
            {
                Console.WriteLine($"\n{(int)TDSStatusCode.UNAUTHORIZED}: {TDSStatusCode.UNAUTHORIZED} TDS client!\n");
                Console.WriteLine(exception.Message);
            }
        }

        public void Send(Socket client, string data)
        {
            try
            {
                byte[] byteData = Encoding.ASCII.GetBytes(data); // Convert the string data to byte data using ASCII encoding.
                client.BeginSend(byteData, 0, byteData.Length, 0, new AsyncCallback(SendCallback), client); // Begin sending the data to the remote device.
            }
            catch (Exception exception)
            {
                throw new Exception(exception.Message);
            }
        }

        public void SendCallback(IAsyncResult asyncResult)
        {
            try
            {
                Socket client = (Socket)asyncResult.AsyncState; // Retrieve the socket from the state object.
                int bytesSent = client.EndSend(asyncResult); // Complete sending the data to the remote device.
                sendDone.Set(); // Signal that all bytes have been sent.
            }
            catch (Exception exception)
            {
                throw new Exception(exception.Message);
            }
        }
    }
}
