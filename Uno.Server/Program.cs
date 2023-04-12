using System.Diagnostics;

namespace Uno.Server;

public static class Program
{
    public static void Main(string[] args)
    {
        int port = args.Length > 0 && !string.IsNullOrEmpty(args[0]) ? int.Parse(args[0]) : 12345;

        Server.Start(port);

        Task.Run(Admin);
        while (Server.IsRunning)
        {
            Server.Tick();
            Game.Tick();
        }

        Server.Destroy();
    }

    private static void Admin()
    {
        while (Server.IsRunning)
        {
            string command = Console.ReadLine()!;
            string[] args = command.Split();

            if (args.Length == 0)
                continue;

            try
            {
                switch (args[0])
                {
                    case "echo":
                        Console.WriteLine(string.Join(" ", args[1..]));
                        break;
                    case "stop":
                        Server.Stop();
                        break;
                    case "bless":
                        if (args.Length < 3)
                            break;

                        string player = args[1];
                        int count = int.Parse(args[2]);

                        Game.uno.Bless(player, count);
                        break;
                }
            }
            catch
            {
                Console.WriteLine("Invalid command");
            }
        }
    }
}