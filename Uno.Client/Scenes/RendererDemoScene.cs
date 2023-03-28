using SimulationFramework;
using SimulationFramework.Drawing;
using Uno.Client.Gameplay;
using Uno.Client.Scenes.MainMenu;

namespace Uno.Client.Scenes;

// i use this for testing renderers
internal class RendererDemoScene : GameScene
{
    FlyingCamera flyingCamera;
    Camera fixedCamera;
    PlayerHand handRenderer;
    bool flycam;

    public override void Begin()
    {
        flyingCamera = new(1);
        fixedCamera = new(1);

        handRenderer = new();
        handRenderer.Cards.AddRange(Enumerable.Repeat(CardFace.Backface, 7));

        base.Begin();
    }

    public override void Render(ICanvas canvas)
    {
        ImGui.Checkbox("flycam", ref flycam);

        Camera camera = flycam ? flyingCamera : fixedCamera;

        camera.Update();
        camera.ApplyTo(canvas);

        canvas.Translate(0,.5f);
        handRenderer.Render(canvas, true);

        base.Render(canvas);
    }


    public override void Update()
    {
        base.Update();
    }
}