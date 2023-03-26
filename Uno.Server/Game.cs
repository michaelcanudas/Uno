using Uno.Packets;

namespace Uno.Server;

internal static class Game
{
    private static State state;
    private static List<int> clients;
    
    static Game()
    {
        state = State.Waiting;
        clients = new();
    }
    
    public static void Tick()
    {
        HandleConnections();

        HandleConditions();

        switch (state)
        {
            case State.Waiting:
                break;
            case State.Playing:
                break;
        }
    }

    private static void HandleConnections()
    {
        foreach ((int id, ConnectPacket packet) in Server.Receive<ConnectPacket>())
        {
            clients.Add(id);
            Console.WriteLine($"Connection from: {id}");
        }

        foreach ((int id, DisconnectPacket packet) in Server.Receive<DisconnectPacket>())
        {
            clients.Remove(id);
            Console.WriteLine($"Disconnect from: {id}");
        }
    }

    private static void HandleConditions()
    {
        if (clients.Count >= 2 && state == State.Waiting)
        {
            state = State.Playing;
            foreach (int id in clients)
            {
                Server.SendAsync(id, new StartPacket());
            }

            Console.WriteLine("Game started");
        }

        if (clients.Count < 2 && state == State.Playing)
        {
            state = State.Waiting;
            foreach (int id in clients)
            {
                Server.SendAsync(id, new StopPacket());
            }

            Console.WriteLine("Game stopped");
        }
    }

    enum State
    {
        Waiting,
        Playing,
    }
}