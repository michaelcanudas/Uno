using ImGuiNET;
using SimulationFramework;
using SimulationFramework.Drawing;
using System;
using System.Buffers.Text;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace Uno.Client.Scenes;
internal class JoinServerScene : GameScene
{
    string hostname = "";
    string port = "";
    string? errmsg = null;

    List<Vector2> positions = new();

    public JoinServerScene()
    {
    }

    public override void Update()
    {
        ImGui.SetNextWindowPos(new(Graphics.GetOutputCanvas().Width / 2, Graphics.GetOutputCanvas().Height / 2), ImGuiCond.Always, new(.5f));
        if (ImGui.Begin("server_select_window", ImGuiWindowFlags.NoDecoration | ImGuiWindowFlags.AlwaysAutoResize))
        {
            ImGui.Text("WELCOME TO UNO");
            ImGui.Separator();

            ImGui.Text("host name");
            ImGui.SameLine();
            ImGui.TextDisabled("(?)");
            if (ImGui.IsItemHovered())
                ImGui.SetTooltip("host ip or url. leave blank for default (localhost).");
            ImGui.InputText("##hostname", ref hostname, 128);

            ImGui.Text("port");
            ImGui.SameLine();
            ImGui.TextDisabled("(?)");
            if (ImGui.IsItemHovered())
                ImGui.SetTooltip("port on host. leave blank for default (12345).");
            ImGui.InputText("##port", ref port, 5, ImGuiInputTextFlags.CharsDecimal);
            
            if (ImGui.Button("Load Last"))
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

    CardRenderer? renderer;
    int fallingCardSeed = Random.Shared.Next();

    public override void Render(ICanvas canvas)
    {
        renderer ??= new();

        int fallingCardCount = 2 * (int)( (canvas.Width * canvas.Height) / (CardRenderer.CardHeight * CardRenderer.CardHeight) );
        const int fallRateMin = 75;
        const int fallRateMax = 150;

        Random rng = new(fallingCardSeed);

        float w = canvas.Width + CardRenderer.CardWidth * 2;
        float h = canvas.Width + CardRenderer.CardWidth * 2;

        for (int i = 0; i < fallingCardCount; i++)
        {
            float x = rng.NextSingle() * w - CardRenderer.CardWidth; 
            float y = rng.NextSingle() * h;

            // animate!
            var fallRate = MathHelper.Lerp(fallRateMin, fallRateMax, rng.NextSingle());
            y += Time.TotalTime * fallRate;
            y %= h;
            y -= CardRenderer.CardHeight;

            renderer.DrawCard(canvas, CardFace.Random(rng), new Vector2(x, y));
        }

        base.Render(canvas);
    }

    private void TryJoinServer(string hostname, string port)
    {
        try
        {
            if (string.IsNullOrEmpty(hostname))
                hostname = "localhost";

            if (string.IsNullOrEmpty(port))
                port = "12345";
            
            if (!int.TryParse(port, out int value))
            {
                if (value < 0 || value > ushort.MaxValue)
                {
                    errmsg = "Invalid port number.";
                    return;
                }
            }

            // load client and initiate scene switch
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
