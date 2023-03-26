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