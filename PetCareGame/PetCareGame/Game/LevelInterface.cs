using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace PetCareGame;

public interface LevelInterface : IDisposable
{
    public void Draw(GameTime gameTime, SpriteBatch spriteBatch);
    public void Update(GameTime gameTime);
    public void HandleInput(GameTime gameTime);
    public void LoadContent(SpriteBatch spriteBatch);
    public void LoadLevel();
}