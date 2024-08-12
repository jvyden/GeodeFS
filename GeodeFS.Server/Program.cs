using System.Reflection;
using Bunkum.Core.Configuration;
using Bunkum.Core.Responses.Serialization;
using GeodeFS.Common.Federation;
using GeodeFS.Common.Networking;
using GeodeFS.Database;
using GeodeFS.Server;
using GeodeFS.Server.Configuration;
using GeodeFS.Server.Services;
using NotEnoughLogs.Behaviour;

BunkumConsole.AllocateConsole();
BunkumServer server = new BunkumHttpServer(new LoggerConfiguration
{
    Behaviour = new QueueLoggingBehaviour(),
    MaxLevel = LogLevel.Trace
});

Logger logger = server.Logger;

GeodeConfig config = Config.LoadFromJsonFile<GeodeConfig>("geode.json", logger);

FederationController controller = new(new TcpNetworkBackend(logger, config.ListenPort), logger);
foreach (string node in config.DirectPeers)
    controller.HandshakeWithNode(node);

server.Initialize = s =>
{
    FileSystemDataStore dataStore = new();
    s.AddConfig(config);

    s.UseDatabaseProvider(new GeodeDbProvider());
    s.DiscoverEndpointsFromAssembly(Assembly.GetExecutingAssembly());

    s.AddService<FederationService>(controller);

    s.AddStorageService(dataStore);
    s.AddService<DataAccessService>(dataStore);
    
    s.AddService<DebugService>();

    s.RemoveSerializer<BunkumJsonSerializer>();
    s.AddSerializer<GeodeJsonSerializer>();
};

server.Start();
await Task.Delay(-1);