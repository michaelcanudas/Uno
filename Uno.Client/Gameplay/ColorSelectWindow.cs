using SimulationFramework.Drawing;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Uno.Client.MainMenu;

namespace Uno.Client.Gameplay;
internal class ColorSelectWindow : MenuWindow
{
    public CardColor? SelectedColor;

    [MemberNotNullWhen(true, nameof(SelectedColor))]
    public bool IsColorSelected => SelectedColor is not null;

    bool open = true;

    public override void Layout()
    {
        open = !IsColorSelected;

        ImGui.OpenPopup("selectedcolor");

        ImGui.SetNextWindowPos(new(Graphics.GetOutputCanvas().Width / 2, Graphics.GetOutputCanvas().Height / 2), ImGuiCond.Always, Vector2.One * .5f);
        if (ImGui.BeginPopupModal("selectedcolor", ref open, ImGuiWindowFlags.NoDecoration | ImGuiWindowFlags.NoMove))
        {
            ImGui.Text("Select color:");
            ImGui.Separator();

            if (ImGui.ColorButton("red", Color.Red.AsVector4(), ImGuiColorEditFlags.NoTooltip, Vector2.One * 100))
                SelectedColor = CardColor.Red;
            if (ImGui.IsItemHovered())
                ImGui.SetTooltip("red");

            ImGui.SameLine();
            if (ImGui.ColorButton("blue", Color.Blue.AsVector4(), ImGuiColorEditFlags.NoTooltip, Vector2.One * 100))
                SelectedColor = CardColor.Blue;
            if (ImGui.IsItemHovered())
                ImGui.SetTooltip("blue");

            if (ImGui.ColorButton("green", Color.Green.AsVector4(), ImGuiColorEditFlags.NoTooltip, Vector2.One * 100))
                SelectedColor = CardColor.Green;
            if (ImGui.IsItemHovered())
                ImGui.SetTooltip("green");

            ImGui.SameLine();
            if (ImGui.ColorButton("yellow", Color.Yellow.AsVector4(), ImGuiColorEditFlags.NoTooltip, Vector2.One * 100))
                SelectedColor = CardColor.Yellow;
            if (ImGui.IsItemHovered())
                ImGui.SetTooltip("yellow");

            ImGui.EndPopup();
        }
    }

    protected override void LayoutContent()
    {
    }
}
