using ImGuiNET;
using SimulationFramework;
using SimulationFramework.Drawing;
using System;
using System.Buffers.Text;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace Uno.Client.Scenes;
internal class MainMenuScene : GameScene
{
    string hostname = "";
    string port = "";
    string? errmsg = null;

    List<Vector2> positions = new();

    public MainMenuScene()
    {
    }

    public override void Update()
    {
        ImGui.SetNextWindowPos(new(Graphics.GetOutputCanvas().Width / 2, Graphics.GetOutputCanvas().Height / 2), ImGuiCond.Always, new(.5f));
        if (ImGui.Begin("server_select_window", ImGuiWindowFlags.NoDecoration | ImGuiWindowFlags.AlwaysAutoResize))
        {
            var s = "WELCOME TO UNO";

            for (int i = 0; i < s.Length; i++)
            {
                if (i is not 0)
                    ImGui.SameLine();

                float sat = MathF.Sin(Time.TotalTime);
                var color = Color.FromHSV(sat * sat, 1, 1);

                ImGui.TextColored(color.AsVector4(), s[i].ToString());
            }

            ImGui.Separator();

            ImGui.Text("host name");
            ImGui.SameLine();
            ImGui.TextDisabled("(?)");

            if (ImGui.IsItemHovered())
                ImGui.SetTooltip("host ip or url.");

            ImGui.InputText("##hostname", ref hostname, 128);

            ImGui.Text("port");
            ImGui.SameLine();
            ImGui.TextDisabled("(?)");
            if (ImGui.IsItemHovered())
                ImGui.SetTooltip("port on host. leave blank for default (12345).");

            ImGui.InputText("##port", ref port, 5, ImGuiInputTextFlags.CharsDecimal);
            
            if (ImGui.Button("Last"))
            {
                TryLoadLastServerSettings();
            }

            ImGui.SameLine();
            if (ImGui.Button("Join"))
            {
                TryJoinServer(hostname, port);
            }

            if (errmsg is not null)
            {
                ImGui.Separator();
                ImGui.TextColored(new(1, 0, 0, 1), errmsg);
            }

        }
        ImGui.End();


        base.Update();
    }

    int fallingCardSeed = Random.Shared.Next();

    public override void Render(ICanvas canvas)
    {
        int fallingCardCount = (int)( (canvas.Width * canvas.Height) / (CardRenderer.CardHeight * CardRenderer.CardHeight) );
        const int fallRateMin = 75;
        const int fallRateMax = 150;

        Random rng = new(fallingCardSeed);

        float w = canvas.Width + CardRenderer.CardWidth * 2;
        float h = canvas.Height + CardRenderer.CardHeight * 2;

        for (int i = 0; i < fallingCardCount; i++)
        {
            float x = rng.NextSingle() * w - CardRenderer.CardWidth; 
            float y = rng.NextSingle() * h;

            // animate!
            var fallRate = MathHelper.Lerp(fallRateMin, fallRateMax, rng.NextSingle());
            y += Time.TotalTime * fallRate;
            y %= h;
            y -= CardRenderer.CardHeight;

            UnoGame.Current.CardRenderer.DrawCard(canvas, CardFace.Random(rng), new Vector2(x, y));
        }

        base.Render(canvas);
    }

    private void TryJoinServer(string hostname, string port)
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

            // connect client
            // save settings for next time
            // create game scene
            // initiate scene switch

            // IDEA: REFERENCE SERVER PROJECT AND ADD A 'HOST' BUTTON TO DIS MENU
            
            // iterators might be cool for the actual gameplay logic

            File.WriteAllLines("last_server.txt", new[] { hostname, port });

            var scene = new GameplayScene(null!); // need to pass client here
            UnoGame.Current.SwitchScenes(scene);

            errmsg = null;
        }
        catch (Exception ex)
        {
            errmsg = ex.Message;
        }
    }

    private void TryLoadLastServerSettings()
    {
        try
        {
            string[] lines = File.ReadAllLines("last_server.txt");
            hostname = lines[0];
            port = lines[1];
            errmsg = null;
        }
        catch
        {
            errmsg = "No data to load";
        }
    }
}
