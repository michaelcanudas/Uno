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

    private Task<bool> connectTask;

    public ConnectingWindow(string address, int port)
    {
        this.address = address;
        this.port = port;

        Connect();
    }

    private void Connect()
    {
        // i changed this to switch the scene on the main thread so the menu switch doesn't swap mid frame
        // this is ok for scenes, since is SwitchScene() queues a scene and does the switch after the frame,
        // but not okay for menus, which just push right to the stack.
        Client.Start(address, port);
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

        // we do this from the main thread now :)

        // if client has stopped trying to connect we should handle it
        if (!Client.IsConnecting)
        {
            if (Client.IsConnected)
            {
                // we sucessfully connected
                MenuScene.windows.Pop(); // pop this window
                MenuScene.windows.Push(new NameSelectMenu()); // push lobby window
            }
            else
            {
                // failure :(
                MenuScene!.windows.Pop();
                MenuScene.errmsg = "WHAT THE HEK... no connect :(";
            }
        }
    }
}
