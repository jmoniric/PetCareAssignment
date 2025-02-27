using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace PetCareGame;

public class PauseMenu : LevelInterface
{
    private Button saveButton;
    private Vector2 saveButtonPos = new Vector2(880, 400);
    public void Dispose()
    {
        throw new System.NotImplementedException();
    }

    public void Draw(GameTime gameTime, SpriteBatch spriteBatch, GraphicsDeviceManager _graphics)
    {
        spriteBatch.Draw(GameHandler.plainWhiteTexture, new Rectangle(0, 0, GameHandler.windowWidth, GameHandler.windowHeight), Color.LightGray);
        spriteBatch.Draw(GameHandler.coreTextureAtlas, new Rectangle(GameHandler.windowWidth/4, GameHandler.windowHeight/6, GameHandler.windowWidth/2, (int)(GameHandler.windowHeight/1.5)), new Rectangle(16,0,16,16), Color.DimGray);
        spriteBatch.Draw(GameHandler.coreTextureAtlas, new Rectangle((int)saveButtonPos.X, (int)saveButtonPos.Y, 160, 64), new Rectangle(16,0,16,16), Color.White);
        spriteBatch.DrawString(GameHandler.courierNew, "Save", new Vector2(900,400), Color.Black);
    }

    public void HandleInput(GameTime gameTime)
    {
        if(GameHandler.CheckIfButtonWasClicked(saveButton)) {
            //call save function here :3
        }
    }

    public void LoadContent(ContentManager _manager, ContentManager _coreAssets)
    {
        saveButton = new Button(GameHandler.coreTextureAtlas, GameHandler.coreTextureAtlas, new Point(64,160), saveButtonPos, "Save", 38, true);
    }

    public void LoadLevel()
    {
        
    }

    public void Update(GameTime gameTime)
    {
        
    }
}