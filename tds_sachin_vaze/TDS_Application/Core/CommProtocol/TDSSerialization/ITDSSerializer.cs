using TDS_Coordinator_Application.TaskCoordinator.DB.Entities;

namespace TDS_Coordinator_Application.Core.CommProtocol.TDSSerialization
{
    public interface ITDSSerializer
    {
        //Serialize the TDSProtocol object to a string format
        string Serialize(object protocol);
        
        TDSRequest DeSerializeAsRequest(string data);

        TDSResponse DeSerializeAsResponse(string data);

        TaskData DeSerializeAsTask(string data);

        TaskResult DeSerializeAsTaskResult(string data);
    }
}
