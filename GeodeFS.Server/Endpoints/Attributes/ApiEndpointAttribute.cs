namespace GeodeFS.Server.Endpoints.Attributes;

public class ApiEndpointAttribute : HttpEndpointAttribute
{
    public ApiEndpointAttribute(string route, HttpMethods method = HttpMethods.Get, string contentType = Bunkum.Listener.Protocol.ContentType.Json) : base(BaseRoute + route, method, contentType)
    {}

    public ApiEndpointAttribute(string route, string contentType) : base(BaseRoute + route, contentType)
    {}

    public ApiEndpointAttribute(string route, string contentType, HttpMethods method) : base(BaseRoute + route, contentType, method)
    {}

    public const string BaseRoute = "/api/";
}