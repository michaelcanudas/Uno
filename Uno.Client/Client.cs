using Telepathy;
using Uno.Packets;

namespace Uno.Client;

internal static class Client
{
    private static Telepathy.Client client;
    private static List<Packet> packets;
    
    public static bool IsConnected => client.Connected;
    public static bool IsConnecting => client.Connecting;

    static Client()
    {
        client = new(4096);
        packets = new();

        client.OnConnected = Connected;
        client.OnData = Data;
        client.OnDisconnected = Disconnected;
    }

    public static void Start(string ip, int port)
    {
        client.Connect(ip, port);
    }

    public static void Tick()
    {
        packets.Clear();
        client.Tick(100);
    }

    private static void Connected()
    {
        packets.Add(new ConnectPacket());
    }

    private static void Data(ArraySegment<byte> data)
    {
        Stream stream = new MemoryStream(data.ToArray());
        Packet packet = Packet.Deserialize(stream);

        packets.Add(packet);
    }

    private static void Disconnected()
    {
        packets.Add(new DisconnectPacket());
    }

    public static void Send(Packet packet)
    {
        using MemoryStream stream = new();
        packet.Serialize(stream);
        client.Send(stream.GetBuffer());
    }

    public static IEnumerable<T> Receive<T>() where T : Packet
    {
        return packets.OfType<T>();
    }

    public static void Disconnect()
    {
        client.Disconnect();
    }
}
