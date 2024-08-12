namespace GeodeFS.Common.Networking.Packets;

[Union(0, typeof(PacketHandshake))]
[Union(1, typeof(PacketShareNode))]
[Union(2, typeof(PacketShareUser))]
public interface IPacket;