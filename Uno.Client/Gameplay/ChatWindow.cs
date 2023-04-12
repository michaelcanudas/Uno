using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Uno.Actions;
using Uno.Client.MainMenu;
using Uno.Packets;

namespace Uno.Client.Gameplay;
internal class ChatWindow : MenuWindow
{
    public override ImGuiWindowFlags WindowFlags => ImGuiWindowFlags.NoDecoration | ImGuiWindowFlags.NoScrollWithMouse;
    public override Vector2 Pivot => Vector2.UnitY;
    public override Alignment Alignment => Alignment.BottomLeft;

    public List<string> Entries = new();
    public string inputTextValue = "";
    public bool wantSendMessage = true;
    bool shouldAutoScroll;
    public string? CurrentMessage = null;

    protected override void LayoutContent()
    {
        wantSendMessage = false;

        ImGui.SetWindowSize(new(200, 170));

        ImGui.SetNextItemWidth(200-15);
        ImGui.BeginListBox("##listbox");

        foreach (var e in Entries)
        {
            ImGui.Text(e);
        }

        if (shouldAutoScroll)
        {
            ImGui.SetScrollHereY();
            shouldAutoScroll = false;
        }

        ImGui.SetScrollHereY();
        ImGui.EndListBox();

        if (ImGui.InputText("##textinput", ref inputTextValue, 256, ImGuiInputTextFlags.EnterReturnsTrue))
        {
            wantSendMessage = true;
            CurrentMessage = inputTextValue;
            inputTextValue = "";
        }

        ImGui.SameLine();
        ImGui.Button("Send");
    }

    public void AddMessage(string sender, string message)
    {
        shouldAutoScroll = true;
        Entries.Add($"[{sender}]: {message}");
    }

    public void AddNotification(string notification)
    {
        shouldAutoScroll = true;
        Entries.Add(notification);
    }
}
