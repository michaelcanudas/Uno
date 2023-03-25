using ImGuiNET;
using SimulationFramework;
using SimulationFramework.Drawing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Uno.Client.Scenes.MainMenu;
internal class MainMenuWindow : MenuWindow
{
    string hostname = "";
    string port = "";

    public override string Title => "Main Menu";

    protected override void LayoutContent()
    {
        var s = "WELCOME TO UNO";

        for (int i = 0; i < s.Length; i++)
        {
            if (i is not 0)
                ImGui.SameLine();

            float sat = MathF.Sin(.25f * (-Time.TotalTime + i * .1f) * MathF.PI);
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
            (hostname, port) = MenuScene.TryLoadLastServerSettings();
        }

        ImGui.SameLine();
        if (ImGui.Button("Join"))
        {
            MenuScene.TryJoinServer(hostname, port);
        }

        if (MenuScene.errmsg is not null)
        {
            ImGui.Separator();
            ImGui.TextColored(Color.Red.AsVector4(), MenuScene.errmsg);
        }

        ImGui.SetNextWindowPos(new(Graphics.GetOutputCanvas().Width / 2f, Graphics.GetOutputCanvas().Height), ImGuiCond.Always, new(.5f, 1));
        if (ImGui.Begin("extra buttons", ImGuiWindowFlags.NoDecoration | ImGuiWindowFlags.AlwaysAutoResize))
        {
            if (ImGui.Button("Host Server"))
            {
                MenuScene.windows.Push(new HostServerWindow());
            }

            ImGui.SameLine();
            if (ImGui.Button("Settings"))
            {
                MenuScene.windows.Push(new SettingsWindow());
            }
        }
        ImGui.End();
    }
}
