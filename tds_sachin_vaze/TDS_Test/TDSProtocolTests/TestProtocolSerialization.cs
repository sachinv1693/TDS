using Newtonsoft.Json.Linq;
using FluentAssertions.Json;
using NUnit.Framework;
using TDS_Coordinator_Application.Core.CommProtocol;
using TDS_Coordinator_Application.Core.CommProtocol.SerializationFactory;
using TDS_Coordinator_Application.Core.CommProtocol.TDSSerialization;
using TDS_Coordinator_Application.Core.CommProtocol.Enums;

namespace TDS_Test
{
    public class TestProtocolSerialization
    {
        private TDSRequest _request;
        private TDSResponse _response;
        private ITDSSerializer _serializer;

        [SetUp]
        public void Setup()
        {
            _request = new TDSRequest();
            _response = new TDSResponse();
            _serializer = SerializationFactory.GetSerializer(SerializerType.JSON);
            InitializeRequestData();
            InitializeResponseData();
        }

        private void InitializeRequestData()
        {
            _request.ProtocolType = "request";
            _request.SetHeader("TDSProtocolVersion", "1.0");
            _request.SetHeader("TDSProtocolFormat", "json");
            _request.SourceIp = "192.158.1.38";
            _request.DestIp = "191.152.1.39";
            _request.SourcePort = 8080;
            _request.DestPort = 8001;
            _request.SetMethod("node-add");
            _request.AddParameter("node-name", "Executor-Test-Serializer");
            _request.AddParameter("node-ip", "192.150.1.33");
            _request.AddParameter("node-port", "8003");
        }

        private void InitializeResponseData()
        {
            _response.ProtocolType = "response";
            _response.SetHeader("TDSProtocolVersion", "1.0");
            _response.SetHeader("TDSProtocolFormat", "json");
            _response.SourceIp = "191.152.1.39";
            _response.DestIp = "192.158.1.38";
            _response.SourcePort = 8001;
            _response.DestPort = 8080;
            _response.SetValue("status", TDSStatusCode.SUCCESS.ToString());
            _response.SetValue("node-id", "121");
            _response.SetValue("error-code", ((int)TDSStatusCode.SUCCESS).ToString());
            _response.SetValue("error-message", "Operation completed successfully");
        }

        [Test]
        public void Request_Object_Serialization_Succeeds()
        {
            string data = "{\"ProtocolType\":\"request\",\"SourceIp\":\"192.158.1.38\",\"DestIp\":\"191.152.1.39\",\"SourcePort\":8080,\"DestPort\":8001,\"Headers\":{\"TDSProtocolFormat\":\"json\",\"TDSProtocolVersion\":\"1.0\"},\"Body\":{\"node-name\":\"Executor-Test-Serializer\",\"method\":\"node-add\",\"node-ip\":\"192.150.1.33\",\"node-port\":\"8003\"}}";
            JToken expectedData = JToken.Parse(data); //Arrange

            string serializedData = _serializer.Serialize(_request);
            JToken actualData = JToken.Parse(serializedData); //Act

            actualData.Should().BeEquivalentTo(expectedData); //Assert
        }

        [Test]
        public void Request_Object_Deserialization_Succeeds()
        {
            //Arrange
            string data = "{\"ProtocolType\":\"request\",\"SourceIp\":\"192.158.1.38\",\"DestIp\":\"191.152.1.39\",\"SourcePort\":8080,\"DestPort\":8001,\"Headers\":{\"TDSProtocolFormat\":\"json\",\"TDSProtocolVersion\":\"1.0\"},\"Body\":{\"node-name\":\"Executor-Test-Serializer\",\"method\":\"node-add\",\"node-ip\":\"192.150.1.33\",\"node-port\":\"8003\"}}";

            TDSRequest deserializedData = _serializer.DeSerializeAsRequest(data); //Act

            //Assert
            string method = deserializedData.GetMethod();
            Assert.That(method, Is.EqualTo("node-add")); // Assert method value
            string protocolVersion = deserializedData.GetHeader("TDSProtocolVersion");
            Assert.That(protocolVersion, Is.EqualTo("1.0")); // Assert a header value
            Assert.That(deserializedData.SourcePort.ToString(), Is.EqualTo("8080")); // Assert direct value
            string nodeIp = deserializedData.GetParameter("node-ip");
            Assert.That(nodeIp, Is.EqualTo("192.150.1.33")); // Assert a payload parameter
        }

        [Test]
        public void Response_Object_Serialization_Succeeds()
        {
            string data = "{\"ProtocolType\":\"response\",\"SourceIp\":\"191.152.1.39\",\"DestIp\":\"192.158.1.38\",\"SourcePort\":8001,\"DestPort\":8080,\"Headers\":{\"TDSProtocolFormat\":\"json\",\"TDSProtocolVersion\":\"1.0\"},\"Body\":{\"node-id\":\"121\",\"status\":\"SUCCESS\",\"error-message\":\"Operation completed successfully\",\"error-code\":\"200\"}}";
            JToken expectedData = JToken.Parse(data); //Arrange
            
            string serializedData = _serializer.Serialize(_response);
            JToken actualData = JToken.Parse(serializedData); //Act

            actualData.Should().BeEquivalentTo(expectedData); //Assert
        }

        [Test]
        public void Response_Object_Deserialization_Succeeds()
        {
            //Arrange
            string data = "{\"ProtocolType\":\"response\",\"SourceIp\":\"191.152.1.39\",\"DestIp\":\"192.158.1.38\",\"SourcePort\":8001,\"DestPort\":8080,\"Headers\":{\"TDSProtocolFormat\":\"json\",\"TDSProtocolVersion\":\"1.0\"},\"Body\":{\"node-id\":\"121\",\"status\":\"SUCCESS\",\"error-message\":\"Operation completed successfully\",\"error-code\":\"200\"}}";

            //Act
            TDSResponse deserializedData = _serializer.DeSerializeAsResponse(data);

            //Assert
            string protocolFormat = deserializedData.GetHeader("TDSProtocolFormat");
            Assert.That(protocolFormat, Is.EqualTo("json")); // Assert a header value
            string errorCode = deserializedData.GetValue("error-code");
            Assert.That(errorCode, Is.EqualTo("200")); // Assert a payload value 
            Assert.That(deserializedData.SourcePort.ToString(), Is.EqualTo("8001")); // Assert direct value
        }
    }
}