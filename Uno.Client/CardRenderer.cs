using SimulationFramework;
using SimulationFramework.Drawing;
using System;
using System.Buffers.Text;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace Uno.Client;
internal class CardRenderer
{
    private ITexture cardSpriteSheet;
    private Dictionary<CardFace, Rectangle> cardBounds = new();

    public const float CardWidth = 70;
    public const float CardHeight = 106;

    public CardRenderer()
    {
        cardSpriteSheet = Graphics.LoadTexture("Assets/cards.png");

        for (int i = 0; i < 13; i++)
        {
            var kind = (CardKind)i;

            for (int j = 0; j < 4; j++)
            {
                var color = (CardColor)j;
                CardFace card = new(kind, color);

                cardBounds.Add(card, GetCardBounds(card));
            }
        }
    }

    public Rectangle GetCardBounds(CardFace card)
    {
        const float baseX = 414;
        const float baseY = 123;

        const float cardSeparationX = 3;
        const float cardSeparationY = 10;

        return new Rectangle(baseX + (int)card.Kind * (CardWidth + cardSeparationX), baseY + (int)card.Color * (CardHeight + cardSeparationY), CardWidth, CardHeight);
    }

    public void DrawCard(ICanvas canvas, CardFace face, Rectangle destination)
    {
        canvas.DrawTexture(cardSpriteSheet, cardBounds[face], destination);
    }

    public void DrawCard(ICanvas canvas, CardFace face, Vector2 position, Alignment alignment = Alignment.TopLeft)
    {
        canvas.DrawTexture(cardSpriteSheet, cardBounds[face], new(position, new(CardWidth, CardHeight), alignment));
    }
}
