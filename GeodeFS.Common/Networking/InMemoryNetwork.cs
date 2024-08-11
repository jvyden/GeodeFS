using System.Collections.Concurrent;

namespace GeodeFS.Common.Networking;

public class InMemoryNetwork : IDisposable
{
    public InMemoryNetwork(bool automatic = true)
    {
        if (!automatic)
            return;

        // ReSharper disable once UseObjectOrCollectionInitializer
        Thread thread = new(() =>
        {
            while (!_disposed)
            {
                ProcessTasks();
                Thread.Sleep(10);
            }
        });
        thread.Name = $"{nameof(InMemoryNetwork)} Task Queue";
        
        thread.Start();
    }

    private readonly ConcurrentQueue<Action> _taskQueue = new();
    
    public void ProcessTasks()
    {
        while (_taskQueue.TryDequeue(out Action? action))
        {
            action();
        }
    }

    internal void QueueTask(Action action)
    {
        this._taskQueue.Enqueue(action);
    }

    public readonly List<InMemoryNetworkBackend> Clients = [];

    private bool _disposed;
    public void Dispose()
    {
        _disposed = true;
        ProcessTasks();

        GC.SuppressFinalize(this);
    }
}