using SimulationFramework;
using SimulationFramework.Drawing;
using SimulationFramework.Messaging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Uno.Client;
internal class UnoGame : Simulation
{
    public static UnoGame Current { get; private set; }

    GameScene activeScene;
    GameScene? nextScene;

    public GameScene ActiveScene => activeScene;

    public CardRenderer CardRenderer { get; private set; }

    public UnoGame(GameScene initialScene)
    {
        activeScene = initialScene;
    }

    public void SwitchScenes(GameScene nextScene)
    {
        this.nextScene = nextScene;
    }

    string[] toasts = new[]
    {
        "The Better Version.",
        "Remember Not To Take Breaks.",
        "I'm watching you.",
        "I'm In Your Walls.",
        "HELLO MICHEAL",
        "2042",
        "goto UNO",
        string.Join(' ', Enumerable.Repeat("UNO:", 69)),
    };

    // hacky workarounds because i can't seem to write functional fullscreen code :)
    bool shouldToggleFullscreen;
    bool isFullscreen = false;
    int prevW, prevH;

    public override void OnInitialize(AppConfig config)
    {
        CardRenderer = new();
        Current = this;
        activeScene.Begin();
        config.Title = "UNO: " + toasts[new Random().Next(0, toasts.Length)];

        // can't do this on render because it corrupts the canvas
        Application!.Dispatcher.Subscribe<FrameBeginMessage>(m =>
        {
            if (shouldToggleFullscreen)
            {
                // the the window is maximized the gets screwed up
                // i am going to rewrite sf's window code soon

                var config = AppConfig.Create();
                isFullscreen = !isFullscreen;
                config.Fullscreen = isFullscreen;
                config.TitlebarHidden = isFullscreen;
                config.Resizable = !isFullscreen;
                
                // if we are entering fullscreen save previous size
                if (isFullscreen)
                {
                    prevW = config.Width;
                    prevH = config.Height;
                }
                else // otherwise restore it
                {
                    config.Width = prevW;
                    config.Height = prevH;
                }

                config.Apply();
            }
        });
    }

    public override void OnRender(ICanvas canvas)
    {
        shouldToggleFullscreen = Keyboard.IsKeyPressed(Key.F11);
        if (nextScene is not null)
        {
            activeScene.End();
            nextScene.Begin();

            activeScene = nextScene;
        }
        
        canvas.Clear(Color.FromHSV(0, 0, .1f));

        activeScene.Update();
        activeScene.Render(canvas);

    }
}
