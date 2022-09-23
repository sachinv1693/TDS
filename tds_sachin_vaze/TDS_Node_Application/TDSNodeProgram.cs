using TDS_Node_Application.NodeCommunicationLayer;
using System;
using System.Net;

namespace TDS_Node_Application
{
    class TDSNodeProgram
    {
        static void Main(string[] args)
        {
            if (args.Length == 2 && args[0] == "-port")
            {
                if (int.TryParse(args[1], out int port))
                {
                    StartNodeServer(port);
                }
                else
                {
                    Console.WriteLine("Please enter an integer value for port no.");
                }
            }
            else
            {
                Console.WriteLine("Invalid command line arguments! Use -port <portNumber>");
            }
        }

        private static void StartNodeServer(int nodePort)
        {
            try
            {
                IPAddress nodeIpAddress = IPAddress.Parse("::1");
                NodeServer nodeServer = new NodeServer(nodeIpAddress, nodePort);
                nodeServer.StartListening();
            }
            catch (Exception exception)
            {
                Console.WriteLine("\nError in starting the node server: " + exception.Message);
                Console.WriteLine("Exiting...");
            }
        }
    }
}
