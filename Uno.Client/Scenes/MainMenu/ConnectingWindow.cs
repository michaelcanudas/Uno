using ImGuiNET;
using SimulationFramework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Uno.Packets;

namespace Uno.Client.Scenes.MainMenu;
internal class ConnectingWindow : MenuWindow
{
    private string address;
    private int port;

    public override string Title => address + ":" + port;

    public override ImGuiWindowFlags WindowFlags => ImGuiWindowFlags.NoCollapse | ImGuiWindowFlags.NoScrollbar | ImGuiWindowFlags.NoResize;

    public ConnectingWindow(string address, int port)
    {
        this.address = address;
        this.port = port;

        Connect();
    }

    private void Connect()
    {
        Client.StartAsync(address, port).ContinueWith((t) =>
        {
            if (t.IsFaulted)
            {
                MenuScene.windows.Pop();
                MenuScene.errmsg = "WHAT THE HEK... no connect :(";
            }
            else
            {
                Client.SendAsync(new TextPacket("Hello from the client!"));
                UnoGame.Current.SwitchScenes(new GameplayScene());
            }
        });
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
