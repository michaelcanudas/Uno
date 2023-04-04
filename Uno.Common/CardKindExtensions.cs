using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Uno;
public static class CardKindExtensions
{
    /// <summary>
    /// Returns true if the given card kind is any kind of wildcard.
    /// </summary>
    public static bool IsWild(this CardKind kind)
    {
        return kind is CardKind.Wild or CardKind.WildDraw4;
    }

    public static bool IsDraw(this CardKind kind, out int count)
    {
        count = kind switch
        {
            CardKind.Draw2 => 2,
            CardKind.WildDraw4 => 4,
            _ => 0,
        };

        return count is not 0;
    }
}
