using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Uno.Packets;
public class SpectatorCountPacket : Packet
{
    public SpectatorCountPacket()
    {

    }

    public SpectatorCountPacket(int count)
    {
        this.SpectatorCount = count;
    }

    public int SpectatorCount { get; set; }
}
