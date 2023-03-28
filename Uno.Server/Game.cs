using Uno.Packets;

namespace Uno.Server;

public static class Game
{
    private static State state;
    private static List<Player> players;
    private static HashSet<int> spectators;

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
                foreach (var (id, packet) in Server.Receive<EnterAsPlayerPacket>())
                {
                    // a new player is joining
                    bool valid = IsValidName(packet.Name);

                    valid &= !players.Any(p => p.Name == packet.Name);

                    Server.Send(id, new WelcomePacket(valid, players.Select(p => p.Name).ToArray(), spectators.Count));

                    if (!valid)
                        continue;

                    // register as a player
                    players.Add(new Player(id, packet.Name));

                    // tell every other player
                    foreach(var connection in Server.Connections)
                    {
                        if (connection == id)
                            continue;

                        Server.Send(connection, new PlayerJoinedPacket(packet.Name));
                    }
                }
                foreach (var (id, packet) in Server.Receive<EnterAsSpectatorPacket>())
                {
                    // a new spectator is joining
                    // no need for any kind of verification for spectators as of now
                    Console.WriteLine(spectators.Count);

                    Server.Send(id, new WelcomePacket(true, players.Select(p => p.Name).ToArray(), spectators.Count));
                    spectators.Add(id);

                    Server.SendAll(new SpectatorCountPacket(spectators.Count));
                }
                break;
            case State.Playing:
                break;
        }
    }

    private static bool IsValidName(string name)
    {
        return name.All(char.IsLetterOrDigit);
    }

    private static void HandleConnections()
    {
        foreach ((int id, ConnectPacket packet) in Server.Receive<ConnectPacket>())
        {
            Console.WriteLine($"Connection from: {id}");
        }

        foreach ((int id, DisconnectPacket packet) in Server.Receive<DisconnectPacket>())
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

        foreach (var (id, packet) in Server.Receive<TextPacket>())
        {
            Console.WriteLine($"{id}: {packet.Text}");
        }
    }

    private static void HandleConditions()
    {
        if (players.Count >= 2 && state == State.Waiting)
        {
            state = State.Playing;
            foreach (Player player in players)
            {
                Server.SendAsync(player.Connection, new StartPacket());
            }

            Console.WriteLine("Game started");
        }

        if (players.Count < 2 && state == State.Playing)
        {
            state = State.Waiting;
            foreach (Player player in players)
            {
                Server.SendAsync(player.Connection, new StopPacket());
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