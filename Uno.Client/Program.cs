using SimulationFramework;
using SimulationFramework.Desktop;
using SimulationFramework.Drawing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Uno.Client.Scenes;
using Uno.Client.Scenes.MainMenu;

namespace Uno.Client;

internal static class Program
{
    private static void Main()
    {
        GameScene scene = new MainMenuScene();

        // IMPORTANT: COMMENT THIS OUT IF YOU ARENT ME TESTING RENDERING
        // scene = new RendererDemoScene();
        
        var game = new UnoGame(scene);
        game.RunDesktop();
    }
}
