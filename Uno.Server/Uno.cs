using Uno.Actions;
using Uno.Packets;

namespace Uno.Server;

internal class Uno
{
    private Stack<Card> stack;
    private Stack<Card> discard;
    private List<Card>[] hands;
    
    private Player[] players;
    private int currentPlayer;

    public Uno(Player[] players)
    {
        this.stack = StackCards();
        this.discard = new Stack<Card>();
        this.hands = new List<Card>[players.Length];
        for (int i = 0; i < hands.Length; i++)
        {
            hands[i] = new List<Card>();
        }

        this.players = players;
        this.currentPlayer = 0;
    }

    public void Tick()
    {
        foreach (var (id, packet) in Server.Receive<PlayerActionPacket>())
        {
            //if (id != currentPlayer)
            //    continue;
            
            switch (packet.Action)
            {
                case DrawCardAction:
                    Card card = stack.Pop();
                    hands[currentPlayer].Add(card);

                    Server.Send(id, new PlayerActionPacket(packet.PlayerName, new DrawCardResponse(card)));
                    Server.SendAllExcept(id, packet);
                    break;
                default:
                    // return error packet
                    break;
            }
        }
    }

    private Stack<Card> StackCards()
    {
        Stack<Card> stack = new();
        List<Card> cards = new List<Card>();

        for (int i = 0; i < 4; i++)
        {
            cards.Add(new Card((CardColor)i, CardKind.Zero));
            for (int j = 0; j < 13; j++)
            {
                if (j == 9)
                    continue;
                
                cards.Add(new Card((CardColor)i, (CardKind)j));
                cards.Add(new Card((CardColor)i, (CardKind)j));
            }
        }

        for (int i = 0; i < 4; i++)
        {
            cards.Add(new Card(CardColor.Neutral, CardKind.Wild));
            cards.Add(new Card(CardColor.Neutral, CardKind.WildDraw4));
        }

        
        while (cards.Any())
        {
            int index = Random.Shared.Next(cards.Count);

            stack.Push(cards[index]);
            cards.RemoveAt(index);
        }

        return stack;
    }
}