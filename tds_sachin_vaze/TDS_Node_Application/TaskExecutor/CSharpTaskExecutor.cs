using TDS_Node_Application.Core.CommProtocol.Enums;
using TDS_Node_Application.Entities;
using System;
using System.IO;

namespace TDS_Node_Application.TaskExecution
{
    public class CSharpTaskExecutor : ITaskExecutor
    {
        private readonly string _cSharpExecutablePath = "C:\\Users\\sachin.vaze\\source\\repos\\MyLoginProject\\packages\\Microsoft.CodeDom.Providers.DotNetCompilerPlatform.2.0.1\\tools\\RoslynLatest\\csc.exe";

        public TaskResult ExecuteTask(TaskData task)
        {
            ExecutionHandler handler = new ExecutionHandler();
            TaskResult result;
            string output = string.Empty;
            try
            {
                string filePath = handler.GenerateExecutableFile(task, ".cs");
                CommandExecutor executor = new CommandExecutor();
                output = executor.RunTask(_cSharpExecutablePath, filePath); // Compile C# file
                string compiledExecutable = Path.GetFileNameWithoutExtension(filePath) + ".exe"; // Extract file name
                if (File.Exists(compiledExecutable))
                {
                    output = executor.RunTask(compiledExecutable, string.Empty);
                    File.Delete(compiledExecutable); // Delete the .exe file
                }
                else
                {
                    output = "Compilation Error: " + output;
                    throw new InvalidOperationException(output);
                }
                File.Delete(filePath); // Delete the .cs file
                result = handler.GetTaskExecutionResult(task, TaskState.SUCCESS, TaskStatus.COMPLETED, output, TDSStatusCode.SUCCESS.ToString(), ((int)TDSStatusCode.SUCCESS).ToString());
            }
            catch (InvalidOperationException exception)
            {
                result = handler.GetTaskExecutionResult(task, TaskState.FAILED, TaskStatus.ABORTED, output, TDSStatusCode.BAD_REQUEST.ToString(), ((int)TDSStatusCode.BAD_REQUEST).ToString());
                Console.WriteLine("Error in executing the file: " + exception.Message);
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
