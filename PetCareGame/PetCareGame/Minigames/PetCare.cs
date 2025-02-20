using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace PetCareGame;

public class PetCare : LevelInterface
{
    private int catX = 1000;
    private Color backgroundColour = new Color(197, 118, 38);
    private Texture2D atlas;

    public void Dispose()
    {
        
    }

    public void Draw(GameTime gameTime, SpriteBatch spriteBatch, GraphicsDeviceManager _graphics)
    {
        Rectangle floor = new Rectangle(0, 0, 31, 31);
        Rectangle floorFiller = new Rectangle(0, 12, 16, 16);
        Rectangle wall = new Rectangle(32, 0, 31, 31);

        _graphics.GraphicsDevice.Clear(backgroundColour);
        GameHandler.catIdle.DrawFrame(spriteBatch, new Vector2(catX, 720));

        for(int i = 0; i < 16; i++) {
            spriteBatch.Draw(atlas, new Rectangle(i*124, 830, 128, 128), floor, Color.White, 0f, Vector2.Zero, SpriteEffects.None, 1f);
            spriteBatch.Draw(atlas, new Rectangle(i*124, 958, 128, 128), floorFiller, Color.White, 0f, Vector2.Zero, SpriteEffects.None, 1f);
        }
        for(int h = 0; h < 16; h++) {
            for(int v = 0; v < 8; v++) {
                spriteBatch.Draw(atlas, new Rectangle(h*124, v*124, 128, 128), wall, Color.White, 0f, Vector2.Zero, SpriteEffects.None, 0f);
            }
        }
        
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
        atlas = _manager.Load<Texture2D>("Sprites/petcare_textureatlas");
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