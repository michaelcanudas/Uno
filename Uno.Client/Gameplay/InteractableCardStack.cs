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

    public Vector2 Position { get; set; }

    public event Action? Clicked;
    public bool IsClicked { get; private set; }

    public void Render(ICanvas canvas)
    {
        if (!Cards.Any())
            return;

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
            card.TargetRotation = 0f;
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