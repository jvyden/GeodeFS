namespace GeodeFS.Common.Networking.Packets;

#nullable disable

[MessagePackObject]
public class PacketShareUser : IPacket
{
    public PacketShareUser()
    {}

    public PacketShareUser(GeodeUser user)
    {
        this.User = user;
    }
    
    [Key(0)]
    public GeodeUser User { get; set; }
}