using TDS_Node_Application.Core.CommProtocol.Enums;
using TDS_Node_Application.Entities;
using System.IO;
using System;

namespace TDS_Node_Application.TaskExecution
{
    public class ExecutionHandler
    {
        public TaskResult GetTaskExecutionResult(TaskData task, TaskState state, TaskStatus status, string output, string errorMessage, string errorCode)
        {
            return new TaskResult()
            {
                TaskGuid = task.TaskGuid,
                TaskState = state,
                TaskStatus = status,
                TaskOutcome = output,
                ErrorMessage = errorMessage,
                ErrorCode = errorCode
            };
        }

        public string GenerateExecutableFile(TaskData task, string fileType)
        {
            try
            {
                string currentFileName = "task_" + task.TaskGuid + fileType;
                string taskExecutablesDirPath = Directory.GetCurrentDirectory() + @"\TaskExecutables\";
                Directory.CreateDirectory(taskExecutablesDirPath);
                string currentFilePath = taskExecutablesDirPath + currentFileName;
                File.Create(currentFilePath).Close();
                File.WriteAllBytes(currentFilePath, Convert.FromBase64String(task.TaskExecutable));
                return currentFilePath;
            }
            catch (Exception exception)
            {
                throw new Exception("Error in generating executable file: " + exception.Message);
            }
        }
    }
}