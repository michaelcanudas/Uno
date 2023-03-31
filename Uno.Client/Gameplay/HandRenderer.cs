using ImGuiNET;
using SimulationFramework.Drawing;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Numerics;
using System.Runtime.ConstrainedExecution;
using System.Text;
using System.Threading.Tasks;

namespace Uno.Client.Gameplay;
internal class PlayerHand
{
    public List<InteractableCard> Cards { get; set; }
    public Vector2 Position { get; set; }

    public PlayerHand(IEnumerable<InteractableCard> cards)
    {
        this.Cards = new(cards);
    }

    float offset = .4f;
    float radius = 2f;
    float breadthStrength = 1.05f;
    float breadthUpper = Angle.ToRadians(90f);
    float breadthLower = Angle.ToRadians(10f);

    public event Action<InteractableCard>? OnCardSelected;

    public void Render(ICanvas canvas, bool frontFacing)
    {
        //UnoGame.Current.CardRenderer.DrawCard(canvas, selectedCard, -2 * Vector2.UnitY, 1, Alignment.Center);

        //Matrix3x2.Invert(canvas.State.Transform, out Matrix3x2 camMatrix);
        //Vector2 mousePos = Vector2.Transform(Mouse.Position, camMatrix);

        //Vector2 dir = mousePos - new Vector2(0, radius - offset);
        //float mouseAngle = MathF.Atan2(dir.Y, dir.X);
        //mouseAngle = MathHelper.NormalizeRadians(mouseAngle);
        //float mouseDist = dir.Length();

        //canvas.Stroke(Color.Purple);

        //float maxAngle = baseAngle + breadth;
        //mouseAngle += MathHelper.DegreesToRadians(5f);

        //float selectionFac = (MathHelper.NormalizeRadians(mouseAngle) - MathHelper.NormalizeRadians(baseAngle - MathF.PI / 2)) / breadth;
        //selection = MathHelper.Lerp(0, Cards.Count - 1, selectionFac);

        //var selectFalloff = Sigmoid(-50 * (mouseDist - (3.65f / 5f)));

        //for (int i = 0; i < Cards.Count; i++)
        //{
        //    var card = Cards[i];

        //    canvas.PushState();

        //    // x is distance to selected card pos (fractional)
        //    float x = i - selection;

        //    // calculate upwards bump
        //    float bump = 1f / (4 * x * x + 1);

        //    canvas.Translate(0, radius - offset);
        //    canvas.Rotate(baseAngle + i * increment);

        //    // calcuate offset to make room for selected card
        //    // just a sigmoid remapped to [-1, 1]
        //    x = Sigmoid(10 * (x - .5f)) * 2 - 1;

        //    canvas.Rotate(x * selectStrength * selectFalloff);
        //    canvas.Translate(0, -(radius + bump * .02f));

        //    UnoGame.Current.CardRenderer.DrawCard(canvas, card.Face, Vector2.Zero, 1, Alignment.BottomCenter);

        //    canvas.PopState();
        //}

        foreach (var card in Cards)
        {
            card.Render(canvas);
        }

        canvas.Stroke(Color.Purple);
        canvas.DrawCircle(Position.X, Position.Y-offset+radius, radius);
    }

    public void Update()
    {
        float breadth = breadthUpper - (breadthUpper - breadthLower) / MathF.Pow(breadthStrength, (Cards.Count - 2));
        float increment = breadth / Cards.Count;
        float baseAngle = (increment - breadth) / 2f - (MathF.PI / 2f);

        var mousePosition = Camera.Active.ScreenToWorld(Mouse.Position);
        var selectedCard = GetCard(mousePosition);
        var selectedCardIndex = selectedCard is null ? -1 : Cards.IndexOf(selectedCard);

        for (int i = 0; i < Cards.Count; i++)
        {
            var card = Cards[i];

            float angle = baseAngle + i * increment;
            
            if (selectedCardIndex is not -1 && selectedCardIndex != (Cards.Count - 1))
            {
                float diff = (increment / -2f) + Angle.ToRadians(12.5f / radius);

                if (i <= selectedCardIndex)
                {
                    angle -= diff;
                }
                else
                {
                    angle += diff;
                }
            }

            card.TargetPosition = Position + Vector2.UnitY * (radius - offset) + Angle.ToVector(angle) * radius;
            card.TargetRotation = angle + (MathF.PI/2f);

            if (card == selectedCard)
            {
                card.TargetPosition += Angle.ToVector(angle) * .15f;
            }

            card.Update();
        }

        if (selectedCard is not null && Mouse.IsButtonPressed(MouseButton.Left))
        {
            this.OnCardSelected?.Invoke(selectedCard);
        }
    }

    public InteractableCard? GetCard(Vector2 worldSpacePoint)
    {
        // cards are stored left to right, and a card is always on top of the card to its left.
        // we can assume that if we test the cards in reverse order, the first success will be the topmost card.

        for (int i = Cards.Count - 1; i >= 0; i--)
        {
            if (Cards[i].ContainsPoint(worldSpacePoint) ||
                Cards[i].ContainsPoint(worldSpacePoint, Vector2.UnitY * -.5f))
                return Cards[i];
        }

        return null;
    }
}