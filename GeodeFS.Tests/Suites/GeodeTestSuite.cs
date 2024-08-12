using GeodeFS.Tests.Logging;
using NotEnoughLogs;
using NotEnoughLogs.Behaviour;

namespace GeodeFS.Tests.Suites;

public abstract class GeodeTestSuite
{
    private readonly Logger _logger = new([new NUnitSink()], new LoggerConfiguration
    {
        Behaviour = new DirectLoggingBehaviour(),
        MaxLevel = LogLevel.Trace,
    });

    protected Logger Proxy(string name) => new ProxyLogger(this._logger, name);
}