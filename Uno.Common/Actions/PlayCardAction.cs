using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Uno.Actions;
public class PlayCardAction : PlayerAction
{
    public Card Card;

    public class Response : PlayerAction
    {
        public bool Ok { get; set; }
    }
}
