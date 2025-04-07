using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace PetCareGame
{
    public class MainMenuScreen : LevelInterface
    {
        private Button _newGameButton;
        private Button _continueButton;
        private Button _quitButton;

        private Vector2 _continueButtonPos = new Vector2(310, 155);
        private Vector2 _newGameButtonPos = new Vector2(310, 210);
        private Vector2 _quitButtonPos = new Vector2(310, 265);

        private Rectangle _newGameButtonBounds;
        private Rectangle _continueButtonBounds;
        private Rectangle _quitButtonBounds;

        private bool _mouseDown = false;

        public static bool fileExists;

        public void Draw(GameTime gameTime, SpriteBatch spriteBatch, GraphicsDeviceManager _graphics)
        {
            SpriteFont font = GameHandler.highPixel22;
            Rectangle atlasButton = new Rectangle(16, 0, 16, 16);

            GameHandler.highPixel22.LineSpacing = 29;

            //draw backing texture
            spriteBatch.Draw(
                GameHandler.coreTextureAtlas,
                new Rectangle(
                    0, 0,
                    (int)GameHandler.baseScreenSize.X,
                    (int)GameHandler.baseScreenSize.Y
                ),
                new Rectangle(32, 0, 16, 16),
                Color.LightGray
            );
            //draw window
            spriteBatch.Draw(
                GameHandler.coreTextureAtlas,
                new Rectangle(
                    (int)(GameHandler.baseScreenSize.X / 8),
                    (int)(GameHandler.baseScreenSize.Y / 10),
                    (int)(GameHandler.baseScreenSize.X / 1.3),
                    (int)(GameHandler.baseScreenSize.Y / 1.25)
                ),
                atlasButton, Color.DimGray
            );

            //draw "Pet Care"
            spriteBatch.DrawString(GameHandler.highPixel36, "Pet Care", new Vector2(240, 100), Color.White);

            // if save file exist draw continue normally
            // if not draw continue faded or darker
            //draw continue button
            spriteBatch.Draw(GameHandler.coreTextureAtlas, _continueButtonBounds, atlasButton, Color.White);
            //draw "Continue"
            spriteBatch.DrawString(font, "Continue", new Vector2(330, _continueButtonPos.Y + 15), Color.Black);

            //draw new game button
            spriteBatch.Draw(GameHandler.coreTextureAtlas, _newGameButtonBounds, atlasButton, Color.White);
            //draw "New Game"
            spriteBatch.DrawString(font, "New Game", new Vector2(330, _newGameButtonPos.Y + 15), Color.Black);

            //draw quit button
            spriteBatch.Draw(GameHandler.coreTextureAtlas, _quitButtonBounds, atlasButton, Color.White);
            //draw "Quit Game"
            spriteBatch.DrawString(font, "Quit Game", new Vector2(330, _quitButtonPos.Y + 15), Color.Black);
        }

        public void HandleInput(GameTime gameTime, GameHandler game)
        {
            if (GameHandler._mouseState.LeftButton == ButtonState.Pressed)
            {
                if (!_mouseDown)
                {
                    _mouseDown = true;

                    if (_newGameButton.CheckIfSelectButtonWasClicked())
                    {
                        // start a new game with a fresh file
                        SetButtonVisibility(false);
                        GameHandler.CurrentState = GameHandler.GameState.Overworld;
                    }
                    else if (_continueButton.CheckIfSelectButtonWasClicked())
                    {
                        // logic for loading content from save file if available
                        SetButtonVisibility(false);
                    }
                    else if (_quitButton.CheckIfSelectButtonWasClicked())
                    {
                        game.Quit();
                    }
                }
            }
            else if (GameHandler._mouseState.LeftButton == ButtonState.Released)
            {
                _mouseDown = false;
            }
        }

        public void Update(GameTime gameTime, GameHandler game)
        {   
            HandleInput(gameTime, game);
        }

        private void SetButtonVisibility(bool isVisible)
        {
            _newGameButton.Visible = isVisible;
            _continueButton.Visible = isVisible;
            _quitButton.Visible = isVisible;
        }

        public void Update(GameTime gameTime)
        {
            throw new NotImplementedException();
        }

        public void HandleInput(GameTime gameTime)
        {
            throw new NotImplementedException();
        }

        public void LoadLevel()
        {
            //creates hitboxes that are used for drawing and checking clicks for buttons
            _continueButtonBounds = new Rectangle((int)_continueButtonPos.X, (int)_continueButtonPos.Y, 200, 48);
            _newGameButtonBounds = new Rectangle((int)_newGameButtonPos.X, (int)_newGameButtonPos.Y, 200, 48);
            _quitButtonBounds = new Rectangle((int)_quitButtonPos.X, (int)_quitButtonPos.Y, 200, 48);
            _newGameButton = new Button(GameHandler.coreTextureAtlas, GameHandler.coreTextureAtlas, new Point(_newGameButtonBounds.Width, _newGameButtonBounds.Height), _newGameButtonPos, "New Game", 33, true);
            _continueButton = new Button(GameHandler.coreTextureAtlas, GameHandler.coreTextureAtlas, new Point(_continueButtonBounds.Width, _continueButtonBounds.Height), _continueButtonPos, "Continue Game", 34, true);
            _quitButton = new Button(GameHandler.coreTextureAtlas, GameHandler.coreTextureAtlas, new Point(_quitButtonBounds.Width, _quitButtonBounds.Height), _quitButtonPos, "Quit Game", 35, true);
        }

        public void LoadContent(ContentManager manager, ContentManager coreAssets)
        {
            throw new NotImplementedException();
        }

        public void CleanupProcesses()
        {
            throw new NotImplementedException();
        }

        public void SaveData()
        {
            throw new NotImplementedException();
        }

        public void LoadData()
        {
            throw new NotImplementedException();
        }

        public void Dispose()
        {
            throw new NotImplementedException();
        }
    }
}