namespace Uno.Packets;

public class TextPacket : Packet
{
    public string Text { get; set; }

    public TextPacket()
    {
        Text = "";
    }

    public TextPacket(string text)
    {
        Text = text;
    }
}
