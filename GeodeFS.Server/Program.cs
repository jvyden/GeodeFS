using System.Reflection;
using Bunkum.Core.Configuration;
using GeodeFS.Common.Federation;
using GeodeFS.Common.Networking;
using GeodeFS.Server.Configuration;
using GeodeFS.Server.Services;
using NotEnoughLogs.Behaviour;

GeodeConfig config = Config.LoadFromJsonFile<GeodeConfig>("geode.json", new Logger());

FederationController controller = new(new TcpNetworkBackend(config.ListenPort));
foreach (string node in config.DirectPeers)
    controller.HandshakeWithNode(node);

BunkumConsole.AllocateConsole();

BunkumServer server = new BunkumHttpServer(new LoggerConfiguration
{
    Behaviour = new QueueLoggingBehaviour(),
    MaxLevel = LogLevel.Trace
});

server.Initialize = s =>
{
    FileSystemDataStore dataStore = new();
    s.AddConfig(config);

    s.DiscoverEndpointsFromAssembly(Assembly.GetExecutingAssembly());

    s.AddService<FederationService>(controller);

    s.AddStorageService(dataStore);
    s.AddService<DataAccessService>(dataStore);
};

server.Start();
await Task.Delay(-1);