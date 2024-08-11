namespace GeodeFS.Server.Services;

public class DataAccessService : EndpointService
{
    private IDataStore _dataStore;
    private FederationService _federationService;

    internal DataAccessService(Logger logger, IDataStore dataStore, FederationService federationService) : base(logger)
    {
        this._dataStore = dataStore;
        this._federationService = federationService;
    }
}