using SimulationFramework;
using SimulationFramework.Drawing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Uno.Client;
internal class UnoGame : Simulation
{
    GameScene activeScene;

    public UnoGame(GameScene initialScene)
    {
        activeScene = initialScene;
    }

    public override void OnInitialize(AppConfig config)
    {
    }

    public override void OnRender(ICanvas canvas)
    {
        canvas.Clear(Color.FromHSV(0, 0, .1f));
        activeScene.Update();
        activeScene.Render(canvas); 
    }
}
