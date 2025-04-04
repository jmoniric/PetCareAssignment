using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace PetCareGame;

public class CatFishing : LevelInterface
{
    override public void CleanupProcesses()
    {
        throw new System.NotImplementedException();
    }

    override public void Draw(GameTime gameTime, SpriteBatch spriteBatch, GraphicsDeviceManager _graphics)
    {
        _graphics.GraphicsDevice.Clear(Color.DarkBlue);
    }

    override public void HandleInput(GameTime gameTime)
    {
        
    }

    override public void LoadContent(ContentManager _manager, ContentManager _coreAssets)
    {
        
    }

    override public void LoadData()
    {
        throw new System.NotImplementedException();
    }

    override public void LoadLevel()
    {
        
    }

    override public void SaveData()
    {
        throw new System.NotImplementedException();
    }

    override public void Update(GameTime gameTime)
    {
        
    }
}