using TDS_Node_Application.Core.CommProtocol.Enums;
using TDS_Node_Application.Entities;
using System;
using System.IO;

namespace TDS_Node_Application.TaskExecution
{
    public class PythonTaskExecutor : ITaskExecutor
    {
        private readonly string _pythonExecutablePath = "C:\\Program Files\\Python39\\python.exe";
        public TaskResult ExecuteTask(TaskData task)
        {
            ExecutionHandler handler = new ExecutionHandler();
            TaskResult result;
            string output = string.Empty;
            try
            {
                string filePath = handler.GenerateExecutableFile(task, ".py");
                CommandExecutor executor = new CommandExecutor();
                output = executor.RunTask(_pythonExecutablePath, filePath);
                result = handler.GetTaskExecutionResult(task, TaskState.SUCCESS, TaskStatus.COMPLETED, output, TDSStatusCode.SUCCESS.ToString(), ((int)TDSStatusCode.SUCCESS).ToString());
                
                File.Delete(filePath); // Delete executable file
            }
            catch (Exception exception)
            {
                result = handler.GetTaskExecutionResult(task, TaskState.FAILED, TaskStatus.ABORTED, output, TDSStatusCode.INTERNAL_SERVER_ERROR.ToString(), ((int)TDSStatusCode.INTERNAL_SERVER_ERROR).ToString());
                Console.WriteLine("Error in executing the file: " + exception.Message);
            }
            return result;
        }
    }
}
