using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace PetCareGame;

public class SlidingGame : LevelInterface
{

    private Color backgroundColour = new Color(50, 205, 50);

    private Vector2 catPos = new Vector2(GameHandler.baseScreenSize.X / 2, 300);

    private Vector2 boxPos = new Vector2(GameHandler.baseScreenSize.X / 2, 840);

    private Point chestPos = new Point(120, 100);

    private Texture2D atlas;

    private Texture2D chest;

    private Rectangle chestBounds;


    public void Dispose()
    {

    }

    public void Draw(GameTime gameTime, SpriteBatch spriteBatch, GraphicsDeviceManager _graphics)
    {

        Rectangle grass = new Rectangle(16, 0, 16, 16);

        Rectangle chestRect = new Rectangle(0, 0, 32, 32);

        _graphics.GraphicsDevice.Clear(backgroundColour);


        //draw background
        for (int h = 0; h < 13; h++)
        {
            for (int v = 0; v < 8; v++)
            {
                spriteBatch.Draw(atlas, new Rectangle(h * 64, v * 64, 64, 64), grass, Color.White, 0f, Vector2.Zero, SpriteEffects.None, 0f);
            }
        }


        chestBounds = new Rectangle(chestPos, new Point(96, 96));

        GameHandler.catIdle.DrawFrame(spriteBatch, catPos, SpriteEffects.None);

        spriteBatch.Draw(chest, chestBounds, chestRect, Color.White, 0f, Vector2.Zero, SpriteEffects.None, 0f);




    }

    public void HandleInput(GameTime gameTime)
    {

        KeyboardState keyboardState = Keyboard.GetState();

        float elapsed = (float)gameTime.ElapsedGameTime.TotalSeconds;

        if (keyboardState.IsKeyDown(Keys.Left))
        {
            catPos.X -= 300 * elapsed;
        }
        if (keyboardState.IsKeyDown(Keys.Right))
        {
            catPos.X += 300 * elapsed;
        }
        if (keyboardState.IsKeyDown(Keys.Up))
        {
            catPos.Y -= 300 * elapsed;
        }
        if (keyboardState.IsKeyDown(Keys.Down))
        {
            catPos.Y += 300 * elapsed;
        }



    }

    public void LoadContent(ContentManager _manager, ContentManager _coreAssets)
    {
        atlas = _manager.Load<Texture2D>("Sprites/petcare_slidetextureatlas");
        chest = _manager.Load<Texture2D>("Sprites/treasure_atlas");

        //you don't need to load this again, it was already loaded inside the GameHandler
        //GameHandler.catIdle.Load(_coreAssets, "Sprites/Animal/idle", 7, 5);
    }

    public void LoadLevel()
    {

    }

    public void Update(GameTime gameTime)
    {


    }
}