using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Uno;

// represents one card in a deck
public record struct CardFace(CardKind Kind, CardColor Color)
{
    public static readonly CardFace Backface = new(CardKind.One, CardColor.Neutral); // any non-wild neutral is drawn as a backface

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

        int kindValue = random.Next(0, (13 * 4) + 2); // first 13 kinds should have 4x the weight of the wild cards
        CardKind kind;

        if (kindValue < 13 * 4)
        {
            kind = (CardKind)(kindValue / 4);
        }
        else
        {
            kind = (CardKind)(kindValue - (13 * 3));
        }

        CardColor color = kind is CardKind.Wild or CardKind.WildDraw4 ? CardColor.Neutral : (CardColor)random.Next(0, 4);

        return new(kind, color);
    }
}