namespace TDS_Coordinator_Application.Core.CommProtocol
{
    public class TDSRequest : TDSProtocol
    {
        public TDSRequest()
        {
            ProtocolType = "request";
        }

        //corresponds to method in the payload
        public string GetMethod()
        {
            Body.TryGetValue("method", out string method);
            return method;
        }

		public void SetMethod(string method)
        {
            Body.TryAdd("method", method);
        }

		//Gets the value of a given parameter with specified key
		public string GetParameter(string key)
        {
            Body.TryGetValue(key, out string paramValue);
            return paramValue;
        }

		//Adds a parameter for the request
		public void AddParameter(string key, string value)
        {
            Body.TryAdd(key, value);
        }
	}
}
