using Telepathy;

namespace Uno.Client;

internal static class Client
{
    private static Telepathy.Client client;
    private static List<Packet> packets;

    public static bool IsConnected => client.Connected;
    public static bool IsConnecting => client.Connecting;

    static Client()
    {
        client = new();
        packets = new();
    }

    public static void Start(string ip, int port)
    {
        client.Connect(ip, port);
    }

    public static void Tick()
    {
        packets.Clear();

        while (client.GetNextMessage(out Message message))
        {
            if (message.eventType == EventType.Connected || message.eventType == EventType.Disconnected)
            {
                continue;
            }

            Stream stream = new MemoryStream(message.data);

            var packet = Packet.Deserialize(stream);
            packets.Add(packet);
            Console.WriteLine("Received packet: " + packet.ToString());
        }
    }

    public static async Task SendAsync(Packet packet)
    {
        await Task.Run(() =>
        {
            MemoryStream stream = new MemoryStream();
            packet.Serialize(stream);

            client.Send(stream.ToArray());
        });
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
