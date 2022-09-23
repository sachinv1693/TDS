using System;
using TDS_Coordinator_Application.Core.CommProtocol.TDSSerialization;
using TDS_Coordinator_Application.Core.CommProtocol.Enums;

namespace TDS_Coordinator_Application.Core.CommProtocol.SerializationFactory
{
    public class SerializationFactory
    {
        public static ITDSSerializer GetSerializer(SerializerType type)
        {
            return type switch
            {
                SerializerType.JSON => new TDSJSonSerializer(),
                SerializerType.XML => new TDSXmlSerializer(),
                _ => throw new InvalidOperationException("Invalid serializer type specified.")
            };
        }
    }
}
