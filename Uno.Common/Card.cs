using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Uno;

// card face + state
public class Card
{
    public CardFace Face { get; set; }

    public Card(CardColor color, CardKind kind)
    {
        Face = new CardFace(kind, color);
    }
}
