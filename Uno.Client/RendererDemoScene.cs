using SimulationFramework;
using SimulationFramework.Drawing;
using Uno.Client.Gameplay;
using Uno.Client.MainMenu;

namespace Uno.Client;

// i use this for testing renderers
internal class RendererDemoScene : GameScene
{
    FlyingCamera flyingCamera;
    Camera fixedCamera;
    PlayerHand hand;
    bool flycam;
    InteractableCardStack depotStack;
    InteractableCardStack drawStack;
    ActionsBar window;
    public override void Begin()
    {
        flyingCamera = new(4);
        fixedCamera = new(4);

        hand = new(Enumerable.Empty<InteractableCard>())
        {
            Position = new(0, 2)
        };

        depotStack = new();
        drawStack = new()
        {
            Position = new(-1, 0)
        };

        //drawStack.Cards.Push(new InteractableCard(CardFace.Random(new(0)))
        //{
        //    IsFaceDown = true,
        //});

        //drawStack.Clicked += () =>
        //{
        //    var card = drawStack.Cards.Pop();
        //    card.IsFaceDown = false;
        //    hand.Cards.Add(card);

        //    if (!drawStack.Cards.Any())
        //    {
        //        for (int i = 0; i < 4; i++)
        //        {
        //            drawStack.Cards.Push(new InteractableCard(new(0, CardFace.Random(new())))
        //            {
        //                IsFaceDown = true,
        //                Position = drawStack.Position,
        //                TargetPosition = drawStack.Position
        //            });
        //        }
        //    }
        //};

        hand.OnCardSelected += card =>
        {
            hand.Cards.Remove(card);
            depotStack.Cards.Push(card);
        };

        window = new();

        base.Begin();
    }

    public override void Render(ICanvas canvas)
    {
        ImGui.Checkbox("flycam", ref flycam);

        Camera camera = flycam ? flyingCamera : fixedCamera;

        camera.ApplyTo(canvas);

        hand.Render(canvas);
        depotStack.Render(canvas);
        drawStack.Render(canvas);

        base.Render(canvas);
    }

    public override void Update()
    {
        window.Layout();
        Camera camera = flycam ? flyingCamera : fixedCamera;
        camera.Update();
        camera.SetActive();

        hand.Update();
        depotStack.Update();
        drawStack.Update();

        base.Update();
    }
}