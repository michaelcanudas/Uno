using System.Diagnostics.CodeAnalysis;
using Telepathy;
using Uno.Packets;

namespace Uno.Server;

public static class Server
{
    private static Telepathy.Server server;
    private static List<(int id, Packet packet)> packets;
    
    public static bool IsRunning { get; private set; }
    public static int Port { get; private set; }

    public static HashSet<int> Connections { get; private set; }

    public static void Start(int port)
    {
        server = new(4096);
        packets = new();
        Connections = new();

        server.OnConnected = Connected;
        server.OnData = Data;
        server.OnDisconnected = Disconnected;
        
        server.Start(port);
        IsRunning = true;
        Port = port;
    }

    public static void Tick()
    {
        packets.Clear();
        server.Tick(100);
    }

    private static void Connected(int id)
    {
        packets.Add((id, new ConnectPacket()));
        Connections.Add(id);
    }

    private static void Data(int id, ArraySegment<byte> data)
    {
        Stream stream = new MemoryStream(data.ToArray());
        Packet packet = Packet.Deserialize(stream);

        packets.Add((id, packet));
    }

    private static void Disconnected(int id)
    {
        packets.Add((id, new DisconnectPacket()));
        Connections.Remove(id);
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
        foreach (var connection in Connections)
        {
            if (connection == except)
                continue;

            Send(connection, packet);
        }
    }
    
    public static IEnumerable<(int, T)> Receive<T>() where T : Packet
    {
        return packets.Where(data => data.packet is T).Select(data => (data.id, (data.packet as T)!));
    }

    public static bool Receive<T>([NotNullWhen(true)] out int connection, [NotNullWhen(true)] out T? packet) where T : Packet
    {
        (connection, packet) = Receive<T>().FirstOrDefault();
        return packet is not null;
    }

    public static void Kick(int id)
    {
        server.Disconnect(id);
    }

    public static void KickAll()
    {
        foreach (var connection in Connections)
        {
            Kick(connection);
        }
    }

    public static void Stop()
    {
        IsRunning = false;
    }

    public static void Destroy()
    {
        server.Stop();
    }
}