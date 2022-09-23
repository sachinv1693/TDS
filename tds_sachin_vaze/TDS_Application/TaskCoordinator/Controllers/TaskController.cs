using System;
using System.Data.SqlClient;
using TDS_Coordinator_Application.CommunicationLayer;
using TDS_Coordinator_Application.Core.CommProtocol;
using TDS_Coordinator_Application.Core.CommProtocol.Enums;
using TDS_Coordinator_Application.Core.TaskScheduler;
using TDS_Coordinator_Application.TaskCoordinator.DB;
using TDS_Coordinator_Application.TaskCoordinator.DB.Entities;
using TDS_Coordinator_Application.TaskCoordinator.DB.Enums;
using TDS_Coordinator_Application.TaskCoordinator.DB.RepoFactory;

namespace TDS_Coordinator_Application.TaskCoordinator.Controllers
{
    public class TaskController : ITDSController
    {
        private readonly IDatabaseManager<SqlDataReader> _databaseManager;
        private readonly dynamic _dao;

        private readonly string _successMessage = "Request was successful!";
        private readonly string _failureGuidMessage = "Failed to generate GUID for the task!";
        private readonly string _failureMessage = "Could not find given task GUID";
        private readonly string _noMethodMessage = "No such method found";

        public TaskController()
        {
            _databaseManager = new SqlDatabaseManager();
            _dao = RepoFactory.GetRepository(ReopsitoryType.TASK, _databaseManager);
        }

        public TDSResponse ProcessRequest(TDSRequest request, StateObject state)
        {
            TDSResponse response = new TDSResponse();
            response.SetProtocolIpEndPoints(state.localIpEndPoint, state.remoteIpEndPoint);
            string requestType = request.GetMethod();
            if (requestType == "task-add")
            {
                ProcessTask(request, state, response);
            }
            else if (requestType == "task-status" || requestType == "task-result")
            {
                string taskGuid = request.GetParameter("task-guid");
                FetchTaskFeatureFromDB(taskGuid, response, requestType);
            }
            else if (request.GetMethod() == "task-delete")
            {
                string guid = request.GetParameter("task-guid");
                _dao.DeleteByGuid(guid);
            }
            else
            {
                SetResponseParameters(response, TDSStatusCode.METHOD_NOT_FOUND, _noMethodMessage, string.Empty);
            }
            return response;
        }

        private void FetchTaskFeatureFromDB(string taskGuid, TDSResponse response, string requestType)
        {
            try
            {
                TaskData task = _dao.GetTaskByGuid(taskGuid);
                if (task == null)
                {
                    SetResponseParameters(response, TDSStatusCode.BAD_REQUEST, _failureMessage, taskGuid);
                    return;
                }
                SetResponseParameters(response, TDSStatusCode.SUCCESS, _successMessage, taskGuid);
                if (requestType == "task-status")
                {
                    response.SetValue(requestType, ((TaskStatus)task.TaskStatus).ToString());
                }
                else // requestType = task-result
                {
                    response.SetValue(requestType, task.TaskResult);
                }
            }
            catch (Exception exception)
            {
                SetResponseParameters(response, TDSStatusCode.INTERNAL_SERVER_ERROR, exception.Message, taskGuid);
                response.SetValue(requestType, $"Failed to fetch {requestType}!");
            }
        }

        private void ProcessTask(TDSRequest request, StateObject state, TDSResponse response)
        {
            TaskData task = new TaskData()
            {
                ClientId = state.ClientID,
                TaskPath = request.GetParameter("task-exe-path"),
                TaskExecutable = request.GetParameter("task-executable"),
                TaskGuid = Guid.NewGuid().ToString()
            };
            AddTaskToDB(task, response);
            var taskData = _dao.GetTaskByGuid(task.TaskGuid);
            task.Id = taskData.Id;
            ScheduleTask(task);
            _dao.SetTaskStatus(task.TaskGuid, TaskStatus.QUEUED); // Update Task Status in DB
            response.SetValue("task-status", TaskStatus.QUEUED.ToString());
        }

        private void ScheduleTask(TaskData task)
        {
            try
            {
                TaskScheduler scheduler = new TaskScheduler();
                scheduler.StartScheduler(task);
                Console.WriteLine($"Task with GUID {task.TaskGuid} has been scheduled to run on node machine!");
            }
            catch (Exception exception)
            {
                Console.WriteLine("An error occurred while scheduling the task: " + exception.Message);
            }
        }

        private void AddTaskToDB(TaskData task, TDSResponse response)
        {
            try
            {
                _dao.Add(task);
                Console.WriteLine($"Task has been added to database. GUID: {task.TaskGuid}");
                SetResponseParameters(response, TDSStatusCode.SUCCESS, _successMessage, task.TaskGuid);
            }
            catch (SqlException sqlException)
            {
                SetResponseParameters(response, TDSStatusCode.BAD_REQUEST, sqlException.Message, _failureGuidMessage);
            }
            catch (Exception exception)
            {
                SetResponseParameters(response, TDSStatusCode.INTERNAL_SERVER_ERROR, exception.Message, _failureGuidMessage);
            }
        }

        private void SetResponseParameters(TDSResponse response, TDSStatusCode tdsStatusCode, string errorMessage, string taskGuid)
        {
            response.SetStatus(tdsStatusCode.ToString());
            response.SetErrorCode(((int)tdsStatusCode).ToString());
            response.SetErrorMessage(errorMessage);
            response.SetValue("task-guid", taskGuid);
        }
    }
}
