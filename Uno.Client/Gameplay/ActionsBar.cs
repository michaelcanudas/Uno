using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Uno.Client.MainMenu;

namespace Uno.Client.Gameplay;
internal class ActionsBar : MenuWindow
{
    public override Vector2 Pivot => new(.5f, 1f);
    public override Alignment Alignment => Alignment.BottomCenter;

    public bool UnoPressed { get; set; }
    public bool CallengePressed { get; set; }

    protected override void LayoutContent()
    {
        UnoPressed = ImGui.Button("Uno!");
        ImGui.SameLine();
        CallengePressed = ImGui.Button("Challenge");
    }
}
