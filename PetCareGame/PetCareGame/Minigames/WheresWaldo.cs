using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace PetCareGame
{
    public class WheresWaldo : LevelInterface
    {
        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;
        private SpriteFont font;
        private Texture2D WaldoFirstDraft;
        private Texture2D greenCheck;
        private Texture2D redX;

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

        private Rectangle waldoBoundingBox;
        private Vector2 checkPosition;
        private Vector2 xPosition;

        public void LoadContent(ContentManager _manager, ContentManager _coreAssets)
        {
            font = _coreAssets.Load<SpriteFont>("Fonts/courier_new_36");
            WaldoFirstDraft = _manager.Load<Texture2D>("Sprites/WaldoFirstDraft");
            greenCheck = _manager.Load<Texture2D>("Sprites/GreenCheck");
            redX = _manager.Load<Texture2D>("Sprites/RedX");
        }

        public void LoadLevel()
        {
            timer = 0f;
            incorrectGuesses = 0;
            gameOver = false;
            showCheck = false;
            showX = false;
            timeVisible = 0f;
            waldoBoundingBox = new Rectangle(735, 570, 45, 20);
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

            MouseState mouseState = Mouse.GetState();
            int mouseX = mouseState.X;
            int mouseY = mouseState.Y;

            if (mouseState.LeftButton == ButtonState.Released)
            {
                mouseReleased = true;
            }

            if (mouseState.LeftButton == ButtonState.Pressed && mouseReleased && !gameOver)
            {
                mouseReleased = false;
                if (waldoBoundingBox.Contains(mouseX, mouseY))
                {
                    showCheck = true;
                    showX = false;
                    checkPosition = new Vector2(mouseX, mouseY);
                    gameOver = true;
                    score++;
                    timeVisible = timeToDisappear;
                }
                else
                {
                    showCheck = false;
                    showX = true;
                    xPosition = new Vector2(mouseX, mouseY);
                    incorrectGuesses++;
                    if (incorrectGuesses >= maxIncorrectGuesses)
                    {
                        gameOver = true;
                    }
                    timeVisible = timeToDisappear;
                }
            }

            if (timeVisible > 0)
            {
                timeVisible -= (float)gameTime.ElapsedGameTime.TotalSeconds;
            }
            else
            {
                showCheck = false;
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

            //spriteBatch.DrawString(font, $"Mouse: {Mouse.GetState().X}, {Mouse.GetState().Y}", new Vector2(10, 10), Color.White);
            //spriteBatch.DrawString(font, $"Time: {timer:F2} sec", new Vector2(10, 30), Color.White);
           // spriteBatch.DrawString(font, $"Score: {score}", new Vector2(10, 50), Color.White);
           spriteBatch.DrawString(font, $"Incorrect Guesses: {incorrectGuesses}/{maxIncorrectGuesses}", new Vector2(10, 70), Color.Red);

            if (showCheck && greenCheck != null)
            {
                spriteBatch.Draw(greenCheck, checkPosition, null, Color.White, 0f, Vector2.Zero, 0.3f, SpriteEffects.None, 0f);
            }

            if (showX && redX != null)
            {
                spriteBatch.Draw(redX, xPosition, null, Color.White, 0f, Vector2.Zero, 0.3f, SpriteEffects.None, 0f);
            }

            if (gameOver)
            {
                string message = incorrectGuesses >= maxIncorrectGuesses ? "Game Over! Too many incorrect guesses." : "You found Waldo!";
                spriteBatch.DrawString(font, message, new Vector2(screenWidth / 2 - 100, screenHeight / 2 - 50), Color.Green);
                spriteBatch.DrawString(font, "Press Space to Restart", new Vector2(screenWidth / 2 - 100, screenHeight / 2 - 20), Color.White);
            }
        }

        public void CleanupProcesses()
        {
        }

        public void Dispose()
        {
        }
    }
}



