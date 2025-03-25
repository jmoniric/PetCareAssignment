using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace PetCareGame;

public class MinigameExplanation : LevelInterface
{
    public void CleanupProcesses()
    {
        throw new System.NotImplementedException();
    }

    public void Dispose()
    {
        throw new System.NotImplementedException();
    }

    public void Draw(GameTime gameTime, SpriteBatch spriteBatch, GraphicsDeviceManager _graphics)
    {
        /***
            This is called when your minigame is selected to be rendered.
            DO NOT CALL "spriteBatch.Begin(...)" or "spriteBatch.End()"!!!
            These methods are called in the GameHandler Draw function and
            should not be called again; will yield error.
        ***/

        //delete me when you have code in here \|/
        throw new System.NotImplementedException();
    }

    public void HandleInput(GameTime gameTime)
    {
        /***
            Handle the input for your game in here, from mouse clicks to keypresses.
            Input handled here should ONLY pertain to your minigame
        ***/

        //delete me when you have code in here \|/
        throw new System.NotImplementedException();
    }

    public void LoadContent(ContentManager _manager, ContentManager _coreAssets)
    {
        /***
            Load your content specific to your minigame here.
            _manager is the asset manager that targets your directory
            Your asset directory matches the name of your minigame level class

            If you need to reload core assets with a different scale, you can
            use the _coreAssets asset manager
        ***/

        //delete me when you have code in here \|/
        throw new System.NotImplementedException();
    }

    public void LoadData()
    {
        throw new System.NotImplementedException();
    }

    public void LoadLevel()
    {
        /***
            This class is in here just in case, but I think it may not be necessary.
            However, this is a good place if you need to generate any RNG as it's called
            before any drawing functions get called
        ***/

        //delete me when you have code in here \|/
        throw new System.NotImplementedException();
    }

    public void SaveData()
    {
        throw new System.NotImplementedException();
    }

    public void Update(GameTime gameTime)
    {
        /***
            This is where your update logic should go. This should include animated
            frame updates and such. This SHOULD NOT be reading from any input devices,
            however checking against variables that hold user input that were set in
            HandleInput is fine. This (should) help if/when we need to troubleshoot,
            as input managing will be separate from updating
        ***/
        throw new System.NotImplementedException();
    }
}