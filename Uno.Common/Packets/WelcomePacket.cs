using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Uno.Packets;

/// <summary>
/// Welcomes a client to the lobby.
/// </summary>
public class WelcomePacket : Packet
{
    public WelcomePacket()
    {

    }

    public WelcomePacket(bool success, bool elevated, string[] players, int spectators)
    {
        this.Success = success;
        this.Elevated = elevated;
        this.Players = players;
        this.spectators = spectators;
    }

    /// <summary>
    /// Whether the player successfully entered the lobby. If this is false, <see cref="Players"/> will be empty and <see cref="spectators"/> will be zero.
    /// </summary>
    public bool Success;

    /// <summary>
    /// Whether the player has elevated permissions (kick players, start the game). usually the first one in the lobby (we could add a configurable whitelist).
    /// </summary>
    public bool Elevated;

    /// <summary>
    /// The players currently in the lobby, including the receiving client. Will be empty if Sucess is <see langword="false"/>.
    /// </summary>
    public string[] Players;
    /// <summary>
    /// The number of spectators currently in the lobby, including the receiving client. Will be zero if <see cref="Success"/> is <see langword="false"/>.
    /// </summary>
    public int spectators;
}
