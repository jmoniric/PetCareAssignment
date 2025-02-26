using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace PetCareGame;

public class DisplayManager
{
    private Game _game;
    private GraphicsDeviceManager _graphics;
    public static int windowHeight = 1080;
    public static int windowWidth = 1920;

    public DisplayManager(Game game, GraphicsDeviceManager graphics){
        _game = game;
        _graphics = graphics;
        SetResolution(windowWidth, windowHeight);
    }

    // 
    public void SetResolution(int x, int y){
        _graphics.PreferredBackBufferWidth = x;
        _graphics.PreferredBackBufferHeight = y;
        _graphics.ApplyChanges();
    }

    //
    public void SetFullScreen(){
        _graphics.PreferredBackBufferWidth = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Width;
        _graphics.PreferredBackBufferHeight = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Height;
        _graphics.IsFullScreen = true;
    }

    public void Draw(SpriteBatch spriteBatch)
    {
        
    }

}