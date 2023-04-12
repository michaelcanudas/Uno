using System.Linq;
using System.Numerics;
using System.Xml.Linq;
using Uno.Actions;
using Uno.Packets;

namespace Uno.Server;

public class Uno
{
    private Stack<Card> stack;
    private Stack<Card> discard;
    private List<Card>[] hands;
    
    private Player[] players;
    private Player current;

    private Settings settings;

    private bool isSkip => discard.Count > 0 && discard.Peek().Face.Kind is CardKind.Skip;
    private bool isReverse => discard.Count > 0 && discard.Peek().Face.Kind is CardKind.Reverse;
    private bool isAscending = false;
    private bool isSelectingColor = false;

    public Uno(Player[] players, Settings settings)
    {
        this.hands = new List<Card>[players.Length];
        for (int i = 0; i < hands.Length; i++)
        {
            hands[i] = new List<Card>();
        }

        this.players = players;
        this.current = players[0];
        this.settings = settings;

        this.stack = StackCards();
        this.discard = new Stack<Card>();

        // initial card in discard pile
        Move(stack, discard);

        // deal initial hands
        foreach (var player in players)
        {
            var hand = GetHand(player);

            for (int i = 0; i < settings.startingCardCount; i++)
            {
                Move(stack, hand);
            }
        }

        // now that initial game state is setup, give all the clients the initial state
        foreach (var player in players)
        {
            Server.Send(player.Connection, new StartPacket
            {
                Players = this.players.Select(p => p.Name).ToArray(),
                StartingHands = hands.Select(h => h == GetHand(player) ? h.ToArray() : h.Select(c => c with { Face = CardFace.Backface } ).ToArray()).ToArray(),
                StartingDiscard = discard.Peek()
            });
        }
    }

    public void Bless(string name, int count)
    {
        CardFace.Random();

        Player player = players.FirstOrDefault(p => p.Name == name)!;
        DrawCard(player.Connection, player.Name, new DrawCardAction(), count);
    }

    public void Tick()
    {
        foreach (var (id, packet) in Server.Receive<PlayerActionPacket>())
        {
            // for now, only accept actions from the player who's turn it is
            // we will need to change this to add challenges and jump ins though
            if (current.Connection != id)
                return;

            switch (packet.Action)
            {
                case DrawCardAction action:
                    DrawCard(id, packet.PlayerName, action);

                    if (!settings.PostPickupPlace)
                        Turn();
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

    private List<Card> GetHand(Player player)
    {
        return hands[Array.IndexOf(players, player)];
    }

    private void DrawCard(int id, string name, DrawCardAction action, int count = 1)
    {
        List<Card> cards = new();
        int index = Array.IndexOf(players, players.Single(p => p.Name == name));
        List<Card> hand = hands[index];

        for (int i = 0; i < count; i++)
        {
            Card? card = Move(stack, hand);

            if (card is null)
            {
                ReStackCards();
                card = Move(stack, hand);
            }

            cards.Add(card!);
        }

        Server.Send(id, new PlayerActionPacket(name, new DrawCardAction.Response(cards.ToArray())));
        Server.SendAllExcept(id, new PlayerActionPacket(name, new DrawCardAction.Response(cards.Select(c => c with { Face = CardFace.Backface }).ToArray())));
    }

    private void PlayCard(int id, string name, PlayCardAction action)
    {
        if (isSelectingColor)
            return;

        if (!settings.AllowRed && action.Card.Face.Color == CardColor.Red)
            return;

        if (discard.TryPeek(out var topCard))
        {
            if (!action.Card.Face.CanBePlayedOn(topCard.Face))
            {
                return;
            }
        }

        int index = Array.IndexOf(players, current);
        bool success = Move(action.Card, hands[index], discard);

        if (!success)
            return;

        Server.SendAll(new PlayerActionPacket(name, new PlayCardAction.Response { PlayedCard = action.Card }));

        if (action.Card.Face.Kind.IsDraw(out int count))
        {
            DrawCard(GetNextPlayer().Connection, GetNextPlayer().Name, new DrawCardAction(), count);
        }

        if (action.Card.Face.Kind.IsWild())
        {
            isSelectingColor = true;
            return; // wait for next turn so color is set
        }
        
        Turn();
    }

    private void SelectColor(int id, string name, SelectColorAction action)
    {
        if (!settings.AllowRed && action.Color == CardColor.Red)
            return;
        
        var topCard = discard.Peek();
        topCard.Face = new(topCard.Face.Kind, action.Color);

        Server.SendAll(new PlayerActionPacket(name, new SelectColorAction.Response { Color = action.Color }));

        isSelectingColor = false;
        
        Turn();

        if (topCard.Face.Kind is CardKind.WildDraw4)
        {
            // turn() twice to skip the recipient of the 4 cards
            Turn();
        }
    }

    private void Turn()
    {
        if (isReverse)
            isAscending = !isAscending;

        current = GetNextPlayer();
    }

    private Player GetNextPlayer()
    {
        int index = Array.IndexOf(players, current);
        int amount = isSkip ? 2 : 1;
        amount *= isAscending ? -1 : 1;

        index = (index + amount) % players.Length;
        if (index < 0)
            index += players.Length;

        return players[index];
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

        ShuffleCards(cards, stack);

        return stack;
    }

    private void ReStackCards()
    {
        Card top = discard.Pop();
        
        if (discard.Count == 0)
        {
            discard = StackCards();
        }

        while (discard.Count > 0)
        {
            Move(discard, stack);
        }
        discard.Push(top);

        Stack<Card> newStack = new Stack<Card>();
        ShuffleCards(stack.ToList(), newStack);

        stack = newStack;
    }

    private void ShuffleCards(List<Card> cards, Stack<Card> stack)
    {
        while (cards.Any())
        {
            int index = Random.Shared.Next(cards.Count);

            stack.Push(cards[index]);
            cards.RemoveAt(index);
        }
    }

    public record Settings(
        bool AllowRed = false,
        bool PostPickupPlace = true,
        int startingCardCount = 7
    );
}