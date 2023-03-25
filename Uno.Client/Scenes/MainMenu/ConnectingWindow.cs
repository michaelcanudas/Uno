using ImGuiNET;
using SimulationFramework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Uno.Client.Scenes.MainMenu;
internal class ConnectingWindow : MenuWindow
{
    private string address;

    public override string Title => address;

    public override ImGuiWindowFlags WindowFlags => ImGuiWindowFlags.NoCollapse | ImGuiWindowFlags.NoScrollbar | ImGuiWindowFlags.NoResize;

    public ConnectingWindow(string address)
    {
        this.address = address;
    }

    public override void Layout()
    {
        // this should load some lobby scene

        ImGui.SetNextWindowSize(new(125, 0));
        base.Layout();
    }

    protected override void LayoutContent()
    {
        var m = "Connecting...";
        ImGui.Text(m[..(int)(10 + Time.TotalTime * 2f % 4f)]);
        if (ImGui.Button("Cancel"))
        {
            // this.Cancel() ?
            MenuScene.windows.Pop();
        }
    }
}
