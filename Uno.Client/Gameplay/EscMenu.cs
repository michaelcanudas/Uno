using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Uno.Client.MainMenu;

namespace Uno.Client.Gameplay;
internal class EscMenu : MenuWindow
{
    public bool WantClose { get; private set; } = false;

    public override bool IsModal => true;

    protected override void LayoutContent()
    {
        const float buttonWidth = 150;

        if (ImGui.Button("Resume", new Vector2(buttonWidth, 0)))
        {
            WantClose = true;
        }

        if (ImGui.Button("Settings", new Vector2(buttonWidth, 0)))
        {
        }

        if (ImGui.Button("Exit To Menu", new Vector2(buttonWidth, 0)))
        {
            Client.Disconnect();
            UnoGame.Current.SwitchScenes(new MainMenuScene());
        }

        if (ImGui.Button("Exit To Desktop", new Vector2(buttonWidth, 0)))
        {
            Environment.Exit(0);
        } 
    }
}
