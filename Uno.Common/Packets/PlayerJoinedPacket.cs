using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Uno.Packets;

/// <summary>
/// The server broadcasts one of these to every other client when a new player joins.
/// </summary>
public class PlayerJoinedPacket : Packet
{
    // required for packet deserializer
    public PlayerJoinedPacket()
    {
    }

    public PlayerJoinedPacket(string name)
    {
        this.Name = name;
    }

    /// <summary>
    /// The name of the new player.
    /// </summary>
    public string Name { get; set; }
}