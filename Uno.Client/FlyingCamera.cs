using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Uno.Client;
internal class FlyingCamera : Camera
{
    public float Speed { get; set; } = 1f;
    public float Zoom { get => MathF.Log(Scale, 1.1f); set => Scale = MathF.Pow(1.1f, value); }

    public FlyingCamera(float height) : base(height)
    {
    }

    public override void Update()
    {
        Vector2 delta = Vector2.Zero;

        if (Keyboard.IsKeyDown(Key.W))
            delta += Vector2.UnitY;
        if (Keyboard.IsKeyDown(Key.A))
            delta += Vector2.UnitX;
        if (Keyboard.IsKeyDown(Key.S))
            delta -= Vector2.UnitY;
        if (Keyboard.IsKeyDown(Key.D))
            delta -= Vector2.UnitX;

        Position -= delta * Time.DeltaTime * Speed;
        Zoom -= Mouse.ScrollWheelDelta;

        base.Update();
    }
}