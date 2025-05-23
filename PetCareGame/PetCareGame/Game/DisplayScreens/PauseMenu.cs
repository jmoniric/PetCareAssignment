/** 
@author: Javier & Nolan
*/
using System;
using System.Security.Principal;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace PetCareGame;

public class PauseMenu : LevelInterface
{
    // Enumarates states in which the game can switch between
    // During the pause menu use
    private enum PauseState
    {
        ExitWarning,
        SavingWarning,
        MainMenuWarning
    }

    private PauseState currentState;

    // Button objects for pause menu
    private Button saveButton;
    private Button mainMenuButton;
    private Button saveQuitButton;
    private Button resumeButton;
    private Button yesButton;
    private Button noButton;
    private Button muteSfxButton;
    private Button muteMusicButton;
    private Button resetWindowButton;

    // Position for buttons
    private Vector2 saveButtonPos = new Vector2(310, 155);//55px spacing
    private Vector2 mmButtonPos = new Vector2(310, 210);
    private Vector2 sqButtonPos = new Vector2(310, 265);
    private Vector2 resumeButtonPos = new Vector2(310, 345);
    private Vector2 yesButtonPos = new Vector2(250, 250);
    private Vector2 noButtonPos = new Vector2(400, 250);
    private Vector2 muteSfxPos = new Vector2(170, 150);
    private Vector2 muteMusicPos = new Vector2(170, 250);
    private Vector2 resetButtonPos = new Vector2(170, 350);

    // Bounds where buttons are created
    private Rectangle saveButtonBounds;
    private Rectangle mmButtonBounds;
    private Rectangle sqButtonBounds;
    private Rectangle resumeButtonBounds;
    private Rectangle yesButtonBounds;
    private Rectangle noButtonBounds;
    private Rectangle muteSfxBounds;
    private Rectangle muteMusicBounds;
    private Rectangle resetButtonBounds;

    private bool mouseDown = false;

    // Is responsible for drawing content to the screen. (Monogame Documentation)
    public void Draw(GameTime gameTime, SpriteBatch spriteBatch, GraphicsDeviceManager _graphics)
    {
        SpriteFont font = GameHandler.highPixel22;
        Rectangle atlasButton = new Rectangle(16, 0, 16, 16);
        Rectangle atlasAudioButton = new Rectangle(0, 32, 16, 16);
        Rectangle atlasMusicButton = new Rectangle(32, 16, 16, 16);
        Rectangle atlasXMark = new Rectangle(48, 0, 16, 16);
        Rectangle atlasResetButton = new Rectangle(48, 16, 16, 16);

        GameHandler.highPixel22.LineSpacing = 29;

        // Draw backing texture
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
        // Draw window
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

        //
        if (isWarning)
        {
            switch (currentState)
            {
                case PauseState.ExitWarning:
                    DrawWarning(spriteBatch, atlasButton, font, "   Exiting Game", 0);
                    break;
                case PauseState.MainMenuWarning:
                    DrawWarning(spriteBatch, atlasButton, font, "Going to Main Menu", 0);
                    break;
                case PauseState.SavingWarning:
                    DrawWarning(spriteBatch, atlasButton, font, "Overwrite Existing\n     Save File", 25);
                    break;
            }
        }
        else
        {
            // Draw "Game Paused"
            spriteBatch.DrawString(GameHandler.highPixel36, "Game Paused", new Vector2(240, 100), Color.White);

            // Draw save button
            spriteBatch.Draw(GameHandler.coreTextureAtlas, saveButtonBounds, atlasButton, Color.White);

            //Draw "Save"
            spriteBatch.DrawString(font, "Save", new Vector2(370, saveButtonPos.Y + 15), Color.Black);

            // Draw main menu button
            spriteBatch.Draw(GameHandler.coreTextureAtlas, mmButtonBounds, atlasButton, Color.White);

            //Draw "Main Menu"
            spriteBatch.DrawString(font, "Main Menu", new Vector2(330, mmButtonPos.Y + 15), Color.Black);

            // Draw save and quit button
            spriteBatch.Draw(GameHandler.coreTextureAtlas, sqButtonBounds, atlasButton, Color.White);

            // Draw "Save and Quit Game"
            spriteBatch.DrawString(font, "Save and\nQuit Game", new Vector2(330, sqButtonPos.Y + 15), Color.Black);

            // Draw resume button
            spriteBatch.Draw(GameHandler.coreTextureAtlas, resumeButtonBounds, atlasButton, Color.White);

            // Draw "Resume"
            spriteBatch.DrawString(font, "Resume", new Vector2(350, resumeButtonPos.Y + 15), Color.Black);

            // Draw mute button
            if (GameHandler.allowAudio)
            {
                spriteBatch.Draw(GameHandler.coreTextureAtlas, muteSfxBounds, atlasAudioButton, Color.White);
                if (GameHandler.muted)
                {
                    spriteBatch.Draw(GameHandler.coreTextureAtlas, muteSfxBounds, atlasXMark, Color.Red);
                }

                spriteBatch.Draw(GameHandler.coreTextureAtlas, muteMusicBounds, atlasMusicButton, Color.White);
                if (GameHandler.musicMuted)
                {
                    spriteBatch.Draw(GameHandler.coreTextureAtlas, muteMusicBounds, atlasXMark, Color.Red);
                }
            }

            spriteBatch.Draw(GameHandler.coreTextureAtlas, resetButtonBounds, atlasResetButton, Color.White);
        }
    }

    // This method handles any input within the pause menu
    public void HandleInput(GameTime gameTime, GameHandler game)
    {
        if (GameHandler.mouseState.LeftButton == ButtonState.Pressed)
        {
            if (!mouseDown)
            {
                mouseDown = true;

                if (saveButton.CheckIfSelectButtonWasClicked())
                {
                    currentState = PauseState.SavingWarning;
                    isWarning = true;
                }
                else if (mainMenuButton.CheckIfSelectButtonWasClicked())
                {
                    isWarning = true;
                    SetButtonVisibility();
                    Overworld.SetVisiblity(true);
                    currentState = PauseState.MainMenuWarning;
                }
                else if (saveQuitButton.CheckIfSelectButtonWasClicked())
                {
                    isWarning = true;
                    SetButtonVisibility();
                    currentState = PauseState.ExitWarning;
                }
                else if (resumeButton.CheckIfSelectButtonWasClicked())
                {
                    GameHandler.isPaused = false;
                }
                else if (muteSfxButton.CheckIfSelectButtonWasClicked())
                {
                    GameHandler.muted = !GameHandler.muted;
                    Console.WriteLine("Mute toggled");
                }
                else if (muteMusicButton.CheckIfSelectButtonWasClicked())
                {
                    GameHandler.musicMuted = !GameHandler.musicMuted;
                    Console.WriteLine("Music toggled");
                }
                else if (resetWindowButton.CheckIfSelectButtonWasClicked())
                {
                    GameHandler.displayManager.SetResolution(800, 600);
                    GameHandler.displayManager.UpdateScreenScaleMatrix();
                }
                else if (isWarning)
                {
                    switch (currentState)
                    {
                        case PauseState.ExitWarning:
                            if (yesButton.CheckIfSelectButtonWasClicked())
                            {
                                GameHandler.saveFile.Save(GameHandler.saveFile);
                                game.Exit();
                            }
                            else if (noButton.CheckIfSelectButtonWasClicked())
                            {
                                isWarning = false;
                            }
                            break;
                        case PauseState.SavingWarning:
                            if (yesButton.CheckIfSelectButtonWasClicked())
                            {
                                GameHandler.saveFile.Save(GameHandler.saveFile);
                                isWarning = false;
                            }
                            else if (noButton.CheckIfSelectButtonWasClicked())
                            {
                                isWarning = false;
                            }
                            break;
                        case PauseState.MainMenuWarning:
                            if (yesButton.CheckIfSelectButtonWasClicked())
                            {
                                if (GameHandler.CurrentState == GameHandler.GameState.Overworld)
                                {
                                    GameHandler.overworldLevel.CleanupProcesses();
                                }
                                GameHandler.UnloadCurrentLevel();
                                GameHandler.CurrentState = GameHandler.GameState.MainMenu;
                                GameHandler.isPaused = false;
                                isWarning = false;
                            }
                            else if (noButton.CheckIfSelectButtonWasClicked())
                            {
                                isWarning = false;
                            }
                            break;
                    }
                }
            }
        }
        else if (GameHandler.mouseState.LeftButton == ButtonState.Released)
        {
            mouseDown = false;
        }
    }

    // Loads necessary assets to pause menu
    public void LoadContent(ContentManager _manager, ContentManager _coreAssets)
    {
        // Initializes the button objects
        saveButton = new Button(GameHandler.coreTextureAtlas, GameHandler.coreTextureAtlas, new Point(saveButtonBounds.Width, saveButtonBounds.Height), saveButtonPos, "Save", 38, true);
        mainMenuButton = new Button(GameHandler.coreTextureAtlas, GameHandler.coreTextureAtlas, new Point(mmButtonBounds.Width, mmButtonBounds.Height), mmButtonPos, "Main Menu", 39, true);
        saveQuitButton = new Button(GameHandler.coreTextureAtlas, GameHandler.coreTextureAtlas, new Point(sqButtonBounds.Width, sqButtonBounds.Height), sqButtonPos, "Save and Quit Game", 40, true);
        resumeButton = new Button(GameHandler.coreTextureAtlas, GameHandler.coreTextureAtlas, new Point(320, 64), resumeButtonPos, "Resume", 41, true);
        yesButton = new Button(GameHandler.coreTextureAtlas, GameHandler.coreTextureAtlas, new Point(yesButtonBounds.Width, yesButtonBounds.Height), yesButtonPos, "Yes", 42, false);
        noButton = new Button(GameHandler.coreTextureAtlas, GameHandler.coreTextureAtlas, new Point(noButtonBounds.Width, noButtonBounds.Height), noButtonPos, "No", 43, false);
        muteSfxButton = new Button(GameHandler.coreTextureAtlas, GameHandler.coreTextureAtlas, new Point(muteSfxBounds.Width, muteSfxBounds.Height), muteSfxPos, "Mute SFX", 44, true);
        muteMusicButton = new Button(GameHandler.coreTextureAtlas, GameHandler.coreTextureAtlas, new Point(muteMusicBounds.Width, muteMusicBounds.Height), muteMusicPos, "Mute Music", 45, true);
        resetWindowButton = new Button(GameHandler.coreTextureAtlas, GameHandler.coreTextureAtlas, new Point(resetButtonBounds.Width, resetButtonBounds.Height), resetButtonPos, "Reset Window", 46, true);
        if (!GameHandler.allowAudio)
        {
            muteSfxButton.Visible = false;
            muteMusicButton.Visible = false;
        }
    }

    // Initializes the necessary bounds for the buttons
    public void LoadLevel()
    {
        // Creates hitboxes that are used for drawing and checking clicks for buttons
        saveButtonBounds = new Rectangle((int)saveButtonPos.X, (int)saveButtonPos.Y, 200, 48);
        mmButtonBounds = new Rectangle((int)mmButtonPos.X, (int)mmButtonPos.Y, 200, 48);
        sqButtonBounds = new Rectangle((int)sqButtonPos.X, (int)sqButtonPos.Y, 200, 76);
        resumeButtonBounds = new Rectangle((int)resumeButtonPos.X, (int)resumeButtonPos.Y, 200, 48);
        yesButtonBounds = new Rectangle((int)yesButtonPos.X, (int)yesButtonPos.Y, 100, 48);
        noButtonBounds = new Rectangle((int)noButtonPos.X, (int)noButtonPos.Y, 100, 48);
        muteSfxBounds = new Rectangle((int)muteSfxPos.X, (int)muteSfxPos.Y, 64, 64);
        muteMusicBounds = new Rectangle((int)muteMusicPos.X, (int)muteMusicPos.Y, 64, 64);
        resetButtonBounds = new Rectangle((int)resetButtonPos.X, (int)resetButtonPos.Y, 64, 64);
    }

    public void Update(GameTime gameTime, GameHandler game)
    {
        HandleInput(gameTime, game);
        SetButtonVisibility();
    }

    // Sets the visibility of buttons to make them nonclickable or clickable
    // Dependant on if the warning page is live
    public void SetButtonVisibility()
    {
        if (isWarning)
        {
            saveButton.Visible = false;
            mainMenuButton.Visible = false;
            saveQuitButton.Visible = false;
            resumeButton.Visible = false;
            yesButton.Visible = true;
            noButton.Visible = true;
        }
        else
        {
            saveButton.Visible = true;
            mainMenuButton.Visible = true;
            saveQuitButton.Visible = true;
            resumeButton.Visible = true;
            yesButton.Visible = false;
            noButton.Visible = false;
        }
    }

    // This method draws warning page that will be reused
    public void DrawWarning(SpriteBatch spriteBatch, Rectangle atlasButton, SpriteFont font, String str, int extraSpacing)
    {
        // Draw str that is passed in
        spriteBatch.DrawString(GameHandler.highPixel36, str, new Vector2(150, 100), Color.White);

        // Draw "Are you sure"
        spriteBatch.DrawString(GameHandler.highPixel36, "Are you sure?", new Vector2(240, 170 + extraSpacing), Color.White);

        // Draw Yes button
        spriteBatch.Draw(GameHandler.coreTextureAtlas, yesButtonBounds, atlasButton, Color.White);

        // Draw "Yes"
        spriteBatch.DrawString(font, "Yes", new Vector2(270, yesButtonPos.Y + 15), Color.Black);

        // Draw No button
        spriteBatch.Draw(GameHandler.coreTextureAtlas, noButtonBounds, atlasButton, Color.White);

        // Draw "no"
        spriteBatch.DrawString(font, "No", new Vector2(420, noButtonPos.Y + 15), Color.Black);
    }

    public void Update(GameTime gameTime)
    {
        throw new NotImplementedException();
    }

    public void HandleInput(GameTime gameTime)
    {
        throw new NotImplementedException();
    }
    public void CleanupProcesses()
    {
        throw new NotImplementedException();
    }
    private bool isWarning = false;

    public void Dispose()
    {
        throw new System.NotImplementedException();
    }

    public void SaveData(SaveFile saveFile)
    {
        throw new NotImplementedException();
    }

    public void LoadData()
    {
        throw new NotImplementedException();
    }
}