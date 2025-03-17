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

        // Calculate the virtual resolution based on the current screen width and height compared to our
        // intended resolution width and height
        if(screenWidth / _resolutionWidth > screenHeight / _resolutionHeight)
        {
            float aspect = screenHeight / _resolutionHeight;
            _virtualWidth = (int)(aspect * _resolutionWidth);
            _virtualHeight = (int)screenHeight;
        }
        else
        {
            float aspect = screenWidth / _resolutionWidth;
            _virtualWidth = (int)screenWidth;
            _virtualHeight = (int)(aspect * _resolutionHeight);
        }

        _scaleMatrix = Matrix.CreateScale(_virtualWidth / (float)_resolutionWidth, _virtualHeight / (float)_resolutionHeight, 1.0f);

        _viewport = new Viewport
        {
            X = (int)(screenWidth / 2 - _virtualWidth / 2),
            Y = (int)(screenHeight / 2 - _virtualHeight / 2),
            Width = _virtualWidth,
            Height = _virtualHeight,
            MinDepth = 0,
            MaxDepth = 1
        };
    }

}