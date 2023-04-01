using SimulationFramework.Drawing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Uno.Client.Gameplay;
internal class InteractableCard
{
    public CardFace Face { get => Card.Face; }

    public Card Card { get; set; }

    public Vector2 TargetPosition { get; set; } = Vector2.Zero;
    public float TargetRotation { get; set; } = 0f;
    public float TargetScale { get; set; } = 1;

    public Vector2 Position { get; set; } = Vector2.Zero;
    public float Rotation { get; set; } = 0f;
    public float Scale { get; set; } = 1;

    public float Smoothing { get; set; } = 10;

    public bool IsFaceDown { get; set; } = false;

    public InteractableCard(Card card)
    {
        Card = card;
    }

    public void Update()
    {
        Position = Vector2.Lerp(Position, TargetPosition, 1f - MathF.Exp(-Smoothing * Time.DeltaTime));
        Rotation = Angle.Lerp(Rotation, TargetRotation, 1f - MathF.Exp(-Smoothing * Time.DeltaTime));
        Scale = MathHelper.Lerp(Scale, TargetScale, 1f - MathF.Exp(-Smoothing * Time.DeltaTime));
    }

    public bool ContainsPoint(Vector2 point, Vector2 localOffset = default)
    {
        var localBounds = new Rectangle(Vector2.Zero, new(CardRenderer.CardAspectRatio, 1), Alignment.BottomCenter);

        var matrix = 
            Matrix3x2.CreateTranslation(-Position) * 
            Matrix3x2.CreateRotation(-Rotation) *
            Matrix3x2.CreateScale(Scale);

        var pointLocalSpace = Vector2.Transform(point, matrix);

        return localBounds.ContainsPoint(pointLocalSpace + localOffset);
    }

    public void Render(ICanvas canvas)
    {
        canvas.PushState();
        canvas.Translate(Position);
        canvas.Rotate(Rotation);
        canvas.Scale(Scale);

        UnoGame.Current.CardRenderer.DrawCard(canvas, IsFaceDown ? CardFace.Backface : Face, Vector2.Zero, 1, Alignment.BottomCenter);

        canvas.PopState();
    }
}
