using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Uno;

// card face + state
// made a record for by-value equality
public record Card
{
    public int ID { get; set; }
    public CardFace Face { get; set; }

    public Card(int id, CardColor color, CardKind kind)
    {
        this.ID = id;
        Face = new CardFace(kind, color);
    }

    public Card(int id, CardFace face)
    {
        this.ID = id;
        Face = face;
    }


    public Card(int id)
    {

    }

    public Card()
    {

    }
}
