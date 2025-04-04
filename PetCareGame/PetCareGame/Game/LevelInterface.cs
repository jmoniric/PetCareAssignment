using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace PetCareGame;

public abstract class LevelInterface : IDisposable
{
    private bool isComplete = false;
    public bool Completion {
        get {
            return isComplete;
        }
        set {
            isComplete = value;
        }
    }

    //renders minigame on screen
    public abstract void Draw(GameTime gameTime, SpriteBatch spriteBatch, GraphicsDeviceManager _graphics);

    //handles state updates
    public abstract void Update(GameTime gameTime);

    //handles user input
    public abstract void HandleInput(GameTime gameTime);
    
    //passes in the individual ContentManager for the specific game
    //and the core assets ContentManager, for sharing things like UI and UI-related SFX
    public abstract void LoadContent(ContentManager _manager, ContentManager _coreAssets);
    
    //loads the level, this will initialize the specific minigame and from then on that level's
    //input and update manager will get called
    public abstract void LoadLevel();

    //use this to clean up processes currently running; called before UnloadLeve()
    public abstract void CleanupProcesses();

    //Unloads assets for this specific minigame
    public void UnloadLevel(ContentManager _manager) {
        _manager.Unload();
    }

    public abstract void SaveData();

    public abstract void LoadData();

    public void Dispose()
    {
        throw new NotImplementedException();
    }
}