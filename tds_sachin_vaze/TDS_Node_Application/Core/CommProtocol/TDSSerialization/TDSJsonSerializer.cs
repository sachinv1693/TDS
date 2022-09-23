using Newtonsoft.Json;
using TDS_Node_Application.Entities;

namespace TDS_Node_Application.Core.CommProtocol.TDSSerialization
{
    public class TDSJSonSerializer : ITDSSerializer
    {
        public string SerializeTask(TaskData task)
        {
            string jsonData = JsonConvert.SerializeObject(task);
            return jsonData;
        }

        public string SerializeTaskResult(TaskResult taskResult)
        {
            string jsonData = JsonConvert.SerializeObject(taskResult);
            return jsonData;
        }

        public TaskData DeSerializeAsTask(string data)
        {
            var deserializedJson = JsonConvert.DeserializeObject<TaskData>(data);
            return deserializedJson;
        }

        public TaskResult DeSerializeAsTaskResult(string data)
        {
            var deserializedJson = JsonConvert.DeserializeObject<TaskResult>(data);
            return deserializedJson;
        }
    }
}
