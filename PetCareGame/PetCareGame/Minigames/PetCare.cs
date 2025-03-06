using System.Runtime.InteropServices;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Diagnostics;

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
    private Vector2 catPos = new Vector2(400, 335);
    private Color backgroundColour = new Color(197, 118, 38);
    private Texture2D atlas;
    private Texture2D particleTex;

    private Point sprayBottlePos = new Point(64, 385);
    private Vector2 sprayBottleOrigin = Vector2.Zero;
    private Rectangle sprayBottleBounds;
    private float sprayBottleRot = 0f;

    private Point clippersPos = new Point(650, 415);
    private Vector2 clippersOrigin = Vector2.Zero;
    private Rectangle clipperBounds;
    private bool clippersUse = false;

    private Point towelPos = new Point(55,85);
    private Vector2 towelOrigin = Vector2.Zero;
    private Rectangle towelBounds;

    private DateTime cooldown;

    private bool faceRight = true;
    private ObjectHeld currentObject = ObjectHeld.None;
    
    private List<Particle> particles = new List<Particle>();

    private ProgressGauge satisfaction;

    public void Dispose()
    {
        
    }

    public void Draw(GameTime gameTime, SpriteBatch spriteBatch, GraphicsDeviceManager _graphics)
    {
        Rectangle floor = new Rectangle(0, 0, 32, 32);
        Rectangle floorFiller = new Rectangle(0, 12, 16, 16);
        Rectangle wall = new Rectangle(32, 64, 32, 32);
        Rectangle sprayBottle = new Rectangle(64, 0, 32, 32);
        Rectangle clippers = new Rectangle(96, 0, 32, 32);
        Rectangle clippersClosed = new Rectangle(96, 32, 32, 32);
        Rectangle towelHook = new Rectangle(32,32,32,32);
        Rectangle towel = new Rectangle(64,32,32,32);

        _graphics.GraphicsDevice.Clear(backgroundColour);
        
        //draw wall paneling
        for(int h = 0; h < 8; h++) {
            for(int v = 0; v < 4; v++) {
                spriteBatch.Draw(atlas, new Rectangle(h*128, v*128, 128, 128), wall, Color.White, 0f, Vector2.Zero, SpriteEffects.None, 0f);
            }
        }

        //draw flooring
        for(int i = 0; i < 8; i++) {
            spriteBatch.Draw(atlas, new Rectangle(i*128, 480, 128, 128), floor, Color.DimGray, 0f, Vector2.Zero, SpriteEffects.None, 1f);
            spriteBatch.Draw(atlas, new Rectangle(i*128, 500, 128, 128), floorFiller, Color.DimGray, 0f, Vector2.Zero, SpriteEffects.None, 1f);
        }

        satisfaction.Draw(gameTime, spriteBatch);

        //draw cat
        if(faceRight) {
            GameHandler.catIdle.DrawFrame(spriteBatch, catPos, SpriteEffects.None, 5f);
        } else {
            GameHandler.catIdle.DrawFrame(spriteBatch, catPos, SpriteEffects.FlipHorizontally, 5f);
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

        //render water particles
        for(int i = 0; i < particles.Count; i++) {
            particles[i].Draw(gameTime, spriteBatch, _graphics, Color.SkyBlue);
        }

        //bounding box of clippers
        clipperBounds = new Rectangle(clippersPos, new Point(72, 72));

        //draw clippers
        if(clippersUse) {
            spriteBatch.Draw(atlas, clipperBounds, clippersClosed, Color.White, 0f, clippersOrigin, SpriteEffects.None, 1f);
        } else {
            spriteBatch.Draw(atlas, clipperBounds, clippers, Color.White, 0f, clippersOrigin, SpriteEffects.None, 1f);
        }

        //draw towel hook
        spriteBatch.Draw(atlas, new Rectangle(75,50,96,96), towelHook, Color.White);
        
        towelBounds = new Rectangle(towelPos, new Point(128,128));
        spriteBatch.Draw(atlas, towelBounds, towel, Color.White, 0f, towelOrigin, SpriteEffects.None, 1f);
    }

    public void HandleInput(GameTime gameTime)
    {
        MouseState mouseState = Mouse.GetState();

        //makes cat face in mouse's direction
        //if(GameHandler.relativeMousePos.X > 400) {
        if(GameHandler._mouseState.X > 400) {
            faceRight = true;
        } else {
            faceRight = false;
        }

        //orientates held spray bottle to point at cat
        if(currentObject == ObjectHeld.SprayBottle) {
            sprayBottleRot = Particle.PointAtSprite(sprayBottlePos.X, sprayBottlePos.Y, (int)catPos.X, (int)catPos.Y);
        }

        TimeSpan cooldownBuffer = new TimeSpan(3L);

        //Console.WriteLine(cooldown.Milliseconds);
        //sprays water particles
        if(currentObject == ObjectHeld.SprayBottle && mouseState.LeftButton == ButtonState.Pressed) {
            /*Console.WriteLine(cooldown.Add(cooldownBuffer).Millisecond);
            Console.WriteLine(DateTime.Now.Millisecond);
            if(cooldown.Add(cooldownBuffer).Millisecond <= DateTime.Now.Millisecond) {*/

                //particles.Add(new Particle((int)GameHandler.relativeMousePos.X, (int)GameHandler.relativeMousePos.Y, (int)catPos.X, (int)catPos.Y, 20, 10, 10, particleTex));
                particles.Add(new Particle((int)GameHandler._mouseState.X, (int)GameHandler._mouseState.Y, (int)catPos.X, (int)catPos.Y, 20, 10, 10, particleTex));
                cooldown = DateTime.Now;
            //}
        }

        //closes clipper when used  
        if(currentObject == ObjectHeld.NailClippers && mouseState.LeftButton == ButtonState.Pressed) {
            clippersUse = true;
        } else {
            clippersUse = false;
        }

        //controls held object
        if(mouseState.LeftButton == ButtonState.Pressed) {
            /***
            if(sprayBottleBounds.Contains(GameHandler.relativeMousePos.X, GameHandler.relativeMousePos.Y)) {
                currentObject = ObjectHeld.SprayBottle;
            } else if(clipperBounds.Contains(GameHandler.relativeMousePos.X, GameHandler.relativeMousePos.Y)) {
                currentObject = ObjectHeld.NailClippers;
            } else if(towelBounds.Contains(GameHandler.relativeMousePos.X, GameHandler.relativeMousePos.Y)) {
                currentObject = ObjectHeld.Towel;
            }
            ***/
            if(sprayBottleBounds.Contains(GameHandler._mouseState.X, GameHandler._mouseState.Y)) {
                currentObject = ObjectHeld.SprayBottle;
            } else if(clipperBounds.Contains(GameHandler._mouseState.X, GameHandler._mouseState.Y)) {
                currentObject = ObjectHeld.NailClippers;
            } else if(towelBounds.Contains(GameHandler._mouseState.X, GameHandler._mouseState.Y)) {
                currentObject = ObjectHeld.Towel;
            }
        }
    }

    public void LoadContent(ContentManager _manager, ContentManager _coreAssets)
    {
        //loads cat in at bigger scale for my game
        //GameHandler.catIdle = new AnimatedTexture(new Vector2(32,16), 0f, 5f, 0.5f);
        //GameHandler.catIdle.Load(_coreAssets, "Sprites/Animal/idle", 7, 5);
        atlas = _manager.Load<Texture2D>("Sprites/petcare_textureatlas");
        particleTex = GameHandler.plainWhiteTexture;
    }

    public void LoadLevel()
    {
        satisfaction = new ProgressGauge(new Rectangle(300, 20, 300, 60), 0, 10, 0);
    }

    public void Update(GameTime gameTime)
    {
        MouseState mouseState = Mouse.GetState();

        float elapsed = (float)gameTime.ElapsedGameTime.TotalSeconds;
        GameHandler.catIdle.UpdateFrame(elapsed);

        if(currentObject == ObjectHeld.SprayBottle) {
            //sprayBottlePos = new Point((int)GameHandler.relativeMousePos.X, (int)GameHandler.relativeMousePos.Y);
            sprayBottlePos = new Point((int)GameHandler._mouseState.X, (int)GameHandler._mouseState.Y);
            sprayBottleOrigin = new Vector2(16, 16);
        } else if(currentObject == ObjectHeld.NailClippers) {
            //clippersPos = new Point((int)GameHandler.relativeMousePos.X, (int)GameHandler.relativeMousePos.Y);
            clippersPos = new Point((int)GameHandler._mouseState.X, (int)GameHandler._mouseState.Y);
            clippersOrigin = new Vector2(16, 8);
        } else if(currentObject == ObjectHeld.Towel) {
            //towelPos = new Point((int)GameHandler.relativeMousePos.X, (int)GameHandler.relativeMousePos.Y);
            towelPos = new Point((int)GameHandler._mouseState.X, (int)GameHandler._mouseState.Y);
            towelOrigin = new Vector2(16, 8);
        }

        int index = 0;
        while (index < particles.Count) {
            particles[index].Update(gameTime);
            if(particles[index].CheckToDestroy(5)) {
                particles.RemoveAt(index);
            } else {
                index++;
            }
        }
    }

    
}