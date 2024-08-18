namespace GeodeFS.Common.Networking.Packets;

public interface ITransactionPacket : IPacket
{
    /// <summary>
    /// How long to wait for a response in a transaction before failing.
    /// </summary>
    public const int MaxTransactionWaitMilliseconds = 500;
    
    public int TransactionId { get; set; }
}