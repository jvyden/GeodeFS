namespace GeodeFS.Common.Networking.Packets;

#nullable disable

public class PacketPong : ITransactionPacket
{
    public PacketPong()
    {}

    public PacketPong(int transactionId)
    {
        this.TransactionId = transactionId;
    }

    [Key(0)]
    public int TransactionId { get; set; }
}