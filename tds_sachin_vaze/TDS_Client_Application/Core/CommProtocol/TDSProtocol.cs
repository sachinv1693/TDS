using System.Collections.Concurrent;
using System.Net;

namespace TDS_Client_Application.Core.CommProtocol
{
    public class TDSProtocol
    {
		public TDSProtocol()
		{
			Headers = new ConcurrentDictionary<string, string>();
			Body = new ConcurrentDictionary<string, string>();
			SetHeader("TDSProtocolVersion", "1.0");
			SetHeader("TDSProtocolFormat", "json");
		}

		//Corresponds to "type" attribute in protocol definition, defines the type of request, could be a request
		// or a response type.
		public string ProtocolType { get; set; }

		public string SourceIp { get; set; }
		public string DestIp { get; set; }
		public int SourcePort { get; set; }
		public int DestPort { get; set; }

		public ConcurrentDictionary<string, string> Headers { get; set; }
		public ConcurrentDictionary<string, string> Body { get; set; }

		public string GetHeader(string headerKey)
		{
			Headers.TryGetValue(headerKey, out string headerValue);
			return headerValue;
		}

		public void SetHeader(string headerKey, string headerValue)
		{
			Headers.TryAdd(headerKey, headerValue);
		}

		public void SetProtocolIpEndPoints(IPEndPoint LocalIpEndPoint, IPEndPoint RemoteIpEndPoint)
		{
			SourceIp = LocalIpEndPoint.Address.ToString();
			SourcePort = LocalIpEndPoint.Port;
			DestIp = RemoteIpEndPoint.Address.ToString();
			DestPort = RemoteIpEndPoint.Port;
		}
	}
}
