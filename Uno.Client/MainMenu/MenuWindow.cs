﻿using ImGuiNET;
using SimulationFramework.Drawing;
using SimulationFramework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Uno.Client.MainMenu;
internal abstract class MenuWindow
{
    public MenuScene? MenuScene => UnoGame.Current.ActiveScene as MenuScene;

    public virtual ImGuiWindowFlags WindowFlags => ImGuiWindowFlags.NoDecoration | ImGuiWindowFlags.AlwaysAutoResize;
    public virtual string Title { get; }
    public virtual Vector2 Pivot => new(.5f);
    public virtual Alignment Alignment => Alignment.Center;
    public virtual bool IsModal => false;

    public virtual void Layout()
    {
        Rectangle bounds = new(0, 0, Graphics.GetOutputCanvas().Width, Graphics.GetOutputCanvas().Height);

        ImGui.SetNextWindowPos(bounds.GetAlignedPoint(Alignment), ImGuiCond.Always, Pivot);
        
        var title = Title ?? this.GetHashCode().ToString();

        if (IsModal)
        {

            ImGui.OpenPopup(title);
            bool open = true;
            if (ImGui.BeginPopupModal(title, ref open, WindowFlags))
            {
                LayoutContent();
                ImGui.End();
            }
        }
        else
        {
            if (ImGui.Begin(title, WindowFlags))
            {
                LayoutContent();
            }
            ImGui.End();
        }
    }

    protected abstract void LayoutContent();
}
