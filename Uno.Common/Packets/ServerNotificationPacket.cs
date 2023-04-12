using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Uno.Packets;
public class ServerNotificationPacket : Packet
{
    public string Notification { get; set; }
}
