namespace Uno.Actions;

public class DrawCardResponse : PlayerAction
{
    public Card Card { get; set; }

    public DrawCardResponse(Card card)
    {
        Card = card;
    }
}
