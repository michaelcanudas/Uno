using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Uno.Server;
internal class Player
{
    public int Connection { get; }
    public string Name { get; }

    public Player(int connection, string name)
    {
        this.Connection = connection;
        this.Name = name;
    }
}
