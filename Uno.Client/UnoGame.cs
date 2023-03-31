using SimulationFramework;
using SimulationFramework.Drawing;
using SimulationFramework.Messaging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Telepathy;
using Uno.Client;
using Uno.Client.MainMenu;
using Uno.Packets;

namespace Uno.Client;
internal class UnoGame : Simulation
{
    public static UnoGame Current { get; private set; }

    public Thread? ServerThread { get; set; }

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
                // when the window is maximized it gets screwed up
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
        if (Client.IsConnected)
            Client.Tick();

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

        // help im sorry i added this here i promise
        // i didnt know this would cause the dark realm
        // to open please forgive me

        // in all seriousness move this whenever cause
        // like idk where u planned on putting this stuff
        //foreach (var packet in Client.Receive<StartPacket>())
        //{
        //    SwitchScenes(new RendererDemoScene());
        //}

        // i commented it out :)
        // btw, the proper place to handle this is in LobbyMenu
        // each menu/scene does the transition the next one itself

        foreach (var packet in Client.Receive<StopPacket>())
        {
            // also maybe we shouldnt just like banish them
            // to the main menu maybe be like "omg heyy.. sorry
            // about this but like u cant play xD "
            SwitchScenes(new MainMenuScene());
        }

        // this is actually kinda smort cause like when u
        // get disconnect from server u gotta be canceled
        foreach (var packet in Client.Receive<DisconnectPacket>())
        {
            SwitchScenes(new MainMenuScene());
        }
    }
}
