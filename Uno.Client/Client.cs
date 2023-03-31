using System.Diagnostics.CodeAnalysis;
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

    /// <summary>
    /// use this for packets we should not receive multiple of (like packets that trigger scene/menu transitions)
    /// </summary>
    public static bool Receive<T>([NotNullWhen(true)] out T? packet) where T : Packet
    {
        packet = Receive<T>().FirstOrDefault();
        return packet is not null;
    }

    public static void Disconnect()
    {
        client.Disconnect();
    }
}
