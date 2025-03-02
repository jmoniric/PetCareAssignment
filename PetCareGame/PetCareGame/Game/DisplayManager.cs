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

    public DisplayManager(Game game, GraphicsDeviceManager graphics, GraphicsDevice graphicsDevice){
        _game = game;
        _graphics = graphics;
        _graphicsDevice = graphicsDevice;
        SetResolution(800, 600);
    }

    // Changes the resolution of window to a specified
    public void SetResolution(int x, int y){
        _graphics.PreferredBackBufferWidth = x;
        _graphics.PreferredBackBufferHeight = y;
        _graphics.ApplyChanges(); // ApplyChanges() is required for changes to screen outside of GameHandler
    }

    // Changes game window to fullscreen
    public void SetFullScreen(){
        _graphics.PreferredBackBufferWidth = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Width;
        _graphics.PreferredBackBufferHeight = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Height;
        _graphics.IsFullScreen = true;
        _graphics.ApplyChanges();
    }

    public void CalculateRectangleDestination(){
        Point size = _graphicsDevice.Viewport.Bounds.Size;

        float scaleX = (float)size.X / _renderTarget.Width;
        float scaleY = (float)size.Y / _renderTarget.Height;
        float scale = Math.Min(scaleX, scaleY);

        _renderDestination.Width = (int)(_renderTarget.Width * scale);
        _renderDestination.Height = (int)(_renderTarget.Height * scale);

        _renderDestination.X = (size.X - _renderDestination.Width) / 2;
        _renderDestination.Y = (size.Y - _renderDestination.Height) / 2;
    }

    public void CalculateButtonDimensionsNPosition(Button button){
        Point size = _graphicsDevice.Viewport.Bounds.Size;

        float scaleX = (float)size.X / _renderTarget.Width;
        float scaleY = (float)size.Y / _renderTarget.Height;
        float scale = Math.Min(scaleX, scaleY);

        Vector2 vector2 = button.Position;
        
        vector2.X = (int)(vector2.X * scale);
        vector2.Y = (int)(vector2.Y * scale);

        button.Position = vector2;

        Point dimensions = button.Dimensions;

        dimensions.X = (size.X - dimensions.X) / 2;
        dimensions.Y = (size.Y - dimensions.Y) / 2;

        button.Dimensions = dimensions;
    }

}