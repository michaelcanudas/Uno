using ImGuiNET;
using SimulationFramework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Uno.Packets;

namespace Uno.Client.MainMenu;
internal class NameSelectMenu : MenuWindow
{
    const string NameCacheFile = "name.txt";

    public override string Title => "Joining lobby";

    string currentInput = "";

    string? sentName;
    bool isJoining;
    bool isJoiningAsSpectator;
    string? error;
    string? info;

    public NameSelectMenu()
    {
        try
        {
            currentInput = File.ReadAllText(NameCacheFile);
        }
        catch
        {
            currentInput = "";
        }
    }

    protected override void LayoutContent()
    {
        const ImGuiInputTextFlags inputTextFlags = ImGuiInputTextFlags.EnterReturnsTrue;

        ImGui.Text("Enter Name:");
        bool hitEnter = ImGui.InputText("##input text box", ref currentInput, 32, inputTextFlags);

        ImGui.Separator();
        if (ImGui.Button("Cancel"))
        {
            Client.Disconnect();
            MenuScene.windows.Pop();
            return;
        }

        ImGui.SameLine();
        if (ImGui.Button("Spectate"))
        {
            Client.Send(new EnterAsSpectatorPacket());
            isJoiningAsSpectator = isJoining = true;
        }

        ImGui.SameLine();
        if ((ImGui.Button("Join") || hitEnter) && !isJoining)
        {
            currentInput = currentInput.Trim();

            if (currentInput != string.Empty && currentInput.All(char.IsLetterOrDigit))
            {
                // try to enter game
                Client.Send(new EnterAsPlayerPacket() { Name = currentInput });
                isJoining = true;
                isJoiningAsSpectator = false;
                sentName = currentInput;

                File.WriteAllText(NameCacheFile, currentInput);
            }
            else
            {
                error = "Invalid Name";
            }
        }


        currentInput = string.Concat(currentInput.Where(c => char.IsLetterOrDigit(c)));

        if (error is not null)
        {
            ImGui.Separator();
            ImGui.TextColored(Color.Red.AsVector4(), error);
        }
        else if (isJoining)
        {
            ImGui.Separator();
            ImGui.Text("Joining...");
        }

        if (isJoining)
        {
            var packet = Client.Receive<WelcomePacket>().FirstOrDefault();

            if (packet is not null)
            {
                if (packet.Success)
                {
                    // we're in.

                    var players = packet.Players;
                    var spectators = packet.spectators;

                    // transition to lobby menu
                    MenuScene!.windows.Pop(); // pop this window, we don't need it and leaving should go to main menu
                    MenuScene!.windows.Push(new LobbyMenu(players, spectators));
                }
                else
                {
                    error = "Invalid or taken name.";
                }
            }
        }
    }
}