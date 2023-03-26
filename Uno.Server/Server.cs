using Telepathy;
using Uno.Packets;

namespace Uno.Server;

internal static class Server
{
    private static Telepathy.Server server;
    private static List<(int id, Packet packet)> packets;
    
    static Server()
    {
        server = new();
        packets = new();
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
                continue;
            }

            if (message.eventType == EventType.Disconnected)
            {
                packets.Add((message.connectionId, new DisconnectPacket()));
                continue;
            }

            Stream stream = new MemoryStream(message.data);
            packets.Add((message.connectionId, Packet.Deserialize(stream)));
        }
    }

    public static async void SendAsync(int id, Packet packet)
    {
        await Task.Run(() =>
        {
            MemoryStream stream = new MemoryStream();
            packet.Serialize(stream);

            server.Send(id, stream.ToArray());
        });
    }
    
    public static IEnumerable<(int, T)> Receive<T>() where T : Packet
    {
        return packets.Where(data => data.packet is T).Select(data => (data.id, (data.packet as T)!));
    }
}