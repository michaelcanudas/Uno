using ImGuiNET;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Uno.Packets;

namespace Uno.Client.MainMenu;
internal class LobbyMenu : MenuWindow
{
    List<string> players;
    int selectedPlayer;
    int spectators;

    public override string Title => "Lobby";

    public LobbyMenu(IEnumerable<string> initialPlayers, int initialSpectatorCount)
    {
        players = new(initialPlayers);
        spectators = initialSpectatorCount;
    }

    protected override void LayoutContent()
    {
        foreach (var packet in Client.Receive<PlayerJoinedPacket>())
        {
            players.Add(packet.Name);
        }

        foreach (var packet in Client.Receive<PlayerLeftPacket>())
        {
            players.Remove(packet.Name);
        }

        foreach (var packet in Client.Receive<SpectatorCountPacket>())
        {
            spectators = packet.SpectatorCount;
        }

        ImGui.Text("Players:");
        ImGui.ListBox("##player listbox", ref selectedPlayer, players.ToArray(), players.Count);
        ImGui.Text($"spectators: {spectators}");
        ImGui.Separator();

        if (ImGui.Button("Leave"))
        {
            Client.Disconnect();
            MenuScene!.windows.Pop();
        }
    }
}
