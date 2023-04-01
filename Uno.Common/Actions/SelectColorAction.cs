namespace Uno.Actions;

public class SelectColorAction : PlayerAction
{
    public CardColor Color { get; set; }
    
    public class Response : PlayerAction
    {
        public CardColor Color { get; set; }
    }
}
