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

    private CardColor? selectedColor => discard.Count > 0 ? discard.Peek().Face.Kind switch
    {
        CardKind.Wild => CardColor.Blue,
        CardKind.WildDraw4 => CardColor.Blue,
        _ => null
    } : null;
    private bool isSkip => discard.Count > 0 && discard.Peek().Face.Kind is CardKind.Skip;
    private bool isReverse => discard.Count > 0 && discard.Peek().Face.Kind is CardKind.Reverse;
    private bool isAscending = true;

    public Uno(Player[] players, Settings settings)
    {
        this.stack = StackCards();
        
        this.discard = new Stack<Card>();
        Move(stack, discard);

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
                case SelectColorAction action:
                    SelectColor(id, packet.PlayerName, action);
                    break;
            }
        }
    }

    private void DrawCard(int id, string name, DrawCardAction action)
    {
        if (current.Connection != id)
            return;

        int index = Array.IndexOf(players, current);
        Card? card = Move(stack, hands[index]);

        if (card is null)
            return;

        Server.Send(id, new PlayerActionPacket(name, new DrawCardAction.Response(card)));
        Server.SendAllExcept(id, new PlayerActionPacket(name, new DrawCardAction.Response(card with { Face = CardFace.Backface })));

        Turn();
    }

    private void PlayCard(int id, string name, PlayCardAction action)
    {
        if (current.Connection != id)
            return;

        if (!settings.AllowRed && action.Card.Face.Color == CardColor.Red)
            return;

        if (selectedColor is null)
        {
            if (action.Card.Face.Kind != CardKind.Wild && action.Card.Face.Kind != CardKind.WildDraw4)
                if (action.Card.Face.Color != discard.Peek().Face.Color && action.Card.Face.Kind != discard.Peek().Face.Kind)
                    return;
        }
        else
        {
            if (action.Card.Face.Kind != CardKind.Wild && action.Card.Face.Kind != CardKind.WildDraw4)
                if (action.Card.Face.Color != selectedColor)
                    return;
        }

        int index = Array.IndexOf(players, current);
        bool success = Move(action.Card, hands[index], discard);

        if (!success)
            return;

        Server.SendAll(new PlayerActionPacket(name, new PlayCardAction.Response { PlayedCard = action.Card }));

        //if (action.Card.Face.Kind == CardKind.Wild || action.Card.Face.Kind == CardKind.WildDraw4)
        //    return; [wait for next turn so color is set]
        
        Turn();
    }

    private void SelectColor(int id, string name, SelectColorAction action)
    {
        if (current.Connection != id)
            return;

        if (!settings.AllowRed && action.Color == CardColor.Red)
            return;

        Server.SendAll(new PlayerActionPacket(name, new SelectColorAction.Response { Color = action.Color }));

        Turn();
    }

    private void Turn()
    {
        if (isReverse)
            isAscending = !isAscending;

        int index = Array.IndexOf(players, current);
        int amount = isSkip ? 2 : 1;
        amount *= isAscending ? -1 : 1;
        
        index = (index + amount) % players.Length;
        if (index < 0)
            index += players.Length;

        current = players[index];
    }

    private bool Move(Card card, List<Card> from, List<Card> to)
    {
        if (!from.Contains(card))
            return false;

        from.Remove(card);
        to.Add(card);

        return true;
    }

    private bool Move(Card card, List<Card> from, Stack<Card> to)
    {
        if (!from.Contains(card))
            return false;

        from.Remove(card);
        to.Push(card);

        return true;
    }

    private Card? Move(Stack<Card> from, List<Card> to)
    {
        if (from.Count == 0)
            return null;
        
        Card card = from.Pop();
        to.Add(card);

        return card;
    }

    private Card? Move(Stack<Card> from, Stack<Card> to)
    {
        if (from.Count == 0)
            return null;

        Card card = from.Pop();
        to.Push(card);

        return card;
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