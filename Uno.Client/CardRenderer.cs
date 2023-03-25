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
    private readonly Dictionary<CardFace, Rectangle> cardBounds = new();
    

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
        const float cardSeparationX = 3;
        const float cardSeparationY = 10;

        if (card.Kind is CardKind.Wild or CardKind.WildDraw4)
        {
            // draw wildcard
            
            const float wildBaseX = 413;
            const float wildBaseY = 587;

            int index = (int)card.Color;

            if (card.Kind is CardKind.WildDraw4)
            {
                index += 5;
            }

            return new Rectangle(wildBaseX + index * (cardSeparationX + CardWidth), wildBaseY, CardWidth, CardHeight);
        }

        if (card.Color is CardColor.Neutral)
        {
            // draw card backface

            return new(5, 21, CardWidth, CardHeight);
        }

        const float baseX = 414;
        const float baseY = 123;

        return new Rectangle(baseX + (int)card.Kind * (CardWidth + cardSeparationX), baseY + (int)card.Color * (CardHeight + cardSeparationY), CardWidth, CardHeight);
    }

    public void DrawCard(ICanvas canvas, CardFace card, Rectangle destination)
    {
        canvas.DrawTexture(cardSpriteSheet, GetCardBounds(card), destination);
    }

    public void DrawCard(ICanvas canvas, CardFace card, Vector2 position, Alignment alignment = Alignment.TopLeft)
    {
        canvas.DrawTexture(cardSpriteSheet, GetCardBounds(card), new(position, new(CardWidth, CardHeight), alignment));
    }
}
