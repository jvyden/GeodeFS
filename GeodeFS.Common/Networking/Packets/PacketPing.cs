namespace GeodeFS.Common.Networking.Packets;

#nullable disable

public class PacketPing : ITransactionPacket
{
    [Key(0)]
    public int TransactionId { get; set; }
}