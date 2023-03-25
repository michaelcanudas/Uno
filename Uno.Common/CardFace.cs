using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Uno;

// represents one card in a deck
public record struct CardFace(CardKind Kind, CardColor Color)
{
    // can this card be played on top of other?
    public bool CanBePlayedOn(CardFace other)
    {
        // wilds can always be played
        if (this.Kind is CardKind.Wild or CardKind.WildDraw4)
            return true;

        // have to account for game rules here..
        // like stacking +2s and +4s

        return this.Color == other.Color || this.Kind == other.Kind;
    }

    public static CardFace Random(Random? random = null)
    {
        // TODO: wildcards

        random ??= System.Random.Shared;

        CardColor color = (CardColor)random.Next(0, 4);
        CardKind kind = (CardKind)random.Next(0, 13);

        return new(kind, color);
    }
}
