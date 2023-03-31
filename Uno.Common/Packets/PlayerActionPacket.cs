using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Uno.Actions;

namespace Uno.Packets;
// sent to the server by the client to do some action, and then sent to by server to every other client to notify them of that action
public class PlayerActionPacket : Packet
{
    public string PlayerName { get; set; }
    public PlayerAction Action { get; set; }

    public PlayerActionPacket()
    {

    }

    public PlayerActionPacket(string playerName, PlayerAction action)
    {
        this.PlayerName = playerName;
        this.Action = action;
    }
}
