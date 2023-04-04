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
        public Card[] Cards { get; set; }

        public Response()
        {

        }

        public Response(Card[] cards)
        {
            Cards = cards;
        }

        public Response(Card card) : this(new[] { card })
        {
        }
    }
}
