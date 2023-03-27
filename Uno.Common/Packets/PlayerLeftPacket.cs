namespace Uno.Packets;

/// <summary>
/// Sent to each client when a player leaves the game
/// </summary>
public class PlayerLeftPacket : Packet
{
    public PlayerLeftPacket()
    {
    }
    public PlayerLeftPacket(string name)
    {
        this.Name = name;
    }

    /// <summary>
    /// the name of the player that left.
    /// </summary>
    public string Name { get; }

}