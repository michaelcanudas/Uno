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
    public Vector2 Position { get; set; } = Vector2.Zero;
    public float Rotation { get; set; } = 0f;
    public float Scale { get; set; } = 1f;

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
    public bool SelectionEnabled { get; set; }

    public InteractableCard? SelectedCard { get; private set; }

    public void Render(ICanvas canvas)
    {
        foreach (var card in Cards)
        {
            card.Render(canvas);
        }

        canvas.Stroke(Color.Purple);
        // canvas.DrawCircle(Position.X, Position.Y-offset+radius, radius);
    }

    public void Update()
    {
        float breadth = breadthUpper - (breadthUpper - breadthLower) / MathF.Pow(breadthStrength, (Cards.Count - 2));
        float increment = (breadth / Cards.Count);
        float baseAngle = (increment - breadth) / 2f - (MathF.PI / 2f);

        var mousePosition = Camera.Active.ScreenToWorld(Mouse.Position);
        this.SelectedCard = GetCard(mousePosition);
        var selectedCardIndex = (SelectedCard is null || (!SelectionEnabled)) ? -1 : Cards.IndexOf(SelectedCard);

        for (int i = 0; i < Cards.Count; i++)
        {
            var card = Cards[i];

            float angle = this.Rotation + baseAngle + i * increment;
            
            if (selectedCardIndex is not -1 && selectedCardIndex != (Cards.Count - 1))
            {
                float diff = (increment / -2f) + Scale * Angle.ToRadians(12.5f / radius);

                if (i <= selectedCardIndex)
                {
                    angle -= diff;
                }
                else
                {
                    angle += diff;
                }
            }

            card.TargetPosition = this.Position + Scale * (Vector2.UnitY * (radius - offset)  + Angle.ToVector(angle) * radius);
            card.TargetRotation = angle + (MathF.PI/2f);
            card.TargetScale = this.Scale;

            if (SelectionEnabled && card == SelectedCard)
            {
                card.TargetScale *= 1.20f;
                card.TargetPosition += Angle.ToVector(angle) * .15f * Scale;
            }


            card.Update();
        }

        if (SelectedCard is not null && SelectionEnabled && Mouse.IsButtonPressed(MouseButton.Left))
        {
            this.OnCardSelected?.Invoke(SelectedCard);
        }
    }

    public InteractableCard? GetCard(Vector2 worldSpacePoint)
    {
        // cards are stored left to right, and a card is always on top of the card to its left.
        // we can assume that if we test the cards in reverse order, the first success will be the topmost card.

        for (int i = Cards.Count - 1; i >= 0; i--)
        {
            if (Cards[i].ContainsPoint(worldSpacePoint) ||
                Cards[i].ContainsPoint(worldSpacePoint, Vector2.UnitY * -1f))
                return Cards[i];
        }

        return null;
    }
}