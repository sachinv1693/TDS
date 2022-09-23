using System;
using System.Diagnostics;

namespace TDS_Node_Application.TaskExecution
{
    public class CommandExecutor
    {
        private readonly int _processTimeoutInMilliSeconds = 15000; // 15 sec timeout for process execution
        
        public string RunTask(string executablePath, string filePath)
        {
            string output;
            try
            {
                var process = new Process
                {
                    StartInfo = new ProcessStartInfo
                    {
                        FileName = executablePath,
                        Arguments = $"{filePath}",
                        UseShellExecute = false,
                        RedirectStandardOutput = true,
                        CreateNoWindow = true
                    }
                };
                output = ExecuteProcess(process);
            }
            catch (Exception exception)
            {
                throw new Exception("Error in running the task: " + exception.Message);
            }
            return output;
        }

        private string ExecuteProcess(Process process)
        {
            string output;
            process.Start();
            if (process.WaitForExit(_processTimeoutInMilliSeconds))
            {
                output = process.StandardOutput.ReadToEnd();
            }
            else
            {
                Console.WriteLine("Process timeout error!");
                process.Kill();
                output = "Process timeout: Could be running into an infinite loop!";
            }
            Console.WriteLine(output);
            return output;
        }
    }
}
