using System.Reflection;
using GeodeFS.Server.Configuration;
using GeodeFS.Server.Services;
using NotEnoughLogs.Behaviour;

BunkumConsole.AllocateConsole();

BunkumServer server = new BunkumHttpServer(new LoggerConfiguration
{
    Behaviour = new QueueLoggingBehaviour(),
    MaxLevel = LogLevel.Trace
});

server.Initialize = s =>
{
    FileSystemDataStore dataStore = new();
    
    s.AddConfigFromJsonFile<GeodeConfig>("geode.json");

    s.DiscoverEndpointsFromAssembly(Assembly.GetExecutingAssembly());
    s.AddStorageService(dataStore);
    s.AddService<DataAccessService>(dataStore);
};

server.Start();
await Task.Delay(-1);