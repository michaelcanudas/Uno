using Telepathy;
using Uno;

namespace Uno.Client;

class Client
{
    // Hello Michael
}

//Client client = new();
//client.Connect("127.0.0.1", 12345);

//while (true)
//{
//    while (client.GetNextMessage(out Message msg))
//    {
//        switch (msg.eventType)
//        {
//            case EventType.Connected:
//                OnConnect(msg);
//                break;
//            case EventType.Data:
//                OnData(msg);
//                break;
//            case EventType.Disconnected:
//                OnDisconnect(msg);
//                break;
//        }
//    }
//}

//void OnConnect(Message msg)
//{
//    Console.WriteLine("Connected");
//}

//void OnData(Message msg)
//{
//    Console.WriteLine("Received: " + msg.data);
//}

//void OnDisconnect(Message msg)
//{
//    Console.WriteLine("Disconnected");
//}