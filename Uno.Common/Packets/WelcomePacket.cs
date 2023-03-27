using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Uno.Packets;

/// <summary>
/// Welcomes a client to the lobby by letting know which players are there.
/// </summary>
public class WelcomePacket : Packet
{
    public WelcomePacket()
    {

    }

    public WelcomePacket(bool success, string[] players, int spectators)
    {
        this.Success = success;
        this.Players = players;
    }

    public bool Success;
    public string[] Players;
    public int spectators;
}
