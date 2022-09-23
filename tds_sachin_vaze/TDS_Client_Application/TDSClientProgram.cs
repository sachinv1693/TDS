using System;
using System.IO;
using System.Net;
using TDS_Client_Application.ClientCommunicationLayer;
using TDS_Client_Application.Core.CommProtocol;

namespace TDS_Client_Application
{
    class TDSClientProgram
    {
        public static void Main(string[] args) // Use string[] args later
        {
            TDSRequest request = null;
            if (IsTDSHelpCommand(args))
            {
                PrintTDSCommands();
                return;
            }
            if (IsValidCommand(args))
            {
                request = FormTDSRequest(args);
            }
            if (request == null)
            {
                Console.WriteLine("\nInvalid arguments passed. Please type \"tds --help\" to know the usecases.\n");
                return;
            }
            //request.SetMethod("node-add");
            //request.AddParameter("node-name", "Executor-Test");
            //request.AddParameter("node-ip", "::1");
            //request.AddParameter("node-port", "12444");

            //request.SetMethod("client-add");
            //request.AddParameter("client-name", "Test-Client");
            //request.AddParameter("client-ip", "::1");
            //request.AddParameter("client-port", "1234");
            
            TDSResponse response = SendTDSRequest(request);
            if (response != null)
            {
                PrintRequestSpecificResponse(request, response);
            }
        }

        private static bool IsTDSHelpCommand(string[] args)
        {
            return args[0] == "tds" && args[1] == "--help";
        }

        private static bool IsValidCommand(string[] args)
        {
            return args.Length == 3 && args[0] == "taskmgr";
        }

        private static void PrintRequestSpecificResponse(TDSRequest request, TDSResponse response)
        {
            string requestType = request.GetMethod();
            switch (requestType)
            {
                case "task-add": Console.WriteLine($"\n{response.GetValue("task-status")}, Task GUID: {response.GetValue("task-guid")}\n");
                    break;
                case "task-status": Console.WriteLine($"\nStatus: {response.GetValue("task-status")}\n");
                    break;
                case "task-result": Console.WriteLine(response.GetValue("task-result"));
                    break;
                default: Console.WriteLine("\nInvalid request\n");
                    break;
            }
        }

        private static TDSRequest FormTDSRequest(string[] args)
        {
            return args[1] switch
            {
                "queue" => CreateTaskAddRequest(args[2]),
                "query" => CreateTaskStatusRequest(args[2]),
                "result" => CreateTaskResultRequest(args[2]),
                _ => null
            };
        }

        private static TDSRequest CreateTaskResultRequest(string taskGuid)
        {
            TDSRequest request = new TDSRequest();
            request.SetMethod("task-result");
            request.AddParameter("task-guid", taskGuid);
            return request;
        }

        private static TDSRequest CreateTaskStatusRequest(string taskGuid)
        {
            TDSRequest request = new TDSRequest();
            request.SetMethod("task-status");
            request.AddParameter("task-guid", taskGuid);
            return request;
        }

        private static TDSRequest CreateTaskAddRequest(string taskExecutablePath)
        {
            TDSRequest request = new TDSRequest();
            string taskExecutable = GetBase64EncodedTask(taskExecutablePath);
            if (string.IsNullOrEmpty(taskExecutable))
            {
                return null;
            }
            request.SetMethod("task-add");
            request.AddParameter("task-exe-path", taskExecutablePath);
            request.AddParameter("task-executable", taskExecutable); // Base64 converted task executable
            return request;
        }

        private static string GetBase64EncodedTask(string taskExecutablePath)
        {
            try
            {
                byte[] bytes = File.ReadAllBytes(taskExecutablePath);
                string taskExecutable = Convert.ToBase64String(bytes);
                return taskExecutable;
            }
            catch (Exception exception)
            {
                Console.WriteLine("Error while reading executable file path: " + exception.Message);
                return null;
            }
        }

        private static void PrintTDSCommands()
        {
            Console.WriteLine("\n# # # # # # # # TDS commands # # # # # # # #");
            Console.WriteLine("-----------------------------------------------------------------------");
            Console.WriteLine("1. To queue a task: taskmgr queue <task executable path>");
            Console.WriteLine("It will return task ID whcih will be queued to run on a node machine");
            Console.WriteLine("-----------------------------------------------------------------------");
            Console.WriteLine("2. To get task status: taskmgr query <task GUID>");
            Console.WriteLine("It will return task status: pending, queued, executing or completed");
            Console.WriteLine("-----------------------------------------------------------------------");
            Console.WriteLine("3. To get result of task: taskmgr result <task GUID>");
            Console.WriteLine("It will return task execution result");
            Console.WriteLine("-----------------------------------------------------------------------\n");
        }

        private static TDSResponse SendTDSRequest(TDSRequest request)
        {
            try
            {
                TDSClient client = new TDSClient(IPAddress.Parse("::1"), 1445);
                TDSResponse response = client.SendRequest(request);
                return response;
            }
            catch
            {
                return null;
            }
        }
    }
}
