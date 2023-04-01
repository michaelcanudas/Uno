using Uno.Actions;
using Uno.Packets;

namespace Uno.Server;

internal class Uno
{
    private Stack<Card> stack;
    private Stack<Card> discard;
    private List<Card>[] hands;
    
    private Player[] players;
    private Player current;

    private Settings settings;

    public Uno(Player[] players, Settings settings)
    {
        this.stack = StackCards();
        this.discard = new Stack<Card>();
        this.hands = new List<Card>[players.Length];
        for (int i = 0; i < hands.Length; i++)
        {
            hands[i] = new List<Card>();
        }

        this.players = players;
        this.current = players[0];

        this.settings = settings;
    }

    public void Tick()
    {
        foreach (var (id, packet) in Server.Receive<PlayerActionPacket>())
        {
            switch (packet.Action)
            {
                case DrawCardAction action:
                    DrawCard(id, packet.PlayerName, action);
                    break;
                case PlayCardAction action:
                    PlayCard(id, packet.PlayerName, action);
                    break;
            }
        }
    }

    private void DrawCard(int id, string name, DrawCardAction action)
    {
        if (current.Connection != id)
            return;

        if (stack.Count == 0)
            return;

        Card card = stack.Pop();
        int index = Array.IndexOf(players, current);
        hands[index].Add(card);

        Server.Send(id, new PlayerActionPacket(name, new DrawCardAction.Response(card)));
        Server.SendAllExcept(id, new PlayerActionPacket(name, new DrawCardAction.Response(card with { Face = CardFace.Backface })));

        // check if next turn
        Turn();
    }

    private void PlayCard(int id, string name, PlayCardAction action)
    {
        if (current.Connection != id)
            return;

        if (!settings.AllowRed && action.Card.Face.Color == CardColor.Red)
            return;

        Server.SendAll(new PlayerActionPacket(name, new PlayCardAction.Response { PlayedCard = action.Card }));

        // check if next turn
        Turn();
    }

    private void Turn()
    {
        int index = Array.IndexOf(players, current);
        index = (index + 1) % players.Length;

        current = players[index];
    }

    private Stack<Card> StackCards()
    {
        Stack<Card> stack = new();
        List<Card> cards = new List<Card>();

        int id = 1;

        for (int i = 0; i < 4; i++)
        {
            cards.Add(new Card(id++, (CardColor)i, CardKind.Zero));
            for (int j = 0; j < 13; j++)
            {
                if (j == 9)
                    continue;
                
                cards.Add(new Card(id++, (CardColor)i, (CardKind)j));
                cards.Add(new Card(id++, (CardColor)i, (CardKind)j));
            }
        }

        for (int i = 0; i < 4; i++)
        {
            cards.Add(new Card(id++, CardColor.Neutral, CardKind.Wild));
            cards.Add(new Card(id++, CardColor.Neutral, CardKind.WildDraw4));
        }

        
        while (cards.Any())
        {
            int index = Random.Shared.Next(cards.Count);

            stack.Push(cards[index]);
            cards.RemoveAt(index);
        }

        return stack;
    }

    public record Settings(
        bool AllowRed = false
    );
}