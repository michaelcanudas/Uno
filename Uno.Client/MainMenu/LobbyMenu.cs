using ImGuiNET;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Uno.Client.Gameplay;
using Uno.Packets;

namespace Uno.Client.MainMenu;
internal class LobbyMenu : MenuWindow
{
    List<string> players;
    int selectedPlayer;
    int spectators;
    // this needs to be passed to the gameplay scene
    string playerName;
    bool elevated;

    public override string Title => "Lobby";

    public LobbyMenu(string playerName, bool elevated, IEnumerable<string> initialPlayers, int initialSpectatorCount)
    {
        this.playerName = playerName;
        this.elevated = elevated;
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

        if (elevated)
        {
            ImGui.SameLine();
            if (ImGui.Button("Start"))
            {
                Client.Send(new StartRequestPacket());
            }
        }

        if (Client.Receive<StartPacket>(out var startPacket))
        {
            UnoGame.Current.SwitchScenes(new GameplayScene(playerName, startPacket));
        }
    }
}
