using SimulationFramework.Drawing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Uno.Client.Gameplay;
internal class InteractableCardStack
{
    public Stack<InteractableCard> Cards { get; } = new();

    public InteractableCardStack()
    {
    }

    float heightPerCard = .1f;
    float xoffset = 0.01f, yoffset = 0.1f;

    public Vector2 Position { get; set; } = Vector2.Zero;
    public float Rotation { get; set; } = 0f;
    public float Scale { get; set; } = 1f;

    public event Action? Clicked;
    public bool IsClicked { get; private set; }
    public bool Opaque { get; set; }

    public void Render(ICanvas canvas)
    {
        if (Opaque)
        {
            canvas.PushState();
            canvas.Translate(this.Position);
            canvas.Rotate(this.Rotation);
            UnoGame.Current.CardRenderer.DrawCard(canvas, CardFace.Backface, Vector2.Zero, this.Scale, Alignment.BottomCenter);
            canvas.PopState();
        }

        foreach (var card in Cards.Reverse())
        {
            card.Render(canvas);
        }
    }

    public void Update()
    {
        foreach (var card in Cards)
        {
            card.TargetPosition = this.Position;
            card.TargetRotation = this.Rotation;
            card.TargetScale = this.Scale;
            card.Update();
        }

        var mousePosition = Camera.Active.ScreenToWorld(Mouse.Position);
        var bounds = new Rectangle(Position, new(CardRenderer.CardAspectRatio, 1), Alignment.BottomCenter);

        IsClicked = Mouse.IsButtonPressed(MouseButton.Left) && bounds.ContainsPoint(mousePosition);
        if (IsClicked)
        {
            Clicked?.Invoke();
        }
    }
}

// above class should really work based off something like:
interface ICardStack
{
    int Height { get; }

    CardFace Peek();
    CardFace Pop();
    void Push(CardFace face);
}