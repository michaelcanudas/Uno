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
    public static UnoGame Current { get; private set; }

    GameScene activeScene;
    GameScene? nextScene;
    public CardRenderer CardRenderer { get; private set; }

    public UnoGame(GameScene initialScene)
    {
        activeScene = initialScene;
    }

    public void SwitchScenes(GameScene nextScene)
    {
        this.nextScene = nextScene;
    }

    public override void OnInitialize(AppConfig config)
    {
        CardRenderer = new();
        Current = this;
        activeScene.Begin();
    }

    public override void OnRender(ICanvas canvas)
    {
        if (nextScene is not null)
        {
            activeScene.End();
            nextScene.Begin();

            activeScene = nextScene;
        }

        canvas.Clear(Color.FromHSV(0, 0, .1f));

        activeScene.Update();
        activeScene.Render(canvas); 
    }
}
