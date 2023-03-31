using ImGuiNET;
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
    public MainMenuScene? MenuScene => UnoGame.Current.ActiveScene as MainMenuScene;

    public virtual ImGuiWindowFlags WindowFlags => ImGuiWindowFlags.NoDecoration | ImGuiWindowFlags.AlwaysAutoResize;
    public virtual string Title { get; }

    public virtual void Layout()
    {
        ImGui.SetNextWindowPos(new(Graphics.GetOutputCanvas().Width / 2, Graphics.GetOutputCanvas().Height / 2), ImGuiCond.Always, new(.5f));
        if (ImGui.Begin(Title ?? string.Empty, WindowFlags))
        {
            LayoutContent();
        }
        ImGui.End();
    }

    protected abstract void LayoutContent();
}
