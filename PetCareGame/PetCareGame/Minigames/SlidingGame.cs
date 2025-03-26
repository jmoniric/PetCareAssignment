using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace PetCareGame;

public class SlidingGame : LevelInterface
{

    private Color backgroundColour = new Color(50, 205, 50);

    private Vector2 catPos = new Vector2(200, 150);


    private Point chestPos = new Point(400, 350);

    private Texture2D atlas;

    private Texture2D chest;



    private Rectangle chestBounds;


    private bool faceRight = true;

    private GameStage currentStage = GameStage.Instructions;

    private Button startButton;
    private Point startButtonPos = new Point(270, 510);

    private Rectangle startButtonBounds;
    private bool isMoving;

    // Frog(s)
    private Texture2D frog;

    private List<Vector2> frogPositions = new List<Vector2>();
    private List<bool> frogMovingRight = new List<bool>();


    // Animation variables
    private int frameWidth;
    private int frameHeight;
    private int frameCount = 7;
    private float frameSpeed = 0.1f;
    private float animationTimer = 0f;
    private int currentFrame = 0;

    //Movement
    private float frogSpeed = 150f; // Adjust speed as needed


    //frog (s) (end)

    enum GameStage
    {
        Instructions,
        Run,

        Completion
    }

    private bool mouseDown = false;



    public void CleanupProcesses()
    {

        catPos = new Vector2(400, 305);


        mouseDown = false;
        faceRight = true;

        currentStage = GameStage.Instructions;
        startButtonPos = new Point(270, 510);


    }

    public void Dispose()
    {

    }



    public void Draw(GameTime gameTime, SpriteBatch spriteBatch, GraphicsDeviceManager _graphics)
    {
        Rectangle grass = new Rectangle(16, 0, 16, 16);
        Rectangle chestRect = new Rectangle(0, 0, 32, 32);
        _graphics.GraphicsDevice.Clear(backgroundColour);


        Rectangle centerTextureSource = new Rectangle(32, 32, 16, 16);
        Rectangle openchestTextureSource = new Rectangle(0, 32, 32, 32);



        int centerX = ((GameHandler.windowWidth - 64) / 2) - 185;
        int centerY = ((GameHandler.windowHeight - 64) / 2) - 190;



        Rectangle textureSource = new Rectangle(16, 32, 16, 16);


        chestBounds = new Rectangle(chestPos.X, chestPos.Y, 64, 64);


        int tolerance = 35;

        // Check if the chest is close enough to the center position (within tolerance)
        bool isOverlapping = Math.Abs(chestPos.X - centerX) <= tolerance && Math.Abs(chestPos.Y - centerY) <= tolerance;


        if (currentStage == GameStage.Instructions)
        {
            Console.WriteLine("Drawing Instructions Screen");
            spriteBatch.Draw(
                GameHandler.plainWhiteTexture,
                new Rectangle(0, 0, (int)GameHandler.baseScreenSize.X, (int)GameHandler.baseScreenSize.Y),
                Color.LightPink
            );

            spriteBatch.DrawString(
                GameHandler.highPixel36,
                "Instructions",
                new Vector2(240, 50),
                Color.Black
            );

            spriteBatch.DrawString(
                GameHandler.highPixel22,
                """
                Your name? 
                Cat McPaws.
                Your mission? 
                Move the Chest to the spot.
                Your obstacle? 
                Frogs
                Do not get caught.
                (Use arrow keys to move)
                """,
                new Vector2(100, 150),
                Color.Black
            );

            spriteBatch.Draw(GameHandler.coreTextureAtlas, startButtonBounds, new Rectangle(16, 0, 16, 16), Color.White);
            spriteBatch.DrawString(GameHandler.highPixel22, "Start", new Vector2(350, startButtonPos.Y + 25), Color.Black);
        }
        else if (currentStage == GameStage.Run)
        {

            // Draw background (grass) first
            for (int h = 0; h < 16; h++)
            {
                for (int v = 0; v < 16; v++)
                {
                    spriteBatch.Draw(atlas, new Rectangle(h * 128, v * 128, 128, 128), grass, Color.White, 0f, Vector2.Zero, SpriteEffects.None, 0f);
                }
            }

            for (int i = 0; i < frogPositions.Count; i++)
            {
                Vector2 frogPosition = frogPositions[i];

                SpriteEffects frogDirection = frogMovingRight[i] ? SpriteEffects.None : SpriteEffects.FlipHorizontally;
                Rectangle sourceRect = new Rectangle(currentFrame * frameWidth, 0, frameWidth, frameHeight);
                Rectangle destinationRect = new Rectangle((int)frogPosition.X, (int)frogPosition.Y, (int)(frameWidth * 2f), (int)(frameHeight * 2f));

                spriteBatch.Draw(frog, destinationRect, sourceRect, Color.White, 0f, Vector2.Zero, frogDirection, 0f);
            }

            if (isOverlapping)
            {

                chestPos = new Point(400, 350);

                // Generate a random position within screen bounds
                Random rand = new Random();
                float newX = rand.Next(10, GameHandler.windowWidth - 100);
                float newY = rand.Next(10, GameHandler.windowHeight - 100);

                frogPositions.Add(new Vector2(newX, newY));
                frogMovingRight.Add(rand.Next(2) == 0); // Randomly choose left or right direction

            }
            else
            {
                spriteBatch.Draw(atlas, new Rectangle(centerX, centerY, 64, 64), centerTextureSource, Color.White);
                spriteBatch.Draw(chest, chestBounds, chestRect, Color.White, 0f, Vector2.Zero, SpriteEffects.None, 0f);
            }

            if (isMoving)
            {
                GameHandler.catWalk.DrawFrame(spriteBatch, catPos, faceRight ? SpriteEffects.None : SpriteEffects.FlipHorizontally, 2f);
            }
            else
            {
                GameHandler.catIdle.DrawFrame(spriteBatch, catPos, faceRight ? SpriteEffects.None : SpriteEffects.FlipHorizontally, 2f);
            }

            if (frogPositions.Count == 4)
            {
                currentStage = GameStage.Completion;
            }

        }
        else if (currentStage == GameStage.Completion)
        {
            spriteBatch.Draw(
                GameHandler.plainWhiteTexture,
                new Rectangle(0, 0, (int)GameHandler.baseScreenSize.X, (int)GameHandler.baseScreenSize.Y),
                Color.LightPink
            );

            spriteBatch.DrawString(
                GameHandler.highPixel36,
                "Congratulations!",
                new Vector2(240, 50),
                Color.Black
            );

            spriteBatch.DrawString(
                GameHandler.highPixel22,
                "You have successfully moved the chest!",
                new Vector2(100, 150),
                Color.Black
            );

            spriteBatch.Draw(GameHandler.coreTextureAtlas, startButtonBounds, new Rectangle(16, 0, 16, 16), Color.White);
            spriteBatch.DrawString(GameHandler.highPixel22, "Start", new Vector2(350, startButtonPos.Y + 25), Color.Black);
        }




    }




    public void HandleInput(GameTime gameTime)
    {
        isMoving = false;
        KeyboardState keyboardState = Keyboard.GetState();
        MouseState mouseState = Mouse.GetState();
        float elapsed = (float)gameTime.ElapsedGameTime.TotalSeconds;

        // Define the cat's bounding box
        Rectangle catBounds = new Rectangle((int)catPos.X, (int)catPos.Y, 64, 64);

        int tileSize = 64;

        // Cat movement logic
        if (keyboardState.IsKeyDown(Keys.Left))
        {
            Rectangle newBounds = new Rectangle((int)(catPos.X - 300 * elapsed), (int)catPos.Y, 64, 64);
            if (newBounds.Intersects(chestBounds)) chestPos.X -= tileSize;
            else catPos.X -= 300 * elapsed;

            faceRight = false;
            isMoving = true;
        }
        if (keyboardState.IsKeyDown(Keys.Right))
        {
            Rectangle newBounds = new Rectangle((int)(catPos.X + 300 * elapsed), (int)catPos.Y, 64, 64);
            if (newBounds.Intersects(chestBounds)) chestPos.X += tileSize;
            else catPos.X += 300 * elapsed;

            faceRight = true;
            isMoving = true;
        }
        if (keyboardState.IsKeyDown(Keys.Up))
        {
            Rectangle newBounds = new Rectangle((int)catPos.X, (int)(catPos.Y - 300 * elapsed), 64, 64);
            if (newBounds.Intersects(chestBounds)) chestPos.Y -= tileSize;
            else catPos.Y -= 300 * elapsed;

            isMoving = true;
        }
        if (keyboardState.IsKeyDown(Keys.Down))
        {
            Rectangle newBounds = new Rectangle((int)catPos.X, (int)(catPos.Y + 300 * elapsed), 64, 64);
            if (newBounds.Intersects(chestBounds)) chestPos.Y += tileSize;
            else catPos.Y += 300 * elapsed;

            isMoving = true;
        }

        // Mouse Input for Start Button
        if (currentStage == GameStage.Instructions)
        {
            if (mouseState.LeftButton == ButtonState.Pressed && !mouseDown)
            {
                if (startButtonBounds.Contains(mouseState.Position))
                {
                    currentStage = GameStage.Run;
                    Console.WriteLine("Game Started!");
                }
                mouseDown = true;
            }
            else if (mouseState.LeftButton == ButtonState.Released)
            {
                mouseDown = false;
            }

            
        }

        if (currentStage == GameStage.Completion)
        {
            if (mouseState.LeftButton == ButtonState.Pressed && !mouseDown)
            {
                if (startButtonBounds.Contains(mouseState.Position))
                {
                    currentStage = GameStage.Run;
                    frogPositions.Clear();
                    frogMovingRight.Clear();
                }
                mouseDown = true;
            }
            else if (mouseState.LeftButton == ButtonState.Released)
            {
                mouseDown = false;
            }

            
        }

        // Update chest bounds to match its new position
        chestBounds = new Rectangle(chestPos.X, chestPos.Y, 64, 64);
    }
    public void LoadContent(ContentManager _manager, ContentManager _coreAssets)
    {

        atlas = _manager.Load<Texture2D>("Sprites/petcare_slidetextureatlas");

        frog = _manager.Load<Texture2D>("Sprites/FrogGreen_Hop");

        GameHandler.catIdle.Load(_coreAssets, "Sprites/Animal/idle", 7, 5);
        GameHandler.catWalk.Load(_coreAssets, "Sprites/Animal/walk", 7, 5);
        chest = _manager.Load<Texture2D>("Sprites/treasure_atlas");
        startButton = new Button(GameHandler.coreTextureAtlas, GameHandler.coreTextureAtlas, new Point(250, 72), new Vector2(startButtonPos.X, startButtonPos.Y), "Start", 42, true);

        frameWidth = frog.Width / frameCount;
        frameHeight = frog.Height;

    }

    public void LoadLevel()
    {
        startButtonBounds = new Rectangle(startButtonPos.X, startButtonPos.Y, 250, 72);

        frogPositions.Add(new Vector2(600, 500));
        frogMovingRight.Add(true); // Start moving right
    }

    public void Update(GameTime gameTime)
    {
        if (currentStage == GameStage.Run)
        {
            // Update frog animation
            animationTimer += (float)gameTime.ElapsedGameTime.TotalSeconds;
            if (animationTimer > frameSpeed)
            {
                animationTimer = 0f;
                currentFrame = (currentFrame + 1) % frameCount;
            }

            // Define chest and cat bounds for collision detection
            Rectangle chestBounds = new Rectangle(chestPos.X, chestPos.Y, 64, 64);
            
             Rectangle catBounds = new Rectangle((int)catPos.X, (int)catPos.Y, 64, 64);


            for (int i = 0; i < frogPositions.Count; i++)
            {
                float elapsed = (float)gameTime.ElapsedGameTime.TotalSeconds;
                Vector2 frogPos = frogPositions[i];

                // Define frog bounds for collision detection
                Rectangle frogBounds = new Rectangle((int)frogPos.X, (int)frogPos.Y, frameWidth * 2, frameHeight * 2);

                // Reset cat position if a frog touches the chest
                if (frogBounds.Intersects(catBounds))
                {
                    catPos = new Vector2(300, 305); // Reset to starting position
                    chestPos = new Point(400, 350); // Reset chest position
                }

                // Move the frogs
                if (frogMovingRight[i])
                {
                    frogPositions[i] += new Vector2(frogSpeed * elapsed, 0);
                    if (frogPositions[i].X >= GameHandler.windowWidth - 64) 
                        frogMovingRight[i] = false;
                }
                else
                {
                    frogPositions[i] -= new Vector2(frogSpeed * elapsed, 0);
                    if (frogPositions[i].X <= 0) 
                        frogMovingRight[i] = true;
                }
            }
        }

    }
}