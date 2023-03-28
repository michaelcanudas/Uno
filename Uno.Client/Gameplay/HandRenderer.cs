using ImGuiNET;
using SimulationFramework.Drawing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace Uno.Client.Gameplay;
internal class PlayerHand
{
    public List<CardFace> Cards { get; set; }

    public PlayerHand()
    {
        this.Cards = new();
    }

    float offset = .025f;
    float radius = .125f;
    float breadthStrength = 1.05f;
    float breadthUpper = MathHelper.DegreesToRadians(90f);
    float breadthLower = MathHelper.DegreesToRadians(10f);
    int cards = 7;
    float selection = 4f;
    float selectStrength = 0.07f;
    public CardFace selectedCard;

    public void Render(ICanvas canvas, bool frontFacing)
    {
        Cards.Clear();
        Cards.AddRange(Enumerable.Range(0, cards).Select(i => CardFace.Random(new Random(i))));
        canvas.Scale(5f);
        UnoGame.Current.CardRenderer.DrawCard(canvas, selectedCard, -.125f * Vector2.UnitY, 0.05f, Alignment.Center);

        Matrix3x2.Invert(canvas.State.Transform, out Matrix3x2 camMatrix);
        Vector2 mousePos = Vector2.Transform(Mouse.Position, camMatrix);

        Vector2 dir = mousePos - new Vector2(0, radius - offset);
        float mouseAngle = MathF.Atan2(dir.Y, dir.X);
        mouseAngle = MathHelper.NormalizeRadians(mouseAngle);
        float mouseDist = dir.Length();
        var selectFalloff = Sigmoid(-100 * (mouseDist - .2f));

        ImGui.Text(selectFalloff.ToString());
        canvas.Stroke(Color.Purple);

        ImGui.DragInt("cards", ref cards, 1, 0, int.MaxValue);
        ImGui.DragFloat("selected", ref selection, 1);
        ImGui.DragFloat("select str", ref selectStrength, 0.01f);

        float breadth = breadthUpper - (breadthUpper - breadthLower) / MathF.Pow(breadthStrength, (cards-2));
        float increment = breadth / Cards.Count;
        float baseAngle = (increment-breadth) / 2f;
        mouseAngle += MathHelper.DegreesToRadians(5f);

        selection = MathHelper.Lerp(0, Cards.Count - 1, (MathHelper.NormalizeRadians(mouseAngle) - MathHelper.NormalizeRadians(baseAngle-MathF.PI/2)) / breadth);

        for (int i = 0; i < Cards.Count; i++)
        {
            var card = Cards[i];
            
            canvas.PushState();

            // x is distance to selected card pos (fractional)
            float x = i - selection;

            // calculate upwards bump
            float bump = 1f / (4 * x * x + 1);

            canvas.Translate(0, radius - offset);
            canvas.Rotate(baseAngle + i * increment);

            // calcuate offset to make room for selected card
            // just a sigmoid remapped to [-1, 1]
            x = Sigmoid(10 * (x-.5f)) * 2 - 1;

            canvas.Rotate(x * selectStrength * selectFalloff);
            canvas.Translate(0, -(radius + bump * .005f));

            UnoGame.Current.CardRenderer.DrawCard(canvas, card, Vector2.Zero, .05f, Alignment.BottomCenter);

            Matrix3x2.Invert(canvas.State.Transform, out var mat);
            Rectangle cardRect = new(Vector2.Zero, new Vector2(.05f * CardRenderer.CardAspectRatio, .05f), Alignment.BottomCenter);
            Vector2 mp = Vector2.Transform(Mouse.Position, mat);
            if (cardRect.ContainsPoint(mp))
            {
                canvas.Stroke(Color.White);
                canvas.StrokeWidth(.001f);
                canvas.DrawRect(cardRect);

                if (Mouse.IsButtonDown(MouseButton.Left))
                {
                    selectedCard = card;
                }
            }

            canvas.PopState();
        }
    }

    static float Sigmoid(float x)
    {
        return 1f / (1f + MathF.Pow(MathF.E, -x)); 
    }

    public void Update()
    {

    }

    private void DoCollisions()
    {
        for (int i = Cards.Count; i >= 0; i--)
        {

        }
    }

    // creates a world space -> card space matrix for the card at a given index
    private Matrix3x2 GetCardMatrix(int card)
    {
        return Matrix3x2.Identity;
    }
}

static class CanvasEx
{
    public static void DrawVector(this ICanvas canvas, Vector2 vector, Vector2 position = default)
    {
        canvas.DrawLine(position, position + vector);
    }
}