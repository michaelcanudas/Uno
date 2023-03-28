namespace Uno.Server;

public static class Program
{
    public static void Main(string[] args)
    {
        int port = args.Length > 0 && !string.IsNullOrEmpty(args[0]) ? int.Parse(args[0]) : 12345;

        Server.Start(port);

        while (Server.IsRunning)
        {
            Server.Tick();
            Game.Tick();
        }

        Server.Destroy();
    }
}