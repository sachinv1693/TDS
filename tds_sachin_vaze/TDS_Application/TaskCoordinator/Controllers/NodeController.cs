using System;
using System.Data.SqlClient;
using TDS_Coordinator_Application.CommunicationLayer;
using TDS_Coordinator_Application.Core.CommProtocol;
using TDS_Coordinator_Application.Core.CommProtocol.Enums;
using TDS_Coordinator_Application.TaskCoordinator.DB;
using TDS_Coordinator_Application.TaskCoordinator.DB.Entities;
using TDS_Coordinator_Application.TaskCoordinator.DB.Enums;
using TDS_Coordinator_Application.TaskCoordinator.DB.RepoFactory;

namespace TDS_Coordinator_Application.TaskCoordinator.Controllers
{
    public class NodeController : ITDSController
    {
        private readonly dynamic _dao;

        private readonly string _successMessage = "Request was successful!";
        private readonly string _failureGuidMessage = "Failed to generate GUID for the node!";
        private readonly string _noMethodMessage = "No such method found!";

        public NodeController()
        {
            IDatabaseManager<SqlDataReader> databaseManager = new SqlDatabaseManager();
            _dao = RepoFactory.GetRepository(ReopsitoryType.NODE, databaseManager);
        }

        public TDSResponse ProcessRequest(TDSRequest request, StateObject state)
        {
            TDSResponse response = new TDSResponse();
            response.SetProtocolIpEndPoints(state.localIpEndPoint, state.remoteIpEndPoint);
            if (request.GetMethod() == "node-add")
            {
                RegisterNode(request, response);
            }
            else if (request.GetMethod() == "node-delete")
            {
                string guid = request.GetParameter("node-guid");
                _dao.DeleteByGuid(guid);
            }
            else
            {
                SetResponseParameters(response, TDSStatusCode.METHOD_NOT_FOUND, _noMethodMessage, string.Empty);
            }
            return response;
        }

        private void SetResponseParameters(TDSResponse response, TDSStatusCode tdsStatusCode, string errorMessage, string nodeGuid)
        {
            response.SetStatus(tdsStatusCode.ToString());
            response.SetErrorCode(((int)tdsStatusCode).ToString());
            response.SetErrorMessage(errorMessage);
            response.SetValue("node-guid", nodeGuid);
        }

        private void RegisterNode(TDSRequest request, TDSResponse response)
        {
            try
            {
                NodeData nodeData = new NodeData()
                {
                    NodeIpAddress = request.GetParameter("node-ip"),
                    NodePort = request.GetParameter("node-port"),
                    HostName = request.GetParameter("node-name"),
                    NodeExecutorType = Convert.ToInt32(request.GetParameter("node-type")),
                    NodeGuid = Guid.NewGuid().ToString()
                };
                _dao.Add(nodeData);
                SetResponseParameters(response, TDSStatusCode.SUCCESS, _successMessage, nodeData.NodeGuid);
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
    }
}
