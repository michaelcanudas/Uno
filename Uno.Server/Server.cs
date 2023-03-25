using Telepathy;

public class Program
{
    public static void Main(string[] args)
    {
        while (true)
        {
            Server.Instance.Tick();
        }
    }
}

public class Packet
{
    public Packet()
    {
        Server.Instance.Packets 
    }
}

public class Server
{
    public static Server Instance { get; } = new();
    
    private Telepathy.Server server;
    
    public Server()
    {
        server = new();
        server.Start(12345);
    }

    public void Tick()
    {
        while (server.GetNextMessage(out Message msg))
        {
            switch (msg.eventType)
            {
                case EventType.Connected:
                    OnConnect(msg);
                    break;
                case EventType.Data:
                    OnData(msg);
                    break;
                case EventType.Disconnected:
                    OnDisconnect(msg);
                    break;
            }
        }
    }

    private void OnConnect(Message message)
    {
        
    }

    private void OnData(Message message)
    {

    }

    private void OnDisconnect(Message message)
    {

    }
}