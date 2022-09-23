using TDS_Node_Application.Entities;

namespace TDS_Node_Application.Core.CommProtocol.TDSSerialization
{
    public interface ITDSSerializer
    {
        //Serialize the TDSProtocol object to a string format
        string SerializeTask(TaskData task);

        string SerializeTaskResult(TaskResult result);

        TaskData DeSerializeAsTask(string data);

        TaskResult DeSerializeAsTaskResult(string data);
    }
}
