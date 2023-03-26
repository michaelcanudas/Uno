using Uno.Packets;

namespace Uno.Server;

internal static class Program
{
    private static void Main(string[] args)
    {
        Server.Start(12345);

        while (true)
        {
            Server.Tick();
            Game.Tick();
        }
    }
}

internal static class Game
{
    public static void Tick()
    {
        foreach ((int id, TextPacket packet) in Server.Receive<TextPacket>())
        {
            Console.WriteLine($"Received message of: {packet.Text} from {id}");
            Server.SendAsync(id, new TextPacket("Message has been receieved"));
        }
    }
}