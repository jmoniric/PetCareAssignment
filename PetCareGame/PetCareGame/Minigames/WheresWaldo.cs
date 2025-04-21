using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace PetCareGame
{
    public class WheresWaldo : LevelInterface
    {
        private SpriteFont font;
        private Texture2D WaldoFirstDraft;
        private Texture2D coreTextureAtlas;

        private Rectangle markXRect = new Rectangle(48, 0, 16, 16);      
        private Rectangle checkmarkRect = new Rectangle(0, 16, 16, 16);  

        private bool showCheck = false;
        private bool showX = false;
        private bool gameOver = false;
        private float timer = 0f;
        private int score = 0;
        private int incorrectGuesses = 0;
        private const int maxIncorrectGuesses = 3;
        private bool mouseReleased = true;
        private float timeVisible = 0f;
        private const float timeToDisappear = 5f;
        private float xTimeVisible = 0f;

        private Rectangle waldoBoundingBox;

        public void LoadContent(ContentManager _manager, ContentManager _coreAssets)
        {
            font = _coreAssets.Load<SpriteFont>("Fonts/courier_new_36");
            WaldoFirstDraft = _manager.Load<Texture2D>("Sprites/WaldoFirstDraft");
            coreTextureAtlas = _coreAssets.Load<Texture2D>("Sprites/core_textureatlas");
        }

        public void LoadLevel()
        {
            timer = 0f;
            incorrectGuesses = 0;
            gameOver = false;
            showCheck = false;
            showX = false;
            timeVisible = 0f;
            xTimeVisible = 0f;

            waldoBoundingBox = new Rectangle(728, 545, 70, 40);
        }

        public void UnloadLevel(ContentManager _manager)
        {
            _manager.Unload();
        }

        public void Update(GameTime gameTime)
        {
            if (!gameOver)
            {
                timer += (float)gameTime.ElapsedGameTime.TotalSeconds;
            }

            float mouseX = GameHandler.relativeMousePos.X;
            float mouseY = GameHandler.relativeMousePos.Y;

            MouseState mouseState = Mouse.GetState();

            if (mouseState.LeftButton == ButtonState.Released)
            {
                mouseReleased = true;
            }

            if (mouseState.LeftButton == ButtonState.Pressed && mouseReleased && !gameOver)
            {
                mouseReleased = false;

                if (waldoBoundingBox.Contains((int)mouseX, (int)mouseY))
                {
                    showCheck = true;
                    showX = false;
                    gameOver = true;
                    score++;
                    timeVisible = timeToDisappear;
                }
                else
                {
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

                    if (incorrectGuesses >= maxIncorrectGuesses)
                    {
                        gameOver = true;
                    }
                }
            }

            if (timeVisible > 0)
            {
                timeVisible -= (float)gameTime.ElapsedGameTime.TotalSeconds;
            }
            else
            {
                showCheck = false;
            }

            if (xTimeVisible > 0)
            {
                xTimeVisible -= (float)gameTime.ElapsedGameTime.TotalSeconds;
            }
            else
            {
                showX = false;
            }
        }

        public void HandleInput(GameTime gameTime)
        {
            if (gameOver && Keyboard.GetState().IsKeyDown(Keys.Space))
            {
                LoadLevel();
            }
        }

        public void Draw(GameTime gameTime, SpriteBatch spriteBatch, GraphicsDeviceManager _graphics)
        {
            _graphics.GraphicsDevice.Clear(Color.CornflowerBlue);

            float screenWidth = _graphics.PreferredBackBufferWidth;
            float screenHeight = _graphics.PreferredBackBufferHeight;

            if (WaldoFirstDraft != null)
            {
                float scale = Math.Min(screenWidth / (float)WaldoFirstDraft.Width, screenHeight / (float)WaldoFirstDraft.Height);
                Vector2 position = new Vector2((screenWidth - WaldoFirstDraft.Width * scale) / 2, (screenHeight - WaldoFirstDraft.Height * scale) / 2);
                spriteBatch.Draw(WaldoFirstDraft, position, null, Color.White, 0f, Vector2.Zero, scale, SpriteEffects.None, 0f);
            }
            else
            {
                spriteBatch.DrawString(font, "Error: Waldo Image Not Loaded", new Vector2(10, 50), Color.Red);
            }

            string incorrectGuessesText = $"Incorrect Guesses: {incorrectGuesses}/{maxIncorrectGuesses}";
            Vector2 incorrectGuessesSize = font.MeasureString(incorrectGuessesText);
            Rectangle incorrectGuessesRect = new Rectangle(0, 0, (int)screenWidth, (int)(incorrectGuessesSize.Y + 10));
            spriteBatch.Draw(GameHandler.plainWhiteTexture, incorrectGuessesRect, Color.Gray);
            spriteBatch.DrawString(font, incorrectGuessesText, new Vector2(10, 5), Color.Black);

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

            if (gameOver)
            {
                string message = incorrectGuesses >= maxIncorrectGuesses ? "Game Over! Too many incorrect guesses." : "You found Waldo!";
                string restartMsg = "Press Space to Restart";

                // Make the font smaller for the game over message
                float scaleFactor = 0.5f; // Scale factor to make the text smaller

                Vector2 messageSize = font.MeasureString(message) * scaleFactor;
                Vector2 restartSize = font.MeasureString(restartMsg) * scaleFactor;

                Vector2 messagePosition = new Vector2((screenWidth - messageSize.X) / 2, screenHeight / 2 - 50);
                Vector2 restartPosition = new Vector2((screenWidth - restartSize.X) / 2, screenHeight / 2 + 12);

                // Draw smaller background rectangles for the messages
                spriteBatch.Draw(GameHandler.plainWhiteTexture, new Rectangle((int)messagePosition.X - 10, (int)messagePosition.Y - 5, (int)(messageSize.X + 20), (int)(messageSize.Y + 10)), Color.Gray);
                spriteBatch.DrawString(font, message, messagePosition, Color.Black, 0f, Vector2.Zero, scaleFactor, SpriteEffects.None, 0f);

                spriteBatch.Draw(GameHandler.plainWhiteTexture, new Rectangle((int)restartPosition.X - 10, (int)restartPosition.Y - 5, (int)(restartSize.X + 20), (int)(restartSize.Y + 10)), Color.Gray);
                spriteBatch.DrawString(font, restartMsg, restartPosition, Color.Black, 0f, Vector2.Zero, scaleFactor, SpriteEffects.None, 0f);
            }
        }

        public void CleanupProcesses() { }

        public void Dispose() { }

        public void SaveData() { }

        public void LoadData() { }
    }
}