using SimulationFramework;
using SimulationFramework.Desktop;
using SimulationFramework.Drawing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Uno.Client.MainMenu;
using Uno.Packets;

namespace Uno.Client;

internal static class Program
{
    private static void Main()
    {
        if (Environment.ProcessPath != null) 
        {
            Environment.CurrentDirectory = Path.GetDirectoryName(Environment.ProcessPath)!;
        }

        GameScene scene = new MenuScene();
        
        // IMPORTANT: COMMENT THIS OUT IF YOU ARENT ME TESTING RENDERING
        // scene = new RendererDemoScene();
        
        var game = new UnoGame(scene);
        game.Run(new DesktopPlatform());
    }
}