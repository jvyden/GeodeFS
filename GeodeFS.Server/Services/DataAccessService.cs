namespace GeodeFS.Server.Services;

public class DataAccessService : EndpointService
{
    private IDataStore _dataStore;

    internal DataAccessService(Logger logger, IDataStore dataStore) : base(logger)
    {
        this._dataStore = dataStore;
    }
}