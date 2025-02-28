using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace PetCareGame;

public class DisplayManager
{
    private Game _game;
    private GraphicsDeviceManager _graphics;

    public DisplayManager(Game game, GraphicsDeviceManager graphics){
        _game = game;
        _graphics = graphics;
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
    }

    private void Draw(SpriteBatch spriteBatch)
    {
        
    }

}