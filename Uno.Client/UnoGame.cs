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
using Uno.Server;

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

    string[] toasts =
    [
        "The Better Version.",
        "Remember Not To Take Breaks.",
        "I'm watching you.",
        "I'm In Your Walls.",
        "HELLO MICHEAL",
        "2042",
        "goto UNO",
        string.Join(' ', Enumerable.Repeat("UNO:", 69)),
    ];

    public override void OnInitialize()
    {
        CardRenderer = new();
        Current = this;
        activeScene.Begin();
        Window.Title = "UNO: " + toasts[new Random().Next(0, toasts.Length)];
        Application.Exiting += Application_Exiting;
    }

    public override void OnRender(ICanvas canvas)
    {
        if (Client.IsConnected)
            Client.Tick();

        if (Keyboard.IsKeyPressed(Key.F11))
        {
            if (Window.IsFullscreen)
            {
                Window.ExitFullscreen();
            }
            else
            {
                Window.EnterFullscreen();
            }
        }

        if (nextScene is not null)
        {
            activeScene.End();
            nextScene.Begin();

            activeScene = nextScene;
        }
        
        canvas.Clear(Color.FromHSV(0, 0, .1f));
        canvas.Antialias(true);

        Rectangle screenBounds = new Rectangle(0, 0, Window.Width, Window.Height);
        canvas.Fill(new LinearGradient(screenBounds.GetAlignedPoint(Alignment.TopCenter), screenBounds.GetAlignedPoint(Alignment.BottomCenter), Color.FromHSV(0, 0, .1f), Color.FromHSV(0, 0, .2f)));
        canvas.DrawRect(screenBounds);

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

        if (Client.Receive<StopPacket>(out _))
        {
            // also maybe we shouldnt just like banish them
            // to the main menu maybe be like "omg heyy.. sorry
            // about this but like u cant play xD "
            SwitchScenes(new MenuScene());
        }

        // this is actually kinda smort cause like when u
        // get disconnect from server u gotta be canceled
        if (Client.Receive<DisconnectPacket>(out _))
        {
            SwitchScenes(new MenuScene());
        }

    }

    private void Application_Exiting(ExitMessage message)
    {
        // sf 0.2.1 beta 5 bug: OnUninitialize is not called
        OnUninitialize();
    }

    public override void OnUninitialize()
    {
        if (ServerThread is not null)
        {
            Server.Server.Stop();
        }

        base.OnUninitialize();
    }
}
