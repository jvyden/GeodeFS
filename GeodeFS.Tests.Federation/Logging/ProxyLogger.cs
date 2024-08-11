using NotEnoughLogs;
using NotEnoughLogs.Behaviour;
using NotEnoughLogs.Sinks;

namespace GeodeFS.Tests.Federation.Logging;

public class ProxyLogger : IDisposable
{
    private readonly Logger _logger;

    public ProxyLogger(Logger otherLogger, string path)
    {
        ProxySink proxy = new(otherLogger, path);
        this._logger = new Logger([proxy], new LoggerConfiguration
        {
            Behaviour = new DirectLoggingBehaviour(),
            MaxLevel = LogLevel.Trace,
        });
    }

    public static implicit operator Logger(ProxyLogger l) => l._logger; 

    public void Dispose()
    {
        this._logger.Dispose();
        GC.SuppressFinalize(this);
    }
    
    private class ProxySink : ILoggerSink
    {
        private readonly Logger _logger;
        private readonly string _path;

        public ProxySink(Logger logger, string path)
        {
            this._logger = logger;
            this._path = path + '/';
        }

        public void Log(LogLevel level, ReadOnlySpan<char> category, ReadOnlySpan<char> content)
        {
            Span<char> newCategory = stackalloc char[category.Length + this._path.Length];
            this._path.CopyTo(newCategory);
            category.CopyTo(newCategory.Slice(this._path.Length));
            
            this._logger.Log(level, newCategory, content);
        }

        public void Log(LogLevel level, ReadOnlySpan<char> category, ReadOnlySpan<char> format, params object[] args)
        {
            Span<char> newCategory = stackalloc char[category.Length + this._path.Length];
            this._path.CopyTo(newCategory);
            category.CopyTo(newCategory.Slice(this._path.Length));
            
            this._logger.Log(level, newCategory, format, args);
        }
    }
}