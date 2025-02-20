using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace PetCareGame;

public class PetCare : LevelInterface
{
    private int catX = 1000;
    private Color backgroundColour = new Color(197, 118, 38);

    public void Dispose()
    {
        
    }

    public void Draw(GameTime gameTime, SpriteBatch spriteBatch, GraphicsDeviceManager _graphics)
    {
        _graphics.GraphicsDevice.Clear(backgroundColour);
        GameHandler.catIdle.DrawFrame(spriteBatch, new Vector2(catX, 700));
    }

    public void HandleInput(GameTime gameTime)
    {
        KeyboardState state = Keyboard.GetState();

        if((state.IsKeyDown(Keys.A) || state.IsKeyDown(Keys.Left)) && catX > 10) {
            catX-= 2;
        }
        if((state.IsKeyDown(Keys.D) || state.IsKeyDown(Keys.Right)) && catX < 1910) {
            catX+= 2;
        }
    }

    public void LoadContent(ContentManager _manager, ContentManager _coreAssets)
    {
        //loads cat in at bigger scale for my game
        GameHandler.catIdle = new AnimatedTexture(new Vector2(32,16), 0f, 4f, 0.5f);
        GameHandler.catIdle.Load(_coreAssets, "Sprites/Animal/idle", 7, 5);
    }

    public void LoadLevel()
    {
        
    }

    public void Update(GameTime gameTime)
    {
        float elapsed = (float)gameTime.ElapsedGameTime.TotalSeconds;
        GameHandler.catIdle.UpdateFrame(elapsed);
    }
}