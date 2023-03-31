using SimulationFramework.Drawing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Uno.Client;
internal class Camera
{
    public static Camera Active { get; private set; }

    public Vector2 Position { get; set; }

    public float Width { get => Height * AspectRatio; set => Height = value / AspectRatio; }
    public float Height { get; set; } = 1f;
    public float Scale { get; set; } = 1f;

    public float DisplayWidth { get; set; }
    public float DisplayHeight { get; set; }
    
    public float AspectRatio => DisplayWidth / DisplayHeight;


    // creates a new camera. width is determined via aspect ratio.
    public Camera(float height)
    {
        this.Height = height;
    }

    public void SetActive()
    {
        Active = this;
    }

    public virtual void Update()
    {
        DisplayWidth = Graphics.GetOutputCanvas().Width;
        DisplayHeight = Graphics.GetOutputCanvas().Height;
    }

    public virtual void ApplyTo(ICanvas canvas)
    {
        canvas.Transform(CreateWorldToScreenMatrix());
    }

    public Vector2 ScreenToWorld(Vector2 point)
    {
        return Vector2.Transform(point, CreateScreenToWorldMatrix());
    }

    public Vector2 WorldToScreen(Vector2 point)
    {
        return Vector2.Transform(point, CreateWorldToScreenMatrix());
    }

    public virtual Matrix3x2 CreateScreenToWorldMatrix()
    {
        return
            Matrix3x2.CreateTranslation(-DisplayWidth / 2f, -DisplayHeight / 2f) *
            Matrix3x2.CreateScale(Scale * Height / DisplayHeight) *
            Matrix3x2.CreateTranslation(Position);
    }

    public virtual Matrix3x2 CreateWorldToScreenMatrix()
    {
        return
            Matrix3x2.CreateTranslation(-Position) *
            Matrix3x2.CreateScale(DisplayHeight / (Scale * Height)) *
            Matrix3x2.CreateTranslation(DisplayWidth / 2f, DisplayHeight / 2f);
    }
}
