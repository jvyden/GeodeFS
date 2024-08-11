

using GeodeFS.Tests.Federation.Logging;
using NotEnoughLogs;
using NotEnoughLogs.Behaviour;

namespace GeodeFS.Tests.Federation.Suites;

public abstract class GeodeTestSuite
{
    private readonly Logger _logger = new([new NUnitSink()], new LoggerConfiguration
    {
        Behaviour = new DirectLoggingBehaviour(),
        MaxLevel = LogLevel.Trace,
    });

    protected Logger Proxy(string name) => new ProxyLogger(this._logger, name);
}