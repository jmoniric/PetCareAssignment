using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace PetCareGame;

public class DisplayManager
{
    private Game _game;
    private GraphicsDeviceManager _graphics;
    private GraphicsDevice _graphicsDevice;
    public RenderTarget2D _renderTarget;
    public Rectangle _renderDestination;
    public float scaleFactor;

    public DisplayManager(Game game, GraphicsDeviceManager graphics, GraphicsDevice graphicsDevice){
        _game = game;
        _graphics = graphics;
        _graphicsDevice = graphicsDevice;
        _renderTarget = new(graphicsDevice, 800, 600);
        SetResolution(800, 600);
    }

    // Changes the resolution of window to a specified
    public void SetResolution(int width, int height){
        _graphicsDevice.PresentationParameters.BackBufferWidth = width;
        _graphicsDevice.PresentationParameters.BackBufferHeight = height;
        _graphics.ApplyChanges(); // ApplyChanges() is required for changes to screen outside of GameHandler
        CalculateRectangleDestination();
    }

    // Changes game window to fullscreen
    public void SetFullScreen(){
        _graphicsDevice.PresentationParameters.BackBufferWidth = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Width;
        _graphicsDevice.PresentationParameters.BackBufferWidth = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Height;
        _graphics.IsFullScreen = true;
        _graphics.ApplyChanges();
    }

    public void CalculateRectangleDestination(){
        Point size = _graphicsDevice.Viewport.Bounds.Size;

        float scaleX = (float)size.X / _renderTarget.Width;
        float scaleY = (float)size.Y / _renderTarget.Height;
        float scale = Math.Min(scaleX, scaleY);
        scaleFactor = scale;

        _renderDestination.Width = (int)(_renderTarget.Width * scale);
        _renderDestination.Height = (int)(_renderTarget.Height * scale);

        _renderDestination.X = (size.X - _renderDestination.Width) / 2;
        _renderDestination.Y = (size.Y - _renderDestination.Height) / 2;
    }

}