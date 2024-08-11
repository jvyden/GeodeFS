using System.Reflection;
using GeodeFS.Common.Federation;
using GeodeFS.Common.Networking;
using GeodeFS.Server.Configuration;
using GeodeFS.Server.Services;
using NotEnoughLogs.Behaviour;

FederationController controller = new(new NullNetworkBackend());

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

    s.AddService<FederationService>(controller);

    s.AddStorageService(dataStore);
    s.AddService<DataAccessService>(dataStore);
};

server.Start();
await Task.Delay(-1);