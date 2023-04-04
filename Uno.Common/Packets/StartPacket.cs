namespace Uno.Packets;

public class StartPacket : Packet
{
    public string[] Players;
    public Card[][] StartingHands;
    public Card StartingDiscard;

    public StartPacket()
    {
    }

    public Card[] GetStartingHand(string player)
    {
        return StartingHands[Array.IndexOf(Players, player)];
    }
}
