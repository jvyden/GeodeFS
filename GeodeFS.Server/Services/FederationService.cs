using System.Diagnostics.CodeAnalysis;
using Bunkum.Core.Database;
using Bunkum.Listener.Request;
using GeodeFS.Common.Federation;

namespace GeodeFS.Server.Services;

public class FederationService : Service
{
    private readonly FederationController _controller;

    internal FederationService(Logger logger, FederationController controller) : base(logger)
    {
        this._controller = controller;
    }

    [SuppressMessage("ReSharper", "ConvertIfStatementToReturnStatement")]
    public override object? AddParameterToEndpoint(ListenerContext context, BunkumParameterInfo parameter, Lazy<IDatabaseContext> database)
    {
        if (ParameterEqualTo<FederationController>(parameter))
            return _controller;
        
        if (ParameterEqualTo<GeodeLocalNode>(parameter))
            return _controller.LocalNode;

        return null;
    }
}