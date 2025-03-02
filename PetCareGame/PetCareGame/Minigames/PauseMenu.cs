using System;
using System.Security.Principal;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace PetCareGame;

public class PauseMenu : LevelInterface
{
    private Button saveButton;
    private Button mainMenuButton;
    private Button saveQuitButton;
    private Button resumeButton;
    private Vector2 saveButtonPos = new Vector2(790, 400);
    private Vector2 mmButtonPos = new Vector2(790, 480);
    private Vector2 sqButtonPos = new Vector2(790, 560);
    private Vector2 resumeButtonPos = new Vector2(790, 700);
    public void Dispose()
    {
        throw new System.NotImplementedException();
    }

    public void Draw(GameTime gameTime, SpriteBatch spriteBatch, GraphicsDeviceManager _graphics)
    {
        SpriteFont font = GameHandler.highPixel36;
        //draw backing texture
        int backBufferWidth = _graphics.GraphicsDevice.PresentationParameters.BackBufferWidth;
        int backBufferHeight = _graphics.GraphicsDevice.PresentationParameters.BackBufferHeight;
        spriteBatch.Draw(GameHandler.coreTextureAtlas, new Rectangle(0, 0, backBufferWidth, backBufferHeight), new Rectangle(32,0,16,16), Color.LightGray);
        //draw window
        spriteBatch.Draw(GameHandler.coreTextureAtlas, new Rectangle(backBufferWidth/4, backBufferHeight/6, backBufferWidth/2, (int)(backBufferHeight / 1.5)), new Rectangle(16,0,16,16), Color.DimGray);

        //draw "Game Paused"
        spriteBatch.DrawString(GameHandler.highPixel64, "Game Paused", new Vector2(680,280), Color.White);

        //draw save button
        spriteBatch.Draw(GameHandler.coreTextureAtlas, new Rectangle((int)saveButtonPos.X, (int)saveButtonPos.Y, 320, 64), new Rectangle(16,0,16,16), Color.White);
        //draw "Save"
        spriteBatch.DrawString(font, "Save", new Vector2(890,saveButtonPos.Y+20), Color.Black);

        //draw main menu button
        spriteBatch.Draw(GameHandler.coreTextureAtlas, new Rectangle((int)mmButtonPos.X, (int)mmButtonPos.Y, 320, 64), new Rectangle(16,0,16,16), Color.White);
        //draw "Main Menu"
        spriteBatch.DrawString(font, "Main Menu", new Vector2(820,mmButtonPos.Y+20), Color.Black);

        //draw save and quit button
        spriteBatch.Draw(GameHandler.coreTextureAtlas, new Rectangle((int)sqButtonPos.X, (int)sqButtonPos.Y, 320, 128), new Rectangle(16,0,16,16), Color.White);
        //draw "Save and Quit Game"
        spriteBatch.DrawString(font, "Save and\nQuit Game", new Vector2(820,sqButtonPos.Y+20), Color.Black);

        //draw resume button
        spriteBatch.Draw(GameHandler.coreTextureAtlas, new Rectangle((int)resumeButtonPos.X, (int)resumeButtonPos.Y, 320, 64), new Rectangle(16,0,16,16), Color.White);
        //draw "Resume"
        spriteBatch.DrawString(font, "Resume", new Vector2(870,resumeButtonPos.Y+20), Color.Black);
    }

    public void HandleInput(GameTime gameTime)
    {
        if(GameHandler._mouseState.LeftButton == ButtonState.Pressed) {
            if(saveButton.CheckIfButtonWasClicked()) {
                //call save function here :3
            } else if(mainMenuButton.CheckIfButtonWasClicked()) {
                //return to main menu
            } else if(saveQuitButton.CheckIfButtonWasClicked()) {
                //call save function, then quit game
            } else if(resumeButton.CheckIfButtonWasClicked()) {
                GameHandler.isPaused = false;
            }
        } 
    }

    public void LoadContent(ContentManager _manager, ContentManager _coreAssets)
    {
        saveButton = new Button(GameHandler.coreTextureAtlas, GameHandler.coreTextureAtlas, new Point(320,64), saveButtonPos, "Save", 38, true);
        mainMenuButton = new Button(GameHandler.coreTextureAtlas, GameHandler.coreTextureAtlas, new Point(320,64), mmButtonPos, "Main Menu", 39, true);
        saveQuitButton = new Button(GameHandler.coreTextureAtlas, GameHandler.coreTextureAtlas, new Point(320,128), sqButtonPos, "Save and Quit Game", 40, true);
        resumeButton = new Button(GameHandler.coreTextureAtlas, GameHandler.coreTextureAtlas, new Point(320,64), resumeButtonPos, "Resume", 41, true);
    }

    public void LoadLevel()
    {
        
    }

    public void Update(GameTime gameTime)
    {
        
    }
}