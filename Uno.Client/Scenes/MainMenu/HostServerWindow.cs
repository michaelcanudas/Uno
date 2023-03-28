using ImGuiNET;
using SimulationFramework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Uno.Server;

namespace Uno.Client.Scenes.MainMenu;
internal class HostServerWindow : MenuWindow
{
    string port = "";
    bool stacking = true;
    string? error;

    public override string Title => "Host Server";

    public HostServerWindow()
    {
        if (UnoGame.Current.ServerThread is not null)
        {
            port = Server.Server.Port.ToString();
        }
    }

    protected override void LayoutContent()
    {
        ImGui.Text("Host");
        ImGui.Separator();

        var game = UnoGame.Current;

        if (game.ServerThread is null)
        {
            ImGui.Text("port");
            ImGui.InputText("##port", ref port, 5, ImGuiInputTextFlags.CharsDecimal);

            // example gamerule
            ImGui.Checkbox("Allow stacking +2 with +4", ref stacking);

            ImGui.Separator();
            if (ImGui.Button("Back"))
            {
                MenuScene.windows.Pop();
            }
            ImGui.SameLine();
            if (ImGui.Button("Start Server"))
            {
                StartServer();
            }

            if (error is not null)
            {
                ImGui.TextColored(Color.Red.AsVector4(), error);
            }
        }
        else
        {
            ImGui.Text($"Server is currently running on port {port}.");

            ImGui.Separator();

            if (ImGui.Button("Back"))
            {
                MenuScene!.windows.Pop();
            }
            ImGui.SameLine();
            if (ImGui.Button("Stop Hosting"))
            {
                Server.Server.Stop();
                UnoGame.Current.ServerThread = null;
            }
            ImGui.SameLine();
            if (ImGui.Button("Join Server"))
            {
                MenuScene!.windows.Push(new ConnectingWindow("localhost", int.Parse(port)));
            }
        }
    }

    private void StartServer()
    {
        if (string.IsNullOrEmpty(port))
            port = "12345";

        if (int.Parse(port) > ushort.MaxValue)
        {
            error = "Invalid port number";
            return;
        }

        var game = UnoGame.Current;
        game.ServerThread = new Thread(() => Server.Program.Main(new[] { port }));
        game.ServerThread.Start();
    }
}
