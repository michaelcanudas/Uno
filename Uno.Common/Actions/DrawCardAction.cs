using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Uno.Actions;
public class DrawCardAction : PlayerAction
{
    public class Response : PlayerAction
    {
        public Card Card { get; set; }

        public Response()
        {

        }

        public Response(Card card)
        {
            Card = card;
        }
    }
}
