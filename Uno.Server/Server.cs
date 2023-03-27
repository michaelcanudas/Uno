using Telepathy;
using Uno.Packets;

namespace Uno.Server;

internal static class Server
{
    private static Telepathy.Server server;
    private static List<(int id, Packet packet)> packets;
    
    public static HashSet<int> Connections { get; }

    static Server()
    {
        server = new();
        packets = new();

        Connections = new();
    }
    
    public static void Start(int port)
    {
        server.Start(port);
    }

    public static void Tick()
    {
        packets.Clear();
        
        while (server.GetNextMessage(out Message message))
        {
            if (message.eventType == EventType.Connected)
            {
                packets.Add((message.connectionId, new ConnectPacket()));
                Connections.Add(message.connectionId);
                continue;
            }

            if (message.eventType == EventType.Disconnected)
            {
                packets.Add((message.connectionId, new DisconnectPacket()));
                Connections.Remove(message.connectionId);
                continue;
            }

            Stream stream = new MemoryStream(message.data);
            Packet packet = Packet.Deserialize(stream);
            packets.Add((message.connectionId, packet));
        }
    }

    public static async void SendAsync(int id, Packet packet)
    {
        await Task.Run(() =>
        {
            Send(id, packet);
        });
    }

    public static void Send(int id, Packet packet)
    {
        MemoryStream stream = new MemoryStream();
        packet.Serialize(stream);

        server.Send(id, stream.ToArray());
    }

    public static void SendAll(Packet packet)
    {
        SendAllExcept(-1, packet);
    }

    public static void SendAllExcept(int except, Packet packet)
    {
        MemoryStream stream = new MemoryStream();
        packet.Serialize(stream);
        var data = stream.ToArray();

        foreach (var connection in Connections)
        {
            if (connection == except)
                continue;

            server.Send(connection, data);
        }
    }
    
    public static IEnumerable<(int, T)> Receive<T>() where T : Packet
    {
        return packets.Where(data => data.packet is T).Select(data => (data.id, (data.packet as T)!));
    }
}