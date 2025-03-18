using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace PetCareGame;

public class SlidingGame : LevelInterface
{

    private Color backgroundColour = new Color(50, 205, 50);

    private Vector2 catPos = new Vector2(GameHandler.windowWidth / 2, 720);

    private Vector2 boxPos = new Vector2(GameHandler.windowWidth / 2, 840);

    private Point chestPos = new Point(400, 400);

    private Texture2D atlas;

    private Texture2D chest;

    private Rectangle chestBounds;

    private bool faceRight = true;


    public void Dispose()
    {

    }

    public void Draw(GameTime gameTime, SpriteBatch spriteBatch, GraphicsDeviceManager _graphics)
    {

        Rectangle grass = new Rectangle(16, 0, 16, 16);

        Rectangle chestRect = new Rectangle(0, 0, 32, 32);

        _graphics.GraphicsDevice.Clear(backgroundColour);


        int squareSize = GameHandler.windowWidth / 2;
        int squareX = (GameHandler.windowWidth - squareSize) / 2;
        int squareY = (GameHandler.windowHeight - squareSize) / 2;

        Rectangle textureSource = new Rectangle(64, 0, 16, 16); // Select texture from the atlas



        //draw background
        for (int h = 0; h < 16; h++)
        {
            for (int v = 0; v < 16; v++)
            {
                spriteBatch.Draw(atlas, new Rectangle(h * 128, v * 128, 128, 128), grass, Color.White, 0f, Vector2.Zero, SpriteEffects.None, 0f);
            }
        }

        //inner square
        spriteBatch.Draw(atlas, new Rectangle(squareX, squareY, squareSize, 5), textureSource, Color.White);
        // Bottom edge
        spriteBatch.Draw(atlas, new Rectangle(squareX, squareY + squareSize - 5, squareSize, 5), textureSource, Color.White);
        // Left edge
        spriteBatch.Draw(atlas, new Rectangle(squareX, squareY, 5, squareSize), textureSource, Color.White);
        // Right edge
        spriteBatch.Draw(atlas, new Rectangle(squareX + squareSize - 5, squareY, 5, squareSize), textureSource, Color.White);



        //Chests, cats and other sprites
        chestBounds = new Rectangle(chestPos, new Point(96, 96));
        
        

        //draw cat
        if (faceRight)
        {
            GameHandler.catIdle.DrawFrame(spriteBatch, catPos, SpriteEffects.None);
        }
        else
        {
            GameHandler.catIdle.DrawFrame(spriteBatch, catPos, SpriteEffects.FlipHorizontally);
        }

        spriteBatch.Draw(chest, chestBounds, chestRect, Color.White, 0f, Vector2.Zero, SpriteEffects.None, 0f);




    }

    public void HandleInput(GameTime gameTime)
    {

        KeyboardState keyboardState = Keyboard.GetState();

        float elapsed = (float)gameTime.ElapsedGameTime.TotalSeconds;

        if (keyboardState.IsKeyDown(Keys.Left))
        {
            faceRight = false;
            catPos.X -= 300 * elapsed;
        }
        if (keyboardState.IsKeyDown(Keys.Right))
        {
            faceRight = true;
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
        GameHandler.catIdle.Load(_coreAssets, "Sprites/Animal/idle", 7, 5);
        chest = _manager.Load<Texture2D>("Sprites/treasure_atlas");
    }

    public void LoadLevel()
    {

    }

    public void Update(GameTime gameTime)
    {


    }
}