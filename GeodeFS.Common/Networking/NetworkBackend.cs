using System.Diagnostics;
using GeodeFS.Common.Federation;
using GeodeFS.Common.Networking.Packets;
using NotEnoughLogs;

namespace GeodeFS.Common.Networking;

public abstract class NetworkBackend
{
    protected readonly Logger Logger;

    protected NetworkBackend(Logger logger)
    {
        this.Logger = logger;
    }

    public event Action<string, IPacket>? OnPacket;
    
    protected readonly List<int> TransactionRequests = [];
    protected readonly List<TransactionResponse> TransactionResponses = [];

    public abstract void SendPacket(string destination, IPacket packet);
    public void SendPacket(GeodeNode node, IPacket packet) => this.SendPacket(node.Source, packet);

    public TResponse? DoTransaction<TResponse>(GeodeNode node, ITransactionPacket packet)
        where TResponse : class, ITransactionPacket
    {
        int id = Random.Shared.Next();
        // int id = 1;
        packet.TransactionId = id;
        
        this.SendPacket(node, packet);
        this.Logger.LogTrace(GeodeCategory.Transaction, $"Starting transaction {id}");
        this.TransactionRequests.Add(id);
        
        Stopwatch sw = Stopwatch.StartNew();
        
        ITransactionPacket? response = null;
        
        while (sw.ElapsedMilliseconds < ITransactionPacket.MaxTransactionWaitMilliseconds)
        {
            TransactionResponse? resp = this.TransactionResponses.FirstOrDefault(r => r.Id == id & r.Source == node.Source);
            if (resp != null)
            {
                this.Logger.LogTrace(GeodeCategory.Transaction, $"Found {resp.Id} in responses! Transaction is complete.");
                response = resp.Packet;
                break;
            }
            
            Thread.Sleep(5);
        }

        this.TransactionRequests.Remove(id);
        return response as TResponse;
    }
    
    public abstract void Handshake(string source);

    protected void FireOnPacket(string source, IPacket packet)
    {
        ITransactionPacket? transactionPacket = packet as ITransactionPacket;
        if (transactionPacket != null && this.TransactionRequests.Contains(transactionPacket.TransactionId))
        {
            this.Logger.LogTrace(GeodeCategory.Transaction, $"Got response for transaction {transactionPacket.TransactionId}");
            this.TransactionResponses.Add(new TransactionResponse(source, transactionPacket));
            return;
        }
        
        if (transactionPacket != null)
        {
            this.Logger.LogTrace(GeodeCategory.Transaction, $"Got request for transaction {transactionPacket.TransactionId}");
        }

        OnPacket?.Invoke(source, packet);
    }
}