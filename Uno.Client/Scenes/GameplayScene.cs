using SimulationFramework.Drawing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Uno.Client.Scenes;
internal class GameplayScene : GameScene
{
    public GameplayScene(Client client)
    {

    }

    public override void Render(ICanvas canvas)
    {
        canvas.DrawText("You are now playing uno.", new(10, 10));
        canvas.DrawText("You are now having fun.", new(10, 40));
        base.Render(canvas);
    }

    public override void Update()
    {
        base.Update();
    }
}
