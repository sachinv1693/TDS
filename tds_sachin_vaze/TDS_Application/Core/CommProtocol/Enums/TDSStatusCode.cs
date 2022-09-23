namespace TDS_Coordinator_Application.Core.CommProtocol.Enums
{
    public enum TDSStatusCode
    {
        SUCCESS = 200,
        BAD_REQUEST = 400,
        UNAUTHORIZED = 401,
        INTERNAL_SERVER_ERROR = 500,
        SERVICE_UNAVAILABLE = 503,
        CONNECTION_TIMEOUT_ERROR = 599,
        METHOD_NOT_FOUND = 404
    }
}
