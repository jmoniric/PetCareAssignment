using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace PetCareGame;

public interface LevelInterface : IDisposable
{
    //renders minigame on screen
    public void Draw(GameTime gameTime, SpriteBatch spriteBatch, GraphicsDeviceManager _graphics);

    //handles state updates
    public void Update(GameTime gameTime);

    //handles user input
    public void HandleInput(GameTime gameTime);
    
    //passes in the individual ContentManager for the specific game
    //and the core assets ContentManager, for sharing things like UI and UI-related SFX
    public void LoadContent(ContentManager _manager, ContentManager _coreAssets);
    
    //loads the level, this will initialize the specific minigame and from then on that level's
    //input and update manager will get called
    public void LoadLevel();

    //use this to clean up processes currently running; called before UnloadLeve()
    public void CleanupProcesses();

    //Unloads assets for this specific minigame
    public void UnloadLevel(ContentManager _manager) {
        _manager.Unload();
    }
}