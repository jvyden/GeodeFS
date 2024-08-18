using GeodeFS.Common.Networking.Packets;

namespace GeodeFS.Common.Networking;

public class TransactionResponse
{
    public readonly string Source;
    public readonly ITransactionPacket Packet;

    public int Id => this.Packet.TransactionId;

    public TransactionResponse(string source, ITransactionPacket packet)
    {
        this.Source = source;
        this.Packet = packet;
    }
}