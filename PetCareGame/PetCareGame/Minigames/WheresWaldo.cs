/** 
@author: Ethan
*/
using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Media;

namespace PetCareGame
{
    public class WheresWaldo : LevelInterface
    {
        // Font and textures
        private SpriteFont font;
        private Texture2D[] waldoImages; // Array of images per level
        private Texture2D coreTextureAtlas; // Contains checkmark and X sprites

        // Rectangles for checkmark and X icons
        private Rectangle markXRect = new Rectangle(48, 0, 16, 16);
        private Rectangle checkmarkRect = new Rectangle(0, 16, 16, 16);

        // Game state flags
        private bool showCheck = false;
        private bool showX = false;
        private bool gameOver = false;
        private bool mouseReleased = true;

        // Timers and scoring
        private float timer = 0f;
        private float timeVisible = 0f; // How long to show check
        private float xTimeVisible = 0f; // How long to show X
        private const float timeToDisappear = 5f;
        private int score = 0;
        private int incorrectGuesses = 0;
        private const int maxIncorrectGuesses = 3;

        private Rectangle waldoBoundingBox; // Defines Waldo's location for each level

        private int currentLevel = 1;
        private const int maxLevels = 3;

 
        private Song backgroundMusic;
        private SoundEffect correctSound;
        private SoundEffect wrongSound;
        private bool musicPlaying = false;

    
        public void LoadContent(ContentManager _manager, ContentManager _coreAssets)
        {
            font = _coreAssets.Load<SpriteFont>("Fonts/courier_new_36");
            coreTextureAtlas = _coreAssets.Load<Texture2D>("Sprites/core_textureatlas");

            waldoImages = new Texture2D[maxLevels + 1];
            waldoImages[1] = _manager.Load<Texture2D>("Sprites/WaldoFirstDraft");
            waldoImages[2] = _manager.Load<Texture2D>("Sprites/Waldo2");
            waldoImages[3] = _manager.Load<Texture2D>("Sprites/Waldo3");

            backgroundMusic = _manager.Load<Song>("Sounds/Song");
            correctSound = _manager.Load<SoundEffect>("Sounds/Correct");
            wrongSound = _manager.Load<SoundEffect>("Sounds/Wrong");
        }

        // Reset and initialize values when level is loaded
        public void LoadLevel()
        {
            timer = 0f;
            incorrectGuesses = 0;
            gameOver = false;
            showCheck = false;
            showX = false;
            timeVisible = 0f;
            xTimeVisible = 0f;
            mouseReleased = false;

            // Start background music 
            if (!musicPlaying)
            {
                MediaPlayer.IsRepeating = true;
                MediaPlayer.Play(backgroundMusic);
                musicPlaying = true;
            }

            // Define Waldo's clickable area per level
            switch (currentLevel)
            {
                case 1:
                    waldoBoundingBox = new Rectangle(728, 545, 70, 40);
                    break;
                case 2:
                    waldoBoundingBox = new Rectangle(410, 295, 33, 25);
                    break;
                case 3:
                    waldoBoundingBox = new Rectangle(404, 73, 25, 26);
                    break;
                default:
                    // If no more levels, return to overworld
                    MediaPlayer.Stop();
                    musicPlaying = false;
                    GameHandler.CurrentState = GameHandler.GameState.Overworld;
                    currentLevel = 1;
                    return;
            }
        }

        public void UnloadLevel(ContentManager _manager)
        {
            _manager.Unload();
        }

        // Main update loop - handles timer, mouse clicks, guess checking
        public void Update(GameTime gameTime)
        {
            if (!gameOver)
            {
                timer += (float)gameTime.ElapsedGameTime.TotalSeconds;
            }

            float mouseX = GameHandler.relativeMousePos.X;
            float mouseY = GameHandler.relativeMousePos.Y;
            MouseState mouseState = Mouse.GetState();

            // Detect mouse release to prevent holding click
            if (mouseState.LeftButton == ButtonState.Released)
            {
                mouseReleased = true;
            }

        
            if (mouseState.LeftButton == ButtonState.Pressed && mouseReleased && !gameOver)
            {
                mouseReleased = false;

                // Player found Waldo
                if (waldoBoundingBox.Contains((int)mouseX, (int)mouseY))
                {
                    correctSound.Play(); 
                    showCheck = true;
                    showX = false;
                    gameOver = true;
                    score++;
                    timeVisible = timeToDisappear;
                }
                else // Player guessed incorrectly
                {
                    wrongSound.Play(); 
                    showCheck = false;

                    if (incorrectGuesses < maxIncorrectGuesses - 1)
                    {
                        showX = true;
                        xTimeVisible = timeToDisappear;
                    }
                    else
                    {
                        showX = false;
                    }

                    incorrectGuesses++;

                    // End game if too many wrong guesses
                    if (incorrectGuesses >= maxIncorrectGuesses)
                    {
                        gameOver = true;
                    }
                }
            }

            // Hide checkmark after timeout
            if (timeVisible > 0)
                timeVisible -= (float)gameTime.ElapsedGameTime.TotalSeconds;
            else
                showCheck = false;

            // Hide red X after timeout
            if (xTimeVisible > 0)
                xTimeVisible -= (float)gameTime.ElapsedGameTime.TotalSeconds;
            else
                showX = false;
        }

        
        public void HandleInput(GameTime gameTime)
        {
            if (gameOver && Keyboard.GetState().IsKeyDown(Keys.Space))
            {
                if (incorrectGuesses >= maxIncorrectGuesses)
                {
                    currentLevel = 1;
                    LoadLevel(); // Restart
                }
                else
                {
                    if (currentLevel < maxLevels)
                    {
                        currentLevel++;
                        LoadLevel(); // Next level
                    }
                    else
                    {
                        // Game finished - return to overworld
                        MediaPlayer.Stop();
                        musicPlaying = false;
                        GameHandler.CurrentState = GameHandler.GameState.Overworld;
                        currentLevel = 1;
                        GameHandler.saveFile.WheresWaldoDone = true;
                        GameHandler.UnloadCurrentLevel();
                        GameHandler.LoadOverworld();
                    }
                }
            }
        }

        // Render everything on screen
        public void Draw(GameTime gameTime, SpriteBatch spriteBatch, GraphicsDeviceManager _graphics)
        {
            _graphics.GraphicsDevice.Clear(Color.CornflowerBlue);

            float screenWidth = _graphics.PreferredBackBufferWidth;
            float screenHeight = _graphics.PreferredBackBufferHeight;

            // Draw Waldo image scaled and centered
            if (waldoImages[currentLevel] != null)
            {
                float scale = Math.Min(screenWidth / (float)waldoImages[currentLevel].Width, screenHeight / (float)waldoImages[currentLevel].Height);
                Vector2 position = new Vector2((screenWidth - waldoImages[currentLevel].Width * scale) / 2, (screenHeight - waldoImages[currentLevel].Height * scale) / 2);
                spriteBatch.Draw(waldoImages[currentLevel], position, null, Color.White, 0f, Vector2.Zero, scale, SpriteEffects.None, 0f);
            }
            else
            {
                spriteBatch.DrawString(font, "Error: Waldo Image Not Loaded", new Vector2(10, 50), Color.Red);
            }

            // Draw incorrect guesses counter
            string incorrectGuessesText = $"Incorrect Guesses: {incorrectGuesses}/{maxIncorrectGuesses}";
            Vector2 incorrectGuessesSize = font.MeasureString(incorrectGuessesText);
            Rectangle incorrectGuessesRect = new Rectangle(0, 0, (int)screenWidth, (int)(incorrectGuessesSize.Y + 10));
            spriteBatch.Draw(GameHandler.plainWhiteTexture, incorrectGuessesRect, Color.Gray);
            spriteBatch.DrawString(font, incorrectGuessesText, new Vector2(10, 5), Color.Black);

            // Draw either checkmark or red X icon
            if (showCheck || showX)
            {
                Rectangle sourceRect = showCheck ? checkmarkRect : markXRect;
                Color tint = showCheck ? Color.LimeGreen : Color.Red;

                float iconScale = 4f;
                float iconWidth = sourceRect.Width * iconScale;
                float iconHeight = sourceRect.Height * iconScale;

                Vector2 iconPosition = new Vector2(
                    (screenWidth - iconWidth) / 2,
                    screenHeight / 2 + 80
                );

                spriteBatch.Draw(coreTextureAtlas, iconPosition, sourceRect, tint, 0f, Vector2.Zero, iconScale, SpriteEffects.None, 0f);
            }

            // Draw end-of-level message
            if (gameOver)
            {
                string message = incorrectGuesses >= maxIncorrectGuesses ? "Game Over! Too many incorrect guesses." : "You found Waldo!";
                string restartMsg = "Press Space to Continue";

                float scaleFactor = 0.5f;

                Vector2 messageSize = font.MeasureString(message) * scaleFactor;
                Vector2 restartSize = font.MeasureString(restartMsg) * scaleFactor;

                Vector2 messagePosition = new Vector2((screenWidth - messageSize.X) / 2, screenHeight / 2 - 50);
                Vector2 restartPosition = new Vector2((screenWidth - restartSize.X) / 2, screenHeight / 2 + 12);

                spriteBatch.Draw(GameHandler.plainWhiteTexture, new Rectangle((int)messagePosition.X - 10, (int)messagePosition.Y - 5, (int)(messageSize.X + 20), (int)(messageSize.Y + 10)), Color.Gray);
                spriteBatch.DrawString(font, message, messagePosition, Color.Black, 0f, Vector2.Zero, scaleFactor, SpriteEffects.None, 0f);

                spriteBatch.Draw(GameHandler.plainWhiteTexture, new Rectangle((int)restartPosition.X - 10, (int)restartPosition.Y - 5, (int)(restartSize.X + 20), (int)(restartSize.Y + 10)), Color.Gray);
                spriteBatch.DrawString(font, restartMsg, restartPosition, Color.Black, 0f, Vector2.Zero, scaleFactor, SpriteEffects.None, 0f);
            }
        }

       
        public void CleanupProcesses() { }
        public void Dispose() { }
        public void SaveData(SaveFile saveFile) { }
        public void LoadData() { }
    }
}
