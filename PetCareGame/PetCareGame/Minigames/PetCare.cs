using System.Runtime.InteropServices;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using Microsoft.Xna.Framework.Audio;

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

    enum GameStage {
        Instructions,
        Idle,
        Bath,
        NailTrim,
        Brushing
    }
    private Vector2 catPos = new Vector2(400, 305);
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

    private Point towelPos = new Point(55,185);
    private Vector2 towelOrigin = Vector2.Zero;
    private Rectangle towelBounds;

    private bool mouseDown = false;

    private bool faceRight = true;
    
    private List<Particle> particles = new List<Particle>();

    private ObjectHeld currentObject = ObjectHeld.None;
    private GameStage currentStage = GameStage.Instructions;
    private Button startButton;
    private Point startButtonPos = new Point(270,510);
    private Rectangle startButtonBounds;
    private ProgressGauge progressGauge;
    private ProgressGauge gameInputGauge;

    private Goal nailGoal;
    private bool nailGoalComplete = false;
    

    
    

    private Rectangle catNailsBounds = new Rectangle(330,430, 130, 75);

    private SoundEffectInstance catPurr;

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

        //sets line spacing to avoid cluttering of text
        GameHandler.highPixel22.LineSpacing = 35;
        
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

        //draw cat

        //cat attack
        if(progressGauge.GetValue() <= 0) {
            GameHandler.catAttack.DrawFrame(spriteBatch, catPos, SpriteEffects.None, 6f);
        //cat irritated
        } else if(progressGauge.GetValue() <= 4) {
            GameHandler.catIrritated.DrawFrame(spriteBatch, catPos, SpriteEffects.None, 6f);
        //cat idle
        } else if(progressGauge.GetValue() > 4){
            GameHandler.catIdle.DrawFrame(spriteBatch, catPos, SpriteEffects.None, 6f);
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
        spriteBatch.Draw(atlas, new Rectangle(75,150,96,96), towelHook, Color.White);
        
        towelBounds = new Rectangle(towelPos, new Point(128,128));
        spriteBatch.Draw(atlas, towelBounds, towel, Color.White, 0f, towelOrigin, SpriteEffects.None, 1f);

        //draw toolbar background
        spriteBatch.Draw(GameHandler.plainWhiteTexture, new Rectangle(0,0, 800, 150), new Color(45,45,45,150));

        //angry cat icon
        spriteBatch.Draw(atlas, new Rectangle(20, 80, 64, 64), new Rectangle(96,64,32,32), Color.White);
        //happy cat icon
        spriteBatch.Draw(atlas, new Rectangle(270, 80, 64, 64), new Rectangle(64,64,32,32), Color.White);

        //draw temperment meter
        progressGauge.Draw(gameTime, spriteBatch);

        // HANDLE GAME STAGES HERE

        //Instructions
        if(currentStage == GameStage.Instructions) {
            spriteBatch.Draw(
                GameHandler.plainWhiteTexture,
                new Rectangle(0,0,(int)GameHandler.baseScreenSize.X,(int)GameHandler.baseScreenSize.Y),
                Color.LightPink
            );

            spriteBatch.DrawString(
                GameHandler.highPixel36,
                "Instructions",
                new Vector2(240,50),
                Color.Black
            );

            spriteBatch.DrawString(
                GameHandler.highPixel22,
                """
                Owning a pet requires some work!
                Your pet needs your help to stay
                clean and happy. Your pet needs
                you to brush their fur, trim their
                nails, and give them a gentle bath.
                But watch out! The gauge at the top
                represents happiness. If it strays
                into the red, your pet will get
                very upset and you will need to
                start over! Good Luck!
                """,
                new Vector2(100, 150),
                Color.Black
            );

            spriteBatch.Draw(GameHandler.coreTextureAtlas, startButtonBounds, new Rectangle(16,0,16,16), Color.White);
            spriteBatch.DrawString(GameHandler.highPixel22, "Start", new Vector2(350, startButtonPos.Y+25), Color.Black);
        } else if(currentStage == GameStage.NailTrim) {
            //calls draw for input gauge - will only draw if isVisible == true
            gameInputGauge.Draw(gameTime, spriteBatch);
            nailGoal.DrawOutput(spriteBatch, GameHandler.highPixel22, new Vector2(355, 120), Color.Black, "Nails");
        }
    }

    public void HandleInput(GameTime gameTime)
    {
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
        if(currentObject == ObjectHeld.SprayBottle && GameHandler._mouseState.LeftButton == ButtonState.Pressed) {
            /*Console.WriteLine(cooldown.Add(cooldownBuffer).Millisecond);
            Console.WriteLine(DateTime.Now.Millisecond);
            if(cooldown.Add(cooldownBuffer).Millisecond <= DateTime.Now.Millisecond) {*/

                //particles.Add(new Particle((int)GameHandler.relativeMousePos.X, (int)GameHandler.relativeMousePos.Y, (int)catPos.X, (int)catPos.Y, 20, 10, 10, particleTex));
                particles.Add(new Particle((int)GameHandler._mouseState.X, (int)GameHandler._mouseState.Y, (int)catPos.X, (int)catPos.Y, 20, 10, 10, particleTex));
            //}
        }

        //closes clipper when used  
        if(currentObject == ObjectHeld.NailClippers && GameHandler._mouseState.LeftButton == ButtonState.Pressed) {
            clippersUse = true;
        } else {
            clippersUse = false;
        }

        
        if(GameHandler._mouseState.LeftButton == ButtonState.Pressed) {
            /***
            if(sprayBottleBounds.Contains(GameHandler.relativeMousePos.X, GameHandler.relativeMousePos.Y)) {
                currentObject = ObjectHeld.SprayBottle;
            } else if(clipperBounds.Contains(GameHandler.relativeMousePos.X, GameHandler.relativeMousePos.Y)) {
                currentObject = ObjectHeld.NailClippers;
            } else if(towelBounds.Contains(GameHandler.relativeMousePos.X, GameHandler.relativeMousePos.Y)) {
                currentObject = ObjectHeld.Towel;
            }
            ***/
            if(!mouseDown) {
                mouseDown = true;

                //if instructions are being displayed
                if(currentStage == GameStage.Instructions) {
                    //if start button is clicked
                    if(startButton.CheckIfButtonWasClicked()) {
                        currentStage = GameStage.Idle;
                    }
                //if game stage is nail trimming
                } else if(currentStage == GameStage.NailTrim) {
                    //check if use has successfully entered input

                    if (
                        (catNailsBounds.X < clipperBounds.X && catNailsBounds.X + catNailsBounds.Width > clipperBounds.X &&
                        catNailsBounds.Y < clipperBounds.Y && catNailsBounds.Y + catNailsBounds.Height > clipperBounds.Y) || 
                        (clipperBounds.X < catNailsBounds.X && clipperBounds.X + clipperBounds.Width > catNailsBounds.X &&
                        clipperBounds.Y < catNailsBounds.Y && clipperBounds.Y + clipperBounds.Height > catNailsBounds.Y)
                    ){
                        int returnState = gameInputGauge.CheckForSuccess();
                        if(returnState == 1) {
                            progressGauge.Increment();
                            //returns true if goal met
                            if(nailGoal.Increment()) {
                                nailGoalComplete = true;
                            }
                        } else if(returnState == -1) {
                            //returns false when unable to decrease anymore
                            if(!progressGauge.Decrement(2)) {
                                nailGoal.ResetGoal();
                            }
                        }
                    }
                }

                //controls held object
                if(currentObject == ObjectHeld.None && currentStage == GameStage.Idle) {
                    if(sprayBottleBounds.Contains(GameHandler._mouseState.X, GameHandler._mouseState.Y)) {
                        currentObject = ObjectHeld.SprayBottle;
                        currentStage = GameStage.Bath;
                    } else if(towelBounds.Contains(GameHandler._mouseState.X, GameHandler._mouseState.Y)) {
                        currentObject = ObjectHeld.Towel;
                        currentStage = GameStage.Bath;
                    } else if(clipperBounds.Contains(GameHandler._mouseState.X, GameHandler._mouseState.Y)) {
                        //sets held object to nail clippers
                        currentObject = ObjectHeld.NailClippers;
                        //sets game stage to nail trimming
                        currentStage = GameStage.NailTrim;
                        //shows input gauge
                        gameInputGauge.SetVisibility(true);
                    }
                }
            }
        } else if(GameHandler._mouseState.LeftButton == ButtonState.Released) {
            mouseDown = false;
        }
    }

    public void LoadContent(ContentManager _manager, ContentManager _coreAssets)
    {
        //loads cat in at bigger scale for my game
        //GameHandler.catIdle = new AnimatedTexture(new Vector2(32,16), 0f, 5f, 0.5f);
        //GameHandler.catIdle.Load(_coreAssets, "Sprites/Animal/idle", 7, 5);
        atlas = _manager.Load<Texture2D>("Sprites/petcare_textureatlas");
        particleTex = GameHandler.plainWhiteTexture;
        startButton = new Button(GameHandler.coreTextureAtlas, GameHandler.coreTextureAtlas, new Point(250, 72), new Vector2(startButtonPos.X,startButtonPos.Y), "Start", 42, true);
        
        catPurr = GameHandler.catPurr.CreateInstance();
        catPurr.IsLooped = true;
    }

    public void LoadLevel()
    {
        //create gauges
        progressGauge = new ProgressGauge(new Rectangle(25, 20, 300, 60), 0, 16, 8, ProgressGauge.GaugeType.GoodBad, true);
        gameInputGauge = new ProgressGauge(new Rectangle(350, 20, 300, 60), 0, 30, 15, ProgressGauge.GaugeType.HitInRange, false);
        startButtonBounds = new Rectangle(startButtonPos.X, startButtonPos.Y, 250, 72);
        nailGoal = new Goal(10);
    }

    public void Update(GameTime gameTime)
    {
        //game is paused
        if(GameHandler.isPaused) {
            catPurr.Pause();

        //game is running
        } else {
            progressGauge.Update(gameTime);

            //no game has been started
            if(currentStage == GameStage.Idle) {
                catPurr.Play();
            } else if(currentStage == GameStage.NailTrim) {
                if(nailGoalComplete) {
                    gameInputGauge.SetVisibility(false);
                }
                gameInputGauge.Update(gameTime);
            }
            //spray bottle held
            if(currentObject == ObjectHeld.SprayBottle) {
                //sprayBottlePos = new Point((int)GameHandler.relativeMousePos.X, (int)GameHandler.relativeMousePos.Y);
                
                //makes spray bottle snap to mouse and change origin
                sprayBottlePos = new Point((int)GameHandler._mouseState.X, (int)GameHandler._mouseState.Y);
                sprayBottleOrigin = new Vector2(16, 16);
            } else if(currentObject == ObjectHeld.NailClippers) {
                //clippersPos = new Point((int)GameHandler.relativeMousePos.X, (int)GameHandler.relativeMousePos.Y);
                
                //snaps clippers to mouse and changes origin
                clippersPos = new Point((int)GameHandler._mouseState.X, (int)GameHandler._mouseState.Y);
                clippersOrigin = new Vector2(16, 8);
            } else if(currentObject == ObjectHeld.Towel) {
                //towelPos = new Point((int)GameHandler.relativeMousePos.X, (int)GameHandler.relativeMousePos.Y);
                
                //snaps towel to mouse and changes origin
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

    
}