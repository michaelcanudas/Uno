using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Uno.Packets;

/// <summary>
/// Packet for a client to enter a lobby as a player. Should be the first packet send to the server by the client upon connection. In order for the client to become a player in the game it needs to send this packet.
/// </summary>
public class EnterAsPlayerPacket : Packet
{
    // we can't just create a new player upon connection because we need args (like name). so we put those here.

    public string Name { get; set; }
}
