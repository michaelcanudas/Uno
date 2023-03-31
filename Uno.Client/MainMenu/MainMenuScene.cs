using ImGuiNET;
using SimulationFramework;
using SimulationFramework.Drawing;
using System;
using System.Buffers;
using System.Buffers.Text;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace Uno.Client.MainMenu;
internal class MainMenuScene : GameScene
{
    public string? errmsg = null;

    public Stack<MenuWindow> windows = new();

    public MainMenuScene()
    {
        windows.Push(new MainMenuWindow());
    }

    public override void Update()
    {
        windows.Peek().Layout();

        base.Update();
    }

    int fallingCardSeed = Random.Shared.Next();

    public override void Render(ICanvas canvas)
    {
        canvas.Antialias(true);

        int fallingCardCount = (int)(canvas.Width * canvas.Height / (CardRenderer.CardHeight * CardRenderer.CardHeight));
        const int fallRateMin = 75;
        const int fallRateMax = 150;
        const float sizeMax = 2f;
        const float sizeMin = .5f;

        Random rng = new(fallingCardSeed);

        float w = canvas.Width + CardRenderer.CardWidth * 2;
        float h = canvas.Height + CardRenderer.CardHeight * 2;

        for (int i = 0; i < fallingCardCount; i++)
        {
            float x = rng.NextSingle() * w - CardRenderer.CardWidth;
            float y = rng.NextSingle() * h;

            // card size is just based on index being that we need render cards in size order
            float sizeT = i / (float)fallingCardCount;
            sizeT *= sizeT * sizeT; // cube it so more cards are smaller

            // animate!
            var fallRate = MathHelper.Lerp(fallRateMin, fallRateMax, rng.NextSingle());
            fallRate *= 1f + sizeT;
            y += Time.TotalTime * fallRate;
            y %= h;
            y -= CardRenderer.CardHeight * sizeMax;

            var size = MathHelper.Lerp(sizeMin, sizeMax, sizeT);

            UnoGame.Current.CardRenderer.DrawCard(canvas, CardFace.Random(rng), new Rectangle(x, y, size * CardRenderer.CardWidth, size * CardRenderer.CardHeight));
        }

        base.Render(canvas);
    }

    public void TryJoinServer(string hostname, string port)
    {
        try
        {
            if (string.IsNullOrEmpty(hostname))
            {
                errmsg = "Invalid host name.";
                return;
            }

            if (string.IsNullOrEmpty(port))
                port = "12345";

            if (int.Parse(port) > ushort.MaxValue)
            {
                errmsg = "Invalid port.";
                return;
            }

            // save settings for next time
            // create game scene
            // initiate scene switch

            // IDEA: REFERENCE SERVER PROJECT AND ADD A 'HOST' BUTTON TO DIS MENU

            // iterators might be cool for the actual gameplay logic

            File.WriteAllLines("last_server.txt", new[] { hostname, port });

            // var scene = new GameplayScene(null!); // need to pass client here
            // UnoGame.Current.SwitchScenes(scene);

            // NEVERMIND
            // need to do it in the connecting window
            // and maybe we should load some lobby scene ? ie before game started, but you can still see other players
            windows.Push(new ConnectingWindow(hostname, int.Parse(port)));
        }
        catch (Exception ex)
        {
            errmsg = ex.Message;
        }
    }

    public (string hostname, string port) TryLoadLastServerSettings()
    {
        try
        {
            string[] lines = File.ReadAllLines("last_server.txt");
            var result = (lines[0], lines[1]);
            errmsg = null;
            return result;
        }
        catch
        {
            errmsg = "No data to load";
            return ("", "");
        }
    }
}
