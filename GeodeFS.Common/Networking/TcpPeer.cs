using System.Net;
using System.Net.Sockets;
using System.Text;
using GeodeFS.Common.Networking.Packets;

namespace GeodeFS.Common.Networking;

public class TcpPeer : IDisposable
{
    public TcpPeer(Action<string, IPacket> onRead, Socket socket, bool isClient)
    {
        IPEndPoint endpoint = (IPEndPoint)socket.RemoteEndPoint!;
        this.Address = endpoint.Address.ToString();
        // me when i am on the team for the .NET stdlib and i do not know about basic data types
        // (i hate myself and i hate our users even more)
        this.Port = (ushort)endpoint.Port;

        this.IsClient = isClient;

        this._socket = socket;
        this._stream = new NetworkStream(socket, false);

        this._writer = new BinaryWriter(this._stream, Encoding.UTF8, true);
        this._reader = new BinaryReader(this._stream, Encoding.UTF8, true);
        
        Task.Factory.StartNew(() =>
        {
            while (!this._disposed)
            {
                IPacket packet = this.ReadPacket();
                onRead(this.Address, packet);
            }
        }, TaskCreationOptions.LongRunning);
    }

    public readonly string Address;
    public readonly ushort Port;
    
    private readonly Socket _socket;
    private readonly NetworkStream _stream;
    private readonly BinaryWriter _writer;
    private readonly BinaryReader _reader;

    public void SendPacket(IPacket packet)
    {
        byte[] data = MessagePackSerializer.Serialize(packet);
        this._writer.Write7BitEncodedInt(data.Length);
        this._writer.Write(data);
    }
    
    public IPacket ReadPacket()
    {
        int length = this._reader.Read7BitEncodedInt();
        byte[] data = this._reader.ReadBytes(length);

        IPacket packet = MessagePackSerializer.Deserialize<IPacket>(data);
        return packet;
    }

    /// <summary>
    /// True if this socket is connecting to us, or false if we are connecting to them.
    /// </summary>
    public readonly bool IsClient;

    private bool _disposed;
    public void Dispose()
    {
        this._disposed = true;

        this._socket.Dispose();
        this._stream.Dispose();
        this._writer.Dispose();
        
        GC.SuppressFinalize(this);
    }
}