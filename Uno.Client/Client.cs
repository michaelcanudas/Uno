using Telepathy;

namespace Uno.Client;

internal class Client
{
    private static Telepathy.Client client;
    private static List<Packet> packets;

    static Client()
    {
        client = new();
        packets = new();
    }

    public static async Task StartAsync(string ip, int port)
    {
        await Task.Run(() =>
        {
            client.Connect(ip, port);
            while (!client.Connected) { }
        });
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
            packets.Add(Packet.Deserialize(stream));
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

    public static IEnumerable<Packet> Receive<T>() where T : Packet
    {
        return packets.OfType<T>();
    }
}
