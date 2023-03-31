using Uno.Packets;

namespace Uno.Server;

public static class Game
{
    private static State state;
    private static List<Player> players;
    private static HashSet<int> spectators;
    private static int elevatedConnection;

    private const int MIN_PLAYERS = 2;
    private const int MAX_PLAYERS = 10;

    static Game()
    {
        state = State.Waiting;
        players = new();
        spectators = new();
    }
    
    public static void Tick()
    {
        HandleConnections();

        HandleConditions();

        switch (state)
        {
            case State.Waiting:
                Waiting();
                break;
            case State.Playing:
                Playing();
                break;
        }
    }

    private static void HandleConnections()
    {
        foreach (var (id, packet) in Server.Receive<ConnectPacket>())
        {
            Console.WriteLine($"Connection from: {id}");
        }

        foreach (var (id, packet) in Server.Receive<DisconnectPacket>())
        {
            // try to find a player for the disconnected id. there may not be one (ie was on name select screen)
            var disconnectedPlayer = players.SingleOrDefault(p => p.Connection == id);

            if (disconnectedPlayer is not null) 
            {
                players.Remove(disconnectedPlayer);

                Server.SendAll(new PlayerLeftPacket(disconnectedPlayer.Name));
            }
            else if (spectators.Contains(id))
            {
                // a spectator left;
                spectators.Remove(id);

                Server.SendAll(new SpectatorCountPacket(spectators.Count));
            }
            // otherwise, a player on the name select screen left. Whatever.

            Console.WriteLine($"Disconnect from: {id}");
        }
    }

    private static void HandleConditions()
    {
        // commented out cause i think i added a start button :D
        // if (players.Count >= MIN_PLAYERS && state == State.Waiting)
        // {
        //     state = State.Playing;
        // 
        //     Server.SendAll(new StartPacket());
        // 
        //     Console.WriteLine("Game started");
        // }

        if (players.Count < MIN_PLAYERS && state == State.Playing)
        {
            state = State.Waiting;

            Server.SendAll(new StopPacket());
            Server.KickAll();

            Console.WriteLine("Game stopped");
        }
    }

    private static void Waiting()
    {
        foreach (var (id, packet) in Server.Receive<EnterAsPlayerPacket>())
        {
            bool valid = IsValidName(packet.Name);
            valid &= !players.Any(p => p.Name == packet.Name);
            valid &= !(players.Count() >= MAX_PLAYERS);

            if (!valid)
            {
                // send a failed welcome packet so the client doesn't softlock
                Server.Send(id, new WelcomePacket(false, false, Array.Empty<string>(), 0));
                continue;
            }

            // first player becomes elevated
            bool elevated = !players.Any();

            if (elevated)
            {
                elevatedConnection = id;
            }

            players.Add(new Player(id, packet.Name));

            Server.Send(id, new WelcomePacket(valid, elevated, players.Select(p => p.Name).ToArray(), spectators.Count));
            Server.SendAllExcept(id, new PlayerJoinedPacket(packet.Name));
        }
        
        foreach (var (id, packet) in Server.Receive<EnterAsSpectatorPacket>())
        {
            spectators.Add(id);
            
            Server.Send(id, new WelcomePacket(true, false, players.Select(p => p.Name).ToArray(), spectators.Count));
            Server.SendAll(new SpectatorCountPacket(spectators.Count));
        }

        if (Server.Receive<StartPacket>(out int connection, out _))
        {
            if (connection == elevatedConnection && players.Count >= MIN_PLAYERS)
            {
                state = State.Playing;
                Server.SendAll(new StartPacket());
            }
        }
    }

    private static void Playing()
    {
        foreach (var (id, packet) in Server.Receive<PlayerActionPacket>())
        {
            Server.SendAllExcept(id, packet);
        }
    }

    private static bool IsValidName(string name)
    {
        return name.All(char.IsLetterOrDigit);
    }

    enum State
    {
        Waiting,
        Playing,
    }
}