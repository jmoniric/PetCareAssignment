using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace PetCareGame
{
    public class MainMenuScreen : LevelInterface
    {
        private Button newGameButton;
        private Button continueButton;
        private Button quitButton;

        private Vector2 continueButtonPos = new Vector2(310, 155);
        private Vector2 newGameButtonPos = new Vector2(310, 210);
        private Vector2 quitButtonPos = new Vector2(310, 265);

        private Rectangle newGameButtonBounds;
        private Rectangle continueButtonBounds;
        private Rectangle quitButtonBounds;

        private bool mouseDown = false;
        private bool ifSaveFileExists;

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
            spriteBatch.DrawString(GameHandler.highPixel36, "Pet Care", new Vector2(290, 100), Color.White);

            // if save file exist draw continue normally
            // if not draw continue faded or darker
            if (SaveFile.doesFileExist())
            {
                //draw active continue button
                spriteBatch.Draw(GameHandler.coreTextureAtlas, continueButtonBounds, atlasButton, Color.White);
            }
            else
            {
                //draw inactive continue button
                spriteBatch.Draw(GameHandler.coreTextureAtlas, continueButtonBounds, atlasButton, Color.White * 0.5f);
            }

            //draw "Continue"
            spriteBatch.DrawString(font, "Continue", new Vector2(340, continueButtonPos.Y + 15), Color.Black);

            //draw new game button
            spriteBatch.Draw(GameHandler.coreTextureAtlas, newGameButtonBounds, atlasButton, Color.White);
            //draw "New Game"
            spriteBatch.DrawString(font, "New Game", new Vector2(330, newGameButtonPos.Y + 15), Color.Black);

            //draw quit button
            spriteBatch.Draw(GameHandler.coreTextureAtlas, quitButtonBounds, atlasButton, Color.White);
            //draw "Quit Game"
            spriteBatch.DrawString(font, "Quit Game", new Vector2(330, quitButtonPos.Y + 15), Color.Black);
        }

        public void HandleInput(GameTime gameTime, GameHandler game)
        {
            if (GameHandler.mouseState.LeftButton == ButtonState.Pressed)
            {
                if (!mouseDown)
                {
                    mouseDown = true;

                    if (newGameButton.CheckIfSelectButtonWasClicked())
                    {
                        // start a new game with a fresh file
                        GameHandler.saveFile.NewFile(GameHandler.saveFile);
                        SetButtonVisibility(false);
                        GameHandler.LoadOverworld();
                    }
                    else if (continueButton.CheckIfSelectButtonWasClicked() && SaveFile.doesFileExist())
                    {
                        // logic for loading content from save file if available
                        SetButtonVisibility(false);
                        GameHandler.saveFile = GameHandler.saveFile.Load();
                        //fix to make sure that level status isn't resetting on game continue
                        GameHandler.slidingLevel.LoadData();
                        GameHandler.LoadOverworld();
                    }
                    else if (quitButton.CheckIfSelectButtonWasClicked())
                    {
                        game.Quit();
                    }
                }
            }
            else if (GameHandler.mouseState.LeftButton == ButtonState.Released)
            {
                mouseDown = false;
            }
        }

        public void Update(GameTime gameTime, GameHandler game)
        {
            HandleInput(gameTime, game);
        }

        private void SetButtonVisibility(bool isVisible)
        {
            newGameButton.Visible = isVisible;
            continueButton.Visible = isVisible;
            quitButton.Visible = isVisible;
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
            continueButtonBounds = new Rectangle((int)continueButtonPos.X, (int)continueButtonPos.Y, 200, 48);
            newGameButtonBounds = new Rectangle((int)newGameButtonPos.X, (int)newGameButtonPos.Y, 200, 48);
            quitButtonBounds = new Rectangle((int)quitButtonPos.X, (int)quitButtonPos.Y, 200, 48);
            newGameButton = new Button(GameHandler.coreTextureAtlas, GameHandler.coreTextureAtlas, new Point(newGameButtonBounds.Width, newGameButtonBounds.Height), newGameButtonPos, "New Game", 33, true);
            quitButton = new Button(GameHandler.coreTextureAtlas, GameHandler.coreTextureAtlas, new Point(quitButtonBounds.Width, quitButtonBounds.Height), quitButtonPos, "Quit Game", 35, true);

            if (SaveFile.doesFileExist())
            {
                ifSaveFileExists = true;
            }
            else
            {
                ifSaveFileExists = false;
            }

            continueButton = new Button(GameHandler.coreTextureAtlas, GameHandler.coreTextureAtlas, new Point(continueButtonBounds.Width, continueButtonBounds.Height), continueButtonPos, "Continue Game", 34, ifSaveFileExists);
        }

        public void LoadContent(ContentManager manager, ContentManager coreAssets)
        {
            throw new NotImplementedException();
        }

        public void CleanupProcesses()
        {
            throw new NotImplementedException();
        }

        public void SaveData(SaveFile saveFile)
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