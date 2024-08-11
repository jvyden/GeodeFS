namespace GeodeFS.Common.Networking.Packets;

[Union(0, typeof(PacketHandshake))]
[Union(1, typeof(PacketShareNode))]
public interface IPacket;