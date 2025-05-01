using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Audio;

namespace PetCareGame;

public class SlidingGame : LevelInterface
{
    //Texture and Sprites
    private Color backgroundColour = new Color(144, 238, 144);
    private Texture2D atlas;

    private Texture2D chest;

    private Texture2D frog;

    private Texture2D fruit;

    private SoundEffectInstance peacefulTrack;

    private SoundEffectInstance fasterTrack;

    private SoundEffectInstance finalTrack;


    //Asset positions and Bounds
    private Rectangle chestBounds;
    private Point chestPos = new Point(400, 350);

    private Vector2 catPos = new Vector2(200, 150);

    private Button startButton;
    private Point startButtonPos = new Point(270, 510);
    private Rectangle startButtonBounds;

    private Rectangle fruitBounds;
    private Point goalTilePos;

    private Point fruitPos = new Point(270, 610);
    // Animation variables
    private int frameWidth;
    private int frameHeight;
    private int frameCount = 7;
    private float frameSpeed = 0.1f;
    private float animationTimer = 0f;
    private int currentFrame = 0;
    private bool faceRight = true;
    private bool isMoving;

    //allows devs to skip game for debugging purposes
    private bool debugSkipGame = false;


    //Frog Logic

    private List<Vector2> frogPositions = new List<Vector2>();
    private List<bool> frogMovingRight = new List<bool>();
    private float frogSpeed = 150f;

    //Game Stages variables
    enum GameStage
    {
        Instructions,
        Run,

        Run2,

        Run3,

        Completion,

        gameOver
    }
    //Initial Game Stage variables
    private GameStage currentStage = GameStage.Instructions;
    private int lives = 5;
    private bool mouseDown = false;

    private bool slidingGoal = false;

    private bool slidingGoalGlobal = false;

    private int stagesCompleted = 0;

    // Transition screen control
    private bool showTransitionScreen = false;
    private float transitionTimer = 0f;
    private string transitionText = "";


    public void CleanupProcesses()
    {

        catPos = new Vector2(400, 305);

        frogPositions.Clear();
        frogMovingRight.Clear();

        mouseDown = false;
        faceRight = true;

        currentStage = GameStage.Instructions;
        startButtonPos = new Point(270, 510);
        lives = 5;
        stagesCompleted = 0;
        
        chestPos = new Point(400, 350);
        
        //Frog Logic
        frogPositions.Add(new Vector2(300, 150));
        new Vector2(200, 150);

        frogMovingRight.Add(true); // Start moving right
    }

    public void Dispose()
    {

    }



    public void Draw(GameTime gameTime, SpriteBatch spriteBatch, GraphicsDeviceManager _graphics)
    {
        //Draw transitions screens if active
        if (showTransitionScreen)
        {
            spriteBatch.Draw(
                GameHandler.plainWhiteTexture,
                new Rectangle(0, 0, (int)GameHandler.baseScreenSize.X, (int)GameHandler.baseScreenSize.Y),
                Color.Black * 0.8f
            );

            spriteBatch.DrawString(
                GameHandler.highPixel36,
                transitionText,
                new Vector2(100, 150),
                Color.White
            );

            return; // Skip drawing the rest of the game if transition is active
        }
        // Draw the game background and elements
        Rectangle grass = new Rectangle(16, 0, 16, 16);
        Rectangle chestRect = new Rectangle(0, 0, 32, 32);
        _graphics.GraphicsDevice.Clear(backgroundColour);


        Rectangle centerTextureSource = new Rectangle(32, 16, 16, 16);
        Rectangle openchestTextureSource = new Rectangle(0, 32, 32, 32);



        int objectiveX = goalTilePos.X;
        int objectiveY = goalTilePos.Y;



        Rectangle textureSource = new Rectangle(16, 32, 16, 16);

        Rectangle flowersRect = new Rectangle(0, 16, 16, 16);


        chestBounds = new Rectangle(chestPos.X, chestPos.Y, 64, 64);

        //tolerance for the chest in the goal position
        int tolerance = 35;

        // Check if the chest is close enough to the center position (within tolerance)
        bool isOverlapping = Math.Abs(chestPos.X - objectiveX) <= tolerance && Math.Abs(chestPos.Y - objectiveY) <= tolerance;


        if (currentStage == GameStage.Instructions)
        {
            Console.WriteLine("Drawing Instructions Screen");
            spriteBatch.Draw(
                GameHandler.plainWhiteTexture,
                new Rectangle(0, 0, (int)GameHandler.baseScreenSize.X, (int)GameHandler.baseScreenSize.Y),
                backgroundColour
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
                (Use arrow keys or WASD to move)
                (R to reset if you get stuck)
                """,
                new Vector2(100, 150),
                Color.Black
            );

            spriteBatch.Draw(GameHandler.coreTextureAtlas, startButtonBounds, new Rectangle(16, 0, 16, 16), Color.White);
            spriteBatch.DrawString(GameHandler.highPixel22, "Start", new Vector2(350, startButtonPos.Y + 25), Color.Black);
        }
        else if (currentStage == GameStage.Run || currentStage == GameStage.Run2 || currentStage == GameStage.Run3)
        {

            // Draw background (grass) first
            for (int h = 0; h < 16; h++)
            {
                for (int v = 0; v < 16; v++)
                {
                    // Compute screen position for the tile
                    int x = h * 64;
                    int y = v * 64;

                    // Draw the grass first
                    spriteBatch.Draw(atlas, new Rectangle(x, y, 64, 64), grass, Color.White);

                    // Draw overlay tile on every other tile (checkerboard pattern)
                    if ((h + v) % 2 == 0) // Alternating pattern
                    {
                        spriteBatch.Draw(atlas, new Rectangle(x, y, 64, 64), flowersRect, Color.White);
                    }
                }

            }
            spriteBatch.DrawString(GameHandler.highPixel22, "Lives: " + lives, new Vector2(0, 50), Color.Black);

            //if chest is overlapping the goal position
            if (isOverlapping)
            {

                chestPos = new Point(400, 350);

                // Generate a random position within screen bounds
                Random rand = new Random();
                int tileSize = 64;
                float newX = rand.Next(10, GameHandler.windowWidth - 100);
                float newY = rand.Next(10, GameHandler.windowHeight - 100);

                goalTilePos = new Point(
                rand.Next(1, (GameHandler.windowWidth / tileSize) - 1) * tileSize,
                rand.Next(1, (GameHandler.windowHeight / tileSize) - 1) * tileSize
   );

                frogPositions.Add(new Vector2(newX, newY));
                frogMovingRight.Add(rand.Next(2) == 0); // Randomly choose left or right direction
                stagesCompleted++;


            }
            else
            {
                spriteBatch.Draw(atlas, new Rectangle(objectiveX, objectiveY, 64, 64), centerTextureSource, Color.White);
                spriteBatch.Draw(chest, chestBounds, chestRect, Color.White, 0f, Vector2.Zero, SpriteEffects.None, 0f);
            }


            // Draw the frogs
            for (int i = 0; i < frogPositions.Count; i++)
            {
                Vector2 frogPosition = frogPositions[i];

                SpriteEffects frogDirection = frogMovingRight[i] ? SpriteEffects.None : SpriteEffects.FlipHorizontally;
                Rectangle sourceRect = new Rectangle(currentFrame * frameWidth, 0, frameWidth, frameHeight);
                Rectangle destinationRect = new Rectangle((int)frogPosition.X, (int)frogPosition.Y, (int)(frameWidth * 2f), (int)(frameHeight * 2f));

                spriteBatch.Draw(frog, destinationRect, sourceRect, Color.White, 0f, Vector2.Zero, frogDirection, 0f);
            }

            // Draw the cat orientation
            if (isMoving)
            {
                GameHandler.catWalk.DrawFrame(spriteBatch, catPos, faceRight ? SpriteEffects.None : SpriteEffects.FlipHorizontally, 2f);
            }
            else
            {
                GameHandler.catIdle.DrawFrame(spriteBatch, catPos, faceRight ? SpriteEffects.None : SpriteEffects.FlipHorizontally, 2f);
            }


            //Handle game over
            if (lives <= 0)
            {
                currentStage = GameStage.gameOver;
            }

            //Handle stage transitions
            if (stagesCompleted == 4 && currentStage == GameStage.Run)
            {
                showTransitionScreen = true;
                transitionTimer = 3f;
                transitionText = "Stage 2!";
                currentStage = GameStage.Run2;
            }
            if (stagesCompleted == 7 && currentStage == GameStage.Run2)
            {
                showTransitionScreen = true;
                transitionTimer = 3f;
                transitionText = "Stage 3!";
                currentStage = GameStage.Run3;
            }


            if (stagesCompleted == 9)
            {
                GameHandler.saveFile.SlidingGameDone = true;
                GameHandler.saveFile.Save(GameHandler.saveFile);
                
                currentStage = GameStage.Completion;
            }

            if (slidingGoal)
            {
                spriteBatch.Draw(fruit, fruitBounds, openchestTextureSource, Color.White);
                spriteBatch.DrawString(GameHandler.highPixel22, "You have moved the chest!", new Vector2(100, 150), Color.Black);
                spriteBatch.DrawString(GameHandler.highPixel22, "Click to continue", new Vector2(100, 200), Color.Black);
                slidingGoal = false;
            }

        }


        else if (currentStage == GameStage.gameOver)
        {
            spriteBatch.Draw(
                GameHandler.plainWhiteTexture,
                new Rectangle(0, 0, (int)GameHandler.baseScreenSize.X, (int)GameHandler.baseScreenSize.Y),
                backgroundColour
            );

            spriteBatch.DrawString(
                GameHandler.highPixel36,
                "Game Over",
                new Vector2(240, 50),
                Color.Black
            );

            spriteBatch.DrawString(
                GameHandler.highPixel22,
                "You have been caught by the frogs!",
                new Vector2(100, 150),
                Color.Black
            );

            spriteBatch.Draw(GameHandler.coreTextureAtlas, startButtonBounds, new Rectangle(16, 0, 16, 16), Color.White);
            spriteBatch.DrawString(GameHandler.highPixel22, "Start", new Vector2(350, startButtonPos.Y + 25), Color.Black);
        }
        else if (currentStage == GameStage.Completion)
        {
            spriteBatch.Draw(
                GameHandler.plainWhiteTexture,
                new Rectangle(0, 0, (int)GameHandler.baseScreenSize.X, (int)GameHandler.baseScreenSize.Y),
                backgroundColour
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

            spriteBatch.Draw(fruit, fruitBounds, new Rectangle(16, 0, 16, 16), Color.White);

            spriteBatch.Draw(GameHandler.coreTextureAtlas, startButtonBounds, new Rectangle(16, 0, 16, 16), Color.White);
            spriteBatch.DrawString(GameHandler.highPixel22, "Return", new Vector2(350, startButtonPos.Y + 25), Color.Black);
        }




    }




    public void HandleInput(GameTime gameTime)
    {
        if(!GameHandler.isPaused) {
            isMoving = false;

            KeyboardState keyboardState = Keyboard.GetState();

            float elapsed = (float)gameTime.ElapsedGameTime.TotalSeconds;
            Rectangle catBounds = new Rectangle((int)catPos.X, (int)catPos.Y, 32, 32);
            int tileSize = 64;

            // Cat movement logic
            if (keyboardState.IsKeyDown(Keys.Left) || keyboardState.IsKeyDown(Keys.A))
            {
                Rectangle newBounds = new Rectangle((int)(catPos.X - 300 * elapsed), (int)catPos.Y, 64, 64);
                if (newBounds.Intersects(chestBounds)) chestPos.X -= tileSize;
                else catPos.X -= 300 * elapsed;

                faceRight = false;
                isMoving = true;
            }
            if (keyboardState.IsKeyDown(Keys.Right) || keyboardState.IsKeyDown(Keys.D))
            {
                Rectangle newBounds = new Rectangle((int)(catPos.X + 300 * elapsed), (int)catPos.Y, 64, 64);
                if (newBounds.Intersects(chestBounds)) chestPos.X += tileSize;
                else catPos.X += 300 * elapsed;

                faceRight = true;
                isMoving = true;
            }
            if (keyboardState.IsKeyDown(Keys.Up) || keyboardState.IsKeyDown(Keys.W))
            {
                Rectangle newBounds = new Rectangle((int)catPos.X, (int)(catPos.Y - 300 * elapsed), 64, 64);
                if (newBounds.Intersects(chestBounds)) chestPos.Y -= tileSize;
                else catPos.Y -= 300 * elapsed;

                isMoving = true;
            }
            if (keyboardState.IsKeyDown(Keys.Down) || keyboardState.IsKeyDown(Keys.S))
            {
                Rectangle newBounds = new Rectangle((int)catPos.X, (int)(catPos.Y + 300 * elapsed), 64, 64);
                if (newBounds.Intersects(chestBounds)) chestPos.Y += tileSize;
                else catPos.Y += 300 * elapsed;

                isMoving = true;
            }

            //In case of chest softlock

            if (keyboardState.IsKeyDown(Keys.R))
            {
                catPos = new Vector2(300, 305); // Reset to starting position
                chestPos = new Point(400, 350); // Reset chest position
            }

            // Mouse Input for Start Button
            if (currentStage == GameStage.Instructions)
            {
                if (GameHandler.mouseState.LeftButton == ButtonState.Pressed && !mouseDown)
                {
                    if (startButtonBounds.Contains(GameHandler.mouseState.Position))
                    {
                        currentStage = GameStage.Run;
                        Console.WriteLine("Game Started!");
                    }
                    mouseDown = true;
                }
                else if (GameHandler.mouseState.LeftButton == ButtonState.Released)
                {
                    mouseDown = false;
                }


            }

            // Mouse Input for Game Over and Completion Screens
            // Game Over Screen
            if (currentStage == GameStage.gameOver)
            {

                if (GameHandler.mouseState.LeftButton == ButtonState.Pressed && !mouseDown)
                {
                    if (startButtonBounds.Contains(GameHandler.mouseState.Position))
                    {
                        currentStage = GameStage.Instructions;
                        frogPositions.Clear();
                        frogMovingRight.Clear();
                        lives = 5;
                        stagesCompleted = 0;
                    }
                    mouseDown = true;
                }
                else if (GameHandler.mouseState.LeftButton == ButtonState.Released)
                {
                    mouseDown = false;
                }
            }

            if (currentStage == GameStage.Completion)
            {
                if (GameHandler.mouseState.LeftButton == ButtonState.Pressed && !mouseDown)
                {
                    if (startButtonBounds.Contains(GameHandler.mouseState.Position))
                    {
            
                        frogPositions.Clear();
                        frogMovingRight.Clear();
                        lives = 5;
                        stagesCompleted = 0;

                        //sets save file bool for this game to be true
                        GameHandler.saveFile.SlidingGameDone = true;
                        //unloads assets this game is using
                        GameHandler.UnloadCurrentLevel();
                        GameHandler.LoadOverworld();

                    }
                    mouseDown = true;
                }
                else if (GameHandler.mouseState.LeftButton == ButtonState.Released)
                {
                    mouseDown = false;
                }
            }

            // Ensure the cat and chest positions are within the screen bounds
            catPos.X = MathHelper.Clamp(catPos.X, 0, GameHandler.windowWidth - tileSize);
            catPos.Y = MathHelper.Clamp(catPos.Y, 0, GameHandler.windowHeight - tileSize);

            chestPos.X = MathHelper.Clamp(chestPos.X, 0, GameHandler.windowWidth - tileSize);
            chestPos.Y = MathHelper.Clamp(chestPos.Y, 0, GameHandler.windowHeight - tileSize);

            // Update chest bounds to match its new position
            chestBounds = new Rectangle(chestPos.X, chestPos.Y, 64, 64);
        }
    }
    public void LoadContent(ContentManager _manager, ContentManager _coreAssets)
    {
        //Sprite load
        atlas = _manager.Load<Texture2D>("Sprites/petcare_slidetextureatlas");
        frog = _manager.Load<Texture2D>("Sprites/FrogGreen_Hop");
        chest = _manager.Load<Texture2D>("Sprites/treasure_atlas");
        fruit = _manager.Load<Texture2D>("Sprites/fruit");
        startButton = new Button(GameHandler.coreTextureAtlas, GameHandler.coreTextureAtlas, new Point(250, 72), new Vector2(startButtonPos.X, startButtonPos.Y), "Start", 42, true);

        // Load the cat animations
        frameWidth = frog.Width / frameCount;
        frameHeight = frog.Height;

        // Load the tracks
        peacefulTrack = _manager.Load<SoundEffect>("Sounds/peacefulTrack").CreateInstance();
        fasterTrack = _manager.Load<SoundEffect>("Sounds/fasterTrack").CreateInstance();
        finalTrack = _manager.Load<SoundEffect>("Sounds/finalTrack").CreateInstance();

    }

    public void LoadLevel()
    {
        //Start Button
        startButtonBounds = new Rectangle(startButtonPos.X, startButtonPos.Y, 250, 72);

        fruitBounds = new Rectangle(fruitPos.X, fruitPos.Y, 250, 172);

        //Goal position
        int tileSize = 64;
        int tilesX = GameHandler.windowWidth / tileSize;
        int tilesY = GameHandler.windowHeight / tileSize;

        Random rand = new Random();
        Point chosenTile;

        do
        {
            int x = rand.Next(tilesX);
            int y = rand.Next(tilesY);
            chosenTile = new Point(x, y);
        } while ((chosenTile.X + chosenTile.Y) % 2 != 0); // Make sure it lands on a checkerboard tile

        goalTilePos = new Point(chosenTile.X * tileSize, chosenTile.Y * tileSize);



        //Frog Logic
        frogPositions.Add(new Vector2(300, 150));
        new Vector2(200, 150);

        frogMovingRight.Add(true); // Start moving right


    }

    public void Update(GameTime gameTime)
    {
        //allows devs to skip game for debugging purposes
        if(debugSkipGame) {
            currentStage = GameStage.Completion;
        }


        //Show transition screen if active
        if (showTransitionScreen)
        {
            transitionTimer -= (float)gameTime.ElapsedGameTime.TotalSeconds;
            if (transitionTimer <= 0f)
            {
                showTransitionScreen = false;
            }
            return; // Skip update logic while transition screen is active
        }

        //this is your music handling code
        if (GameHandler.allowAudio)
        {
            //music not muted, game stage is run (as you specified), and game is not paused
            if (!GameHandler.musicMuted && currentStage == GameStage.Run && !GameHandler.isPaused)
            {
                fasterTrack.IsLooped = false;
                fasterTrack.Stop(true);
                peacefulTrack.IsLooped = true;
                peacefulTrack.Play();
            }
            else if (!GameHandler.musicMuted && currentStage == GameStage.Run2 && !GameHandler.isPaused)
            {
                
                peacefulTrack.IsLooped = false;
                peacefulTrack.Stop(true);
                fasterTrack.IsLooped = true;
                fasterTrack.Volume = 0.5f;
                fasterTrack.Play();
            }
            else if (!GameHandler.musicMuted && currentStage == GameStage.Run3 && !GameHandler.isPaused)
            {
                fasterTrack.IsLooped = false;
                fasterTrack.Stop(true);
                finalTrack.IsLooped = true;
                finalTrack.Play();
            }
            else //otherwise, pause soundtrack
            {
                peacefulTrack.Pause();
            }


            if (currentStage == GameStage.Completion)
            {
                finalTrack.IsLooped = false;
                finalTrack.Stop();
                slidingGoal = true;
                slidingGoalGlobal = true;
            }
        }
        if(!GameHandler.isPaused) {
            // Handle input for the game stage transitions
            if (currentStage == GameStage.Run || currentStage == GameStage.Run2 || currentStage == GameStage.Run3)
            {
                // Check for mouse input on the start button
                if (GameHandler.mouseState.LeftButton == ButtonState.Pressed && !mouseDown)
                {
                    if (startButtonBounds.Contains(GameHandler.mouseState.Position))
                    {
                        currentStage = GameStage.Instructions;
                        frogPositions.Clear();
                        frogMovingRight.Clear();
                        lives = 5;
                    }
                    mouseDown = true;
                }
                else if (GameHandler.mouseState.LeftButton == ButtonState.Released)
                {
                    mouseDown = false;
                }
            }
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

                if (isMoving)
                {
                    GameHandler.catWalk.UpdateFrame((float)gameTime.ElapsedGameTime.TotalSeconds);
                }
                else
                {
                    GameHandler.catIdle.UpdateFrame((float)gameTime.ElapsedGameTime.TotalSeconds);
                }


                for (int i = frogPositions.Count - 1; i >= 0; i--) // Loop backwards to safely remove
                {
                    float elapsed = (float)gameTime.ElapsedGameTime.TotalSeconds;
                    Vector2 frogPos = frogPositions[i];

                    // Define frog bounds
                    Rectangle frogBounds = new Rectangle((int)frogPos.X, (int)frogPos.Y, 64, 64);

                    // Check for collision with cat
                    if (frogBounds.Intersects(catBounds))
                    {
                        lives--;
                        frogPositions.RemoveAt(i);
                        frogMovingRight.RemoveAt(i);
                        continue;
                    }

                    // Move frog
                    float moveAmount = frogSpeed * elapsed;
                    if (frogMovingRight[i])
                    {
                        frogPos.X += moveAmount;
                        if (frogPos.X > GameHandler.windowWidth - 64) frogMovingRight[i] = false;
                    }
                    else
                    {
                        frogPos.X -= moveAmount;
                        if (frogPos.X < 0) frogMovingRight[i] = true;
                    }

                    frogPositions[i] = frogPos;
                }
            }
        }
    }



    public void SaveData(SaveFile saveFile)
    {
        saveFile.SlidingGameDone = slidingGoalGlobal;
    }

    public void LoadData()
    {

    }


}