using System.Runtime.InteropServices;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;

namespace PetCareGame;

public class PetCare : LevelInterface
{
    enum ObjectHeld {
        SprayBottle,
        Brush,
        Towel,
        NailClippers,
        None
    }
    private Vector2 catPos = new Vector2(GameHandler.windowWidth / 2, 720);
    private Color backgroundColour = new Color(197, 118, 38);
    private Texture2D atlas;

    private Point sprayBottlePos = new Point(124, 400);
    private Vector2 sprayBottleOrigin = new Vector2(0,0);
    private Rectangle sprayBottleBounds;
    private float sprayBottleRot = 0f;

    private bool faceRight = true;
    private ObjectHeld currentObject = ObjectHeld.None;

    public void Dispose()
    {
        
    }

    public void Draw(GameTime gameTime, SpriteBatch spriteBatch, GraphicsDeviceManager _graphics)
    {
        Rectangle floor = new Rectangle(0, 0, 31, 31);
        Rectangle floorFiller = new Rectangle(0, 12, 16, 16);
        Rectangle wall = new Rectangle(32, 0, 31, 31);
        Rectangle sprayBottle = new Rectangle(64, 0, 31, 31);

        _graphics.GraphicsDevice.Clear(backgroundColour);
        
        //draw wall paneling
        for(int h = 0; h < 16; h++) {
            for(int v = 0; v < 8; v++) {
                spriteBatch.Draw(atlas, new Rectangle(h*124, v*124, 128, 128), wall, Color.White, 0f, Vector2.Zero, SpriteEffects.None, 0f);
            }
        }

        //draw flooring
        for(int i = 0; i < 16; i++) {
            spriteBatch.Draw(atlas, new Rectangle(i*124, 830, 128, 128), floor, Color.White, 0f, Vector2.Zero, SpriteEffects.None, 1f);
            spriteBatch.Draw(atlas, new Rectangle(i*124, 958, 128, 128), floorFiller, Color.White, 0f, Vector2.Zero, SpriteEffects.None, 1f);
        }

        //draw cat
        if(faceRight) {
            GameHandler.catIdle.DrawFrame(spriteBatch, catPos, SpriteEffects.None);
        } else {
            GameHandler.catIdle.DrawFrame(spriteBatch, catPos, SpriteEffects.FlipHorizontally);
        }

        //bounding box of spray bottle
        sprayBottleBounds = new Rectangle(sprayBottlePos, new Point(96, 96));

        //if object is held and is on right side of screen, rotate and flip sprite so it points at cat
        if(currentObject == ObjectHeld.SprayBottle && faceRight) {
            spriteBatch.Draw(atlas, sprayBottleBounds, sprayBottle, Color.White, sprayBottleRot + 270f, sprayBottleOrigin, SpriteEffects.FlipVertically, 1f);
        
        //otherwise render with original rotation calculated in HandleInput
        } else {
            spriteBatch.Draw(atlas, sprayBottleBounds, sprayBottle, Color.White, sprayBottleRot, sprayBottleOrigin, SpriteEffects.None, 1f);
        }
        
        
    }

    public void HandleInput(GameTime gameTime)
    {
        MouseState mouseState = Mouse.GetState();

        //makes cat face in mouse's direction
        if(mouseState.X > (GameHandler.windowWidth/2)) {
            faceRight = true;
        } else {
            faceRight = false;
        }

        //orientates held spray bottle to point at cat
        if(currentObject == ObjectHeld.SprayBottle) {
            sprayBottleRot = PointAtSprite(sprayBottlePos.X, sprayBottlePos.Y, (int)catPos.X, (int)catPos.Y);
        }
        if(currentObject == ObjectHeld.SprayBottle && mouseState.LeftButton == ButtonState.Pressed) {
            
        }

        //controls held object
        if(mouseState.LeftButton == ButtonState.Pressed) {
            if(sprayBottleBounds.Contains(mouseState.X, mouseState.Y)) {
                currentObject = ObjectHeld.SprayBottle;
            }
        }
    }

    public void LoadContent(ContentManager _manager, ContentManager _coreAssets)
    {
        //loads cat in at bigger scale for my game
        GameHandler.catIdle = new AnimatedTexture(new Vector2(32,16), 0f, 4f, 0.5f);
        GameHandler.catIdle.Load(_coreAssets, "Sprites/Animal/idle", 7, 5);
        atlas = _manager.Load<Texture2D>("Sprites/petcare_textureatlas");
    }

    public void LoadLevel()
    {
        
    }

    public void Update(GameTime gameTime)
    {
        MouseState mouseState = Mouse.GetState();

        float elapsed = (float)gameTime.ElapsedGameTime.TotalSeconds;
        GameHandler.catIdle.UpdateFrame(elapsed);

        if(currentObject == ObjectHeld.SprayBottle) {
            sprayBottlePos = new Point(mouseState.X, mouseState.Y);
            sprayBottleOrigin = new Vector2(16, 16);
        }
    }

    private float PointAtSprite(int pointX, int pointY, int targetX, int targetY) {
        return (float)Math.Atan2(targetY - pointY, targetX - pointX);
    }
}