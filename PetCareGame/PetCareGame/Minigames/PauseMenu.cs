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

    private Vector2 saveButtonPos = new Vector2(310, 155);//55px spacing
    private Vector2 mmButtonPos = new Vector2(310, 210);
    private Vector2 sqButtonPos = new Vector2(310, 265);
    private Vector2 resumeButtonPos = new Vector2(310, 345);

    private Rectangle saveButtonBounds;
    private Rectangle mmButtonBounds;
    private Rectangle sqButtonBounds;
    private Rectangle resumeButtonBounds;

    private bool mouseDown = false;

    public void Dispose()
    {
        throw new System.NotImplementedException();
    }

    public void Draw(GameTime gameTime, SpriteBatch spriteBatch, GraphicsDeviceManager _graphics)
    {
        SpriteFont font = GameHandler.highPixel22;
        Rectangle atlasButton = new Rectangle(16,0,16,16);

        GameHandler.highPixel22.LineSpacing = 29;

        //draw backing texture
        spriteBatch.Draw(
            GameHandler.coreTextureAtlas,
            new Rectangle(
                0, 0,
                (int)GameHandler.baseScreenSize.X,
                (int)GameHandler.baseScreenSize.Y
            ),
            new Rectangle(32,0,16,16),
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

        //draw "Game Paused"
        spriteBatch.DrawString(GameHandler.highPixel36, "Game Paused", new Vector2(240,100), Color.White);

        //draw save button
        spriteBatch.Draw(GameHandler.coreTextureAtlas, saveButtonBounds, atlasButton, Color.White);
        //draw "Save"
        spriteBatch.DrawString(font, "Save", new Vector2(370,saveButtonPos.Y+15), Color.Black);

        //draw main menu button
        
        spriteBatch.Draw(GameHandler.coreTextureAtlas, mmButtonBounds, atlasButton, Color.White);
        //draw "Main Menu"
        spriteBatch.DrawString(font, "Main Menu", new Vector2(330,mmButtonPos.Y+15), Color.Black);

        //draw save and quit button
        
        spriteBatch.Draw(GameHandler.coreTextureAtlas, sqButtonBounds, atlasButton, Color.White);
        //draw "Save and Quit Game"
        spriteBatch.DrawString(font, "Save and\nQuit Game", new Vector2(330,sqButtonPos.Y+15), Color.Black);

        //draw resume button
        spriteBatch.Draw(GameHandler.coreTextureAtlas, resumeButtonBounds, atlasButton, Color.White);
        //draw "Resume"
        spriteBatch.DrawString(font, "Resume", new Vector2(350,resumeButtonPos.Y+15), Color.Black);
    }

    public void HandleInput(GameTime gameTime)
    {
        if(GameHandler._mouseState.LeftButton == ButtonState.Pressed) {
            if(!mouseDown) {
                mouseDown = true;

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
        } else if(GameHandler._mouseState.LeftButton == ButtonState.Released) {
            mouseDown = false;
        }
    }

    public void LoadContent(ContentManager _manager, ContentManager _coreAssets)
    {
        saveButton = new Button(GameHandler.coreTextureAtlas, GameHandler.coreTextureAtlas, new Point(saveButtonBounds.Width,saveButtonBounds.Height), saveButtonPos, "Save", 38, true);
        mainMenuButton = new Button(GameHandler.coreTextureAtlas, GameHandler.coreTextureAtlas, new Point(mmButtonBounds.Width,mmButtonBounds.Height), mmButtonPos, "Main Menu", 39, true);
        saveQuitButton = new Button(GameHandler.coreTextureAtlas, GameHandler.coreTextureAtlas, new Point(sqButtonBounds.Width,sqButtonBounds.Height), sqButtonPos, "Save and Quit Game", 40, true);
        resumeButton = new Button(GameHandler.coreTextureAtlas, GameHandler.coreTextureAtlas, new Point(320,64), resumeButtonPos, "Resume", 41, true);
    }

    public void LoadLevel()
    {
        //creates hitboxes that are used for drawing and checking clicks for buttons
        saveButtonBounds = new Rectangle((int)saveButtonPos.X, (int)saveButtonPos.Y, 200, 48);
        mmButtonBounds =  new Rectangle((int)mmButtonPos.X, (int)mmButtonPos.Y, 200, 48);
        sqButtonBounds = new Rectangle((int)sqButtonPos.X, (int)sqButtonPos.Y, 200, 76);
        resumeButtonBounds = new Rectangle((int)resumeButtonPos.X, (int)resumeButtonPos.Y, 200, 48);
    }

    public void Update(GameTime gameTime)
    {
        
    }
}