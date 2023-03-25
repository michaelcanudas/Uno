using SimulationFramework;
using SimulationFramework.Drawing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Uno.Client.Scenes;

namespace Uno.Client;

internal static class Program
{
    private static void Main()
    {
        new UnoGame(new MainMenuScene()).Run();
    }
}
