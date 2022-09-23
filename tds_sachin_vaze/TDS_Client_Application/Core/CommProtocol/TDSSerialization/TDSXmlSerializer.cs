using System;
using System.IO;
using System.Text;
using System.Xml;
using System.Xml.Serialization;
using TDS_Client_Application.Entities;

namespace TDS_Client_Application.Core.CommProtocol.TDSSerialization
{
    public class TDSXmlSerializer : ITDSSerializer
    {
        private readonly StringBuilder _sbData;
        private StringWriter _swWriter;
        private readonly XmlDocument _xDoc;

        public TDSXmlSerializer()
        {
            _sbData = new StringBuilder();
            _xDoc = new XmlDocument();
        }

        public string Serialize(object protocol)
        {
            _swWriter = new StringWriter(_sbData);
            new XmlSerializer(typeof(TDSProtocol)).Serialize(_swWriter, protocol);
            return _sbData.ToString();
        }

        public string SerializeTask(TaskData task)
        {
            _swWriter = new StringWriter(_sbData);
            new XmlSerializer(typeof(TaskData)).Serialize(_swWriter, task);
            return _sbData.ToString();
        }

        public string SerializeTaskResult(TaskResult taskResult)
        {
            _swWriter = new StringWriter(_sbData);
            new XmlSerializer(typeof(TaskResult)).Serialize(_swWriter, taskResult);
            return _sbData.ToString();
        }

        public TDSRequest DeSerializeAsRequest(string data)
        {
            var deserializedXml = GetDeserializedXml(data, typeof(TDSRequest));
            return (TDSRequest)deserializedXml;
        }

        public TDSResponse DeSerializeAsResponse(string data)
        {
            var deserializedXml = GetDeserializedXml(data, typeof(TDSResponse));
            return (TDSResponse)deserializedXml;
        }

        public TaskData DeSerializeAsTask(string data)
        {
            var deserializedXml = GetDeserializedXml(data, typeof(TaskData));
            return (TaskData)deserializedXml;
        }

        public TaskResult DeSerializeAsTaskResult(string data)
        {
            var deserializedXml = GetDeserializedXml(data, typeof(TaskResult));
            return (TaskResult)deserializedXml;
        }

        private object GetDeserializedXml(string data, Type type)
        {
            _xDoc.LoadXml(data);
            XmlNodeReader xNodeReader = new XmlNodeReader(_xDoc.DocumentElement);
            var deserializedXml = new XmlSerializer(type).Deserialize(xNodeReader);
            return deserializedXml;
        }
    }
}
