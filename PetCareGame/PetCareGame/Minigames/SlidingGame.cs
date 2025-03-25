using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace PetCareGame;

public class SlidingGame : LevelInterface
{

    private Color backgroundColour = new Color(50, 205, 50);

    private Vector2 catPos = new  Vector2(300, 305);

    private Vector2 boxPos = new  Vector2(500, 500);

    private Point chestPos = new Point(400, 350);

    private Texture2D atlas;

    private Texture2D chest;

    private Rectangle chestBounds;

    private bool faceRight = true;

     private bool isMoving = false;


    public void Dispose()
    {

    }

   

public void Draw(GameTime gameTime, SpriteBatch spriteBatch, GraphicsDeviceManager _graphics)
{
    Rectangle grass = new Rectangle(16, 0, 16, 16);
    Rectangle chestRect = new Rectangle(0, 0, 32, 32);
    _graphics.GraphicsDevice.Clear(backgroundColour);

    // Define center and chest texture sources
    Rectangle centerTextureSource = new Rectangle(32, 32, 16, 16);  
    Rectangle openchestTextureSource = new Rectangle(0, 32, 32, 32); 

    int squareSize = (GameHandler.windowWidth / 2) + 50;
    int squareX = (GameHandler.windowWidth - squareSize) / 2;
    int squareY = (GameHandler.windowHeight - squareSize) / 2;

    int centerX = ((GameHandler.windowWidth - 64) / 2) - 185;  
    int centerY = ((GameHandler.windowHeight - 64) / 2) - 190; 

   
    Rectangle textureSource = new Rectangle(16, 32, 16, 16); 

    // Draw background (grass) first
    for (int h = 0; h < 16; h++)
    {
        for (int v = 0; v < 16; v++)
        {
            spriteBatch.Draw(atlas, new Rectangle(h * 128, v * 128, 128, 128), grass, Color.White, 0f, Vector2.Zero, SpriteEffects.None, 0f);
        }
    }

    
    int edgeThickness = 15;  

    // Draw the inner square
    spriteBatch.Draw(atlas, new Rectangle(squareX, squareY, squareSize, edgeThickness), textureSource, Color.White);  // Top edge
    spriteBatch.Draw(atlas, new Rectangle(squareX, squareY + squareSize - edgeThickness, squareSize, edgeThickness), textureSource, Color.White);  // Bottom edge
    spriteBatch.Draw(atlas, new Rectangle(squareX, squareY, edgeThickness, squareSize), textureSource, Color.White);  // Left edge
    spriteBatch.Draw(atlas, new Rectangle(squareX + squareSize - edgeThickness, squareY, edgeThickness, squareSize), textureSource, Color.White);  // Right edge

    // Tolerance value to allow for small movement errors 
    int tolerance = 35;

    // Check if the chest is close enough to the center position (within tolerance)
    bool isOverlapping = Math.Abs(chestPos.X - centerX) <= tolerance && Math.Abs(chestPos.Y - centerY) <= tolerance;

    // If chest overlaps with the center, hide the center texture and original chest texture, and draw the chest texture
    if (isOverlapping)
    {
        // Only draw the chest texture in place of the center texture if they overlap
        spriteBatch.Draw(chest, new Rectangle(centerX, centerY, 64, 64), openchestTextureSource, Color.White); // Draw the open chest texture at the center
    }
    else
    {
        // Draw the center textures only if there's no overlap with the chest
        spriteBatch.Draw(atlas, new Rectangle(centerX, centerY, 64, 64), centerTextureSource, Color.White); 

        spriteBatch.Draw(chest, chestBounds, chestRect, Color.White, 0f, Vector2.Zero, SpriteEffects.None, 0f); // Original chest texture
    }

    // Chests, cats, and other sprites
    chestBounds = new Rectangle(chestPos.X, chestPos.Y, 64, 64);

    if (isMoving)
    {
        GameHandler.catWalk.DrawFrame(spriteBatch, catPos, faceRight ? SpriteEffects.None : SpriteEffects.FlipHorizontally, 2f);
    }
    else
    {
        GameHandler.catIdle.DrawFrame(spriteBatch, catPos, faceRight ? SpriteEffects.None : SpriteEffects.FlipHorizontally, 2f);
    }
}


public void HandleInput(GameTime gameTime)
{
    isMoving = false;
    KeyboardState keyboardState = Keyboard.GetState();
    float elapsed = (float)gameTime.ElapsedGameTime.TotalSeconds;

    // Define the cat's bounding box
    Rectangle catBounds = new Rectangle((int)catPos.X, (int)catPos.Y, 96, 96); // Assuming cat size is 96x96

    // Store original chest position in case movement needs to be reverted
    Point originalChestPos = chestPos;

    // Define the tile size (64x64)
    int tileSize = 64;

    // Check movement in each direction and move chest if colliding
    if (keyboardState.IsKeyDown(Keys.Left))
    {
        Rectangle newBounds = new Rectangle((int)(catPos.X - 300 * elapsed), (int)catPos.Y, 96, 96);
        if (newBounds.Intersects(chestBounds))
        {
            // Move chest to the left (tile-based movement)
            chestPos.X -= tileSize; // Move chest to the left by one tile
        }
        else
        {
            // Move cat normally
            catPos.X -= 300 * elapsed;
        }
        faceRight = false;
        isMoving = true;
    }
    if (keyboardState.IsKeyDown(Keys.Right))
    {
        Rectangle newBounds = new Rectangle((int)(catPos.X + 300 * elapsed), (int)catPos.Y, 96, 96);
        if (newBounds.Intersects(chestBounds))
        {
            // Move chest to the right (tile-based movement)
            chestPos.X += tileSize; // Move chest to the right by one tile
        }
        else
        {
            // Move cat normally
            catPos.X += 300 * elapsed;
        }
        faceRight = true;
        isMoving = true;
    }
    if (keyboardState.IsKeyDown(Keys.Up))
    {
        Rectangle newBounds = new Rectangle((int)catPos.X, (int)(catPos.Y - 300 * elapsed), 96, 96);
        if (newBounds.Intersects(chestBounds))
        {
            // Move chest up (tile-based movement)
            chestPos.Y -= tileSize; // Move chest up by one tile
        }
        else
        {
            // Move cat normally
            catPos.Y -= 300 * elapsed;
        }
        isMoving = true;
    }
    if (keyboardState.IsKeyDown(Keys.Down))
    {
        Rectangle newBounds = new Rectangle((int)catPos.X, (int)(catPos.Y + 300 * elapsed), 96, 96);
        if (newBounds.Intersects(chestBounds))
        {
            // Move chest down (tile-based movement)
            chestPos.Y += tileSize; // Move chest down by one tile
        }
        else
        {
            // Move cat normally
            catPos.Y += 300 * elapsed;
        }
        isMoving = true;
    }

    // Update chest bounds to match its new position
    chestBounds = new Rectangle(chestPos.X, chestPos.Y, 64, 64);
}

    public void LoadContent(ContentManager _manager, ContentManager _coreAssets)
    {

        atlas = _manager.Load<Texture2D>("Sprites/petcare_slidetextureatlas");
        GameHandler.catIdle.Load(_coreAssets, "Sprites/Animal/idle", 7, 5);
        GameHandler.catWalk.Load(_coreAssets, "Sprites/Animal/walk", 7, 5);
        chest = _manager.Load<Texture2D>("Sprites/treasure_atlas");
    }

    public void LoadLevel()
    {

    }

    public void Update(GameTime gameTime)
    {


    }
}