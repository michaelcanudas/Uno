﻿using SimulationFramework;
using SimulationFramework.Drawing;
using Uno.Client.Gameplay;
using Uno.Client.Scenes.MainMenu;

namespace Uno.Client.Scenes;

// i use this for testing renderers
internal class RendererDemoScene : GameScene
{
    FlyingCamera flyingCamera;
    Camera fixedCamera;
    PlayerHand hand;
    bool flycam;
    InteractableCardStack depotStack;
    InteractableCardStack drawStack;

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

        drawStack.Cards.Push(new InteractableCard(CardFace.Random(new(0))) 
        { 
            IsFaceDown = true,
        });

        drawStack.Clicked += () =>
        {
            var card = drawStack.Cards.Pop();
            card.IsFaceDown = false;
            hand.Cards.Add(card);
            
            if (!drawStack.Cards.Any())
            {
                for (int i = 0; i < 4; i++)
                {
                    drawStack.Cards.Push(new InteractableCard(CardFace.Random(new()))
                    {
                        IsFaceDown = true,
                        Position = drawStack.Position,
                        TargetPosition = drawStack.Position
                    });
                }
            }
        };

        hand.OnCardSelected += card => 
        {
            this.hand.Cards.Remove(card);
            this.depotStack.Cards.Push(card); 
        };


        base.Begin();
    }

    public override void Render(ICanvas canvas)
    {
        ImGui.Checkbox("flycam", ref flycam);

        Camera camera = flycam ? flyingCamera : fixedCamera;

        camera.ApplyTo(canvas);

        hand.Render(canvas, true);
        depotStack.Render(canvas);
        drawStack.Render(canvas);

        base.Render(canvas);
    }

    public override void Update()
    {
        Camera camera = flycam ? flyingCamera : fixedCamera;
        camera.Update();
        camera.SetActive();

        hand.Update();
        depotStack.Update();
        drawStack.Update();

        base.Update();
    }
}