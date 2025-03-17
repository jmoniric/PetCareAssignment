using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace PetCareGame;

public class DisplayManager
{
    private Game _game;
    private GraphicsDeviceManager _graphics;
    private GraphicsDevice _graphicsDevice;
    
    // Desired Resolution
    private readonly int _resolutionWidth = 800;
    private readonly int _resolutionHeight = 600;

    // Resolution the game will be rendered at
    private int _virtualWidth = 800;
    private int _virtualHeight = 600;

    public Matrix _scaleMatrix;
    public Viewport _viewport;

    public DisplayManager(Game game, GraphicsDeviceManager graphics, GraphicsDevice graphicsDevice){
        _game = game;
        _graphics = graphics;
        _graphicsDevice = graphicsDevice;
        SetResolution(_resolutionWidth, _resolutionHeight);
        UpdateScreenScaleMatrix();
    }

    // Changes the resolution of window to a specified resolution
    public void SetResolution(int width, int height){
        _graphics.PreferredBackBufferWidth = width;
        _graphics.PreferredBackBufferHeight = height;
        _graphics.ApplyChanges();
    }

    // Changes game window to fullscreen
    public void SetFullScreen(){
        _graphicsDevice.PresentationParameters.BackBufferWidth = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Width;
        _graphicsDevice.PresentationParameters.BackBufferHeight = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Height;
        _graphics.IsFullScreen = true;
        _graphics.ApplyChanges();
    }
    public void UpdateScreenScaleMatrix()
    {
        // Determine the size of the actual screen
        float screenWidth = _graphicsDevice.PresentationParameters.BackBufferWidth;
        float screenHeight = _graphicsDevice.PresentationParameters.BackBufferHeight;
        //fix window scaling before checking input
        if (screenHeight != _graphicsDevice.PresentationParameters.BackBufferHeight ||
                screenWidth != _graphicsDevice.PresentationParameters.BackBufferWidth)
        {
            //window size 480x800 reduces down to 3x5
            //checks that user set width is a factor of 5 or height is factor of 3
            //if this fails, it uses integer division (which rounds to nearest whole number)
            //to find other number and then multiplies 3 and 5 by that factor and sets
            //it to height and width, respectively, before updating graphics

            if (_graphicsDevice.PresentationParameters.BackBufferWidth % 4 != 0)
            {
                //factor being multiplied by 5
                int factor = _graphicsDevice.PresentationParameters.BackBufferWidth / 4;
                //set preferred dimensions to aspect-correct dimensions
                _graphics.PreferredBackBufferWidth = factor * 4;
                _graphics.PreferredBackBufferHeight = factor * 3;
                //applies changes
                _graphics.ApplyChanges();
            }
            else if (_graphicsDevice.PresentationParameters.BackBufferHeight % 3 != 0)
            {
                //factor being multiplied by 3
                int factor = _graphicsDevice.PresentationParameters.BackBufferHeight / 3;
                //set preferred dimensions to aspect-correct dimensions
                _graphics.PreferredBackBufferWidth = factor * 4;
                _graphics.PreferredBackBufferHeight = factor * 3;
                //applies changes
                _graphics.ApplyChanges();
            }
        }

        float scaleX = screenWidth / _resolutionWidth;
        float scaleY = screenHeight / _resolutionHeight;
        Vector3 scale = new(scaleX, scaleY, 1);

        _scaleMatrix = Matrix.CreateScale(scale);

    }

}