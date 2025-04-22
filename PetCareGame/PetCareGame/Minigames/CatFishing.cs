using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace PetCareGame;

public class CatFishing : LevelInterface
{
    public void CleanupProcesses()
    {
        throw new System.NotImplementedException();
    }

    public void Dispose()
    {
        
    }

    public void Draw(GameTime gameTime, SpriteBatch spriteBatch, GraphicsDeviceManager _graphics)
    {
        _graphics.GraphicsDevice.Clear(Color.DarkBlue);
    }

    public void HandleInput(GameTime gameTime)
    {
        
    }

    public void LoadContent(ContentManager _manager, ContentManager _coreAssets)
    {
        
    }

    public void LoadData()
    {
        throw new System.NotImplementedException();
    }

    public void LoadLevel()
    {
        
    }

    public void SaveData(SaveFile saveFile)
    {
        throw new System.NotImplementedException();
    }

    public void Update(GameTime gameTime)
    {
        
    }
}