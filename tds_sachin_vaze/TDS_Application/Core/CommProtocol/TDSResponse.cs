using System;

namespace TDS_Coordinator_Application.Core.CommProtocol
{
    public class TDSResponse : TDSProtocol
    {
        public TDSResponse()
        {
            ProtocolType = "response";
        }

        //Should return true/false, corresponds to status tag in the response
        public bool GetStatus()
        {
            Body.TryGetValue("status", out string status);
            return (status == "SUCCESS");
        }

		public int GetErrorCode()
        {
            Body.TryGetValue("error-code", out string errorCode);
            return Convert.ToInt32(errorCode);
        }

		public string GetErrorMessage()
        {
            Body.TryGetValue("error-message", out string errorMessage);
            return errorMessage;
        }

		//Should return the value for a specific key from the response data, for example to retrieve the node-id
		public string GetValue(string key)
        {
            Body.TryGetValue(key, out string value);
            return value;
        }

		public void SetValue(string key, string value)
        {
            Body.TryAdd(key, value);
        }

        public void SetStatus(string value)
        {
            Body.TryAdd("status", value);
        }

        public void SetErrorCode(string value)
        {
            Body.TryAdd("error-code", value);
        }

        public void SetErrorMessage(string value)
        {
            Body.TryAdd("error-message", value);
        }
    }
}
