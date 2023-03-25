using ImGuiNET;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Uno.Client.Scenes.MainMenu;
internal class HostServerWindow : MenuWindow
{
    string port = "";
    bool stacking = true;

    public override string Title => "Host Server";

    protected override void LayoutContent()
    {
        ImGui.Text("Host");
        ImGui.Separator();

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
        }
    }
}
