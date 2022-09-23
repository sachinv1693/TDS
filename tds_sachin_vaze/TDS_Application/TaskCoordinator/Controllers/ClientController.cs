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
    public class ClientController : ITDSController
    {
        private readonly dynamic _dao;
        private readonly string _successMessage = "Request was successful!";
        private readonly string _failureGuidMessage = "Failed to generate GUID for the client!";
        private readonly string _noMethodMessage = "No such method found";

        public ClientController()
        {
            IDatabaseManager<SqlDataReader> databaseManager = new SqlDatabaseManager();
            _dao = RepoFactory.GetRepository(ReopsitoryType.CLIENT, databaseManager);
        }

        public TDSResponse ProcessRequest(TDSRequest request, StateObject state)
        {
            TDSResponse response = new TDSResponse();
            response.SetProtocolIpEndPoints(state.localIpEndPoint, state.remoteIpEndPoint);
            if (request.GetMethod() == "client-add")
            {
               RegisterClient(request, response);
            }
            else if (request.GetMethod() == "client-delete")
            {
                string guid = request.GetParameter("client-guid");
                _dao.DeleteByGuid(guid);
            }
            else
            {
                SetResponseParameters(response, TDSStatusCode.METHOD_NOT_FOUND, _noMethodMessage, string.Empty);
            }
            return response;
        }

        private void SetResponseParameters(TDSResponse response, TDSStatusCode tdsStatusCode, string errorMessage, string clientGuid)
        {
            response.SetStatus(tdsStatusCode.ToString());
            response.SetErrorCode(((int)tdsStatusCode).ToString());
            response.SetErrorMessage(errorMessage);
            response.SetValue("client-guid", clientGuid);
        }

        private void RegisterClient(TDSRequest request, TDSResponse response)
        {
            try
            {
                ClientData clientData = new ClientData()
                {
                    HostName = request.GetParameter("client-name"),
                    ClientIpAddress = request.GetParameter("client-ip"),
                    ClientPort = request.GetParameter("client-port"),
                    ClientGuid = Guid.NewGuid().ToString()
                };
                _dao.Add(clientData);
                SetResponseParameters(response, TDSStatusCode.SUCCESS, _successMessage, clientData.ClientGuid);
            }
            catch (SqlException sqlEx)
            {
                SetResponseParameters(response, TDSStatusCode.BAD_REQUEST, sqlEx.Message, _failureGuidMessage);
            }
            catch (Exception exception)
            {
                SetResponseParameters(response, TDSStatusCode.INTERNAL_SERVER_ERROR, exception.Message, _failureGuidMessage);
            }
        }
    }
}
