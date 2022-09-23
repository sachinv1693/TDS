using Newtonsoft.Json;
using TDS_Client_Application.Entities;

namespace TDS_Client_Application.Core.CommProtocol.TDSSerialization
{
    public class TDSJSonSerializer : ITDSSerializer
    {
        public string Serialize(object protocol)
        {
            string jsonData = JsonConvert.SerializeObject(protocol);
            return jsonData;
        }

        public TDSRequest DeSerializeAsRequest(string data)
        {
            var deserializedJson = JsonConvert.DeserializeObject<TDSRequest>(data);
            return deserializedJson;
        }

        public TDSResponse DeSerializeAsResponse(string data)
        {
            var deserializedJson = JsonConvert.DeserializeObject<TDSResponse>(data);
            return deserializedJson;
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
