using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Uno.Packets;
internal class NotificationPacket : Packet
{
    public string? Message { get; set; }
}
