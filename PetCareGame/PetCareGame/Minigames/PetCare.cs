using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
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
    private Rectangle catBounds;
    private Color backgroundColour = new Color(197, 118, 38);
    private Texture2D atlas;
    private Texture2D particleTex;

    private Point sprayBottlePos = new Point(64, 385);
    private Vector2 sprayBottleOrigin = Vector2.Zero;
    private Rectangle sprayBottleBounds;
    private Rectangle jumpBounds = new Rectangle(530,100,150,450);
    private Rectangle jumpGate = new Rectangle(100,100,50,450);
    private bool isJumping = false;
    private bool allowJump = false;
    private bool waiting = false;
    private double jumpCooldown = 0;
    private int jumpCooldownDuration;
    private int jumpFrame = 2;

    private Random rand = new Random();
    
    //use this to alternate between the 3 possible heights cat can jump to
    private int jumpIndex = 0;
    private int prevJumpIndex = -1;

    private Point clippersPos = new Point(650, 415);
    private Vector2 clippersOrigin = Vector2.Zero;
    private Rectangle clipperBounds;
    private bool clippersUse = false;

    private Point towelPos = new Point(55,185);
    private Vector2 towelOrigin = Vector2.Zero;
    private Rectangle towelBounds;
    
    private Point brushPos = new Point(600,200);
    private Vector2 brushOrigin = Vector2.Zero;
    private Rectangle brushBounds;
    private Point brushHeadOffset;

    private bool mouseDown = false;
    
    private List<Particle> particles = new List<Particle>();

    private ObjectHeld currentObject = ObjectHeld.None;
    private GameStage currentStage = GameStage.Instructions;
    private Button startButton;
    private Point startButtonPos = new Point(270,510);
    private Rectangle startButtonBounds;
    private ProgressGauge tempermentGauge;
    private ProgressGauge gameInputGauge;
    private ProgressGauge progressGauge;

    private Goal nailGoal = new Goal(10);
    private bool brushGoal = false;

    private Rectangle brushPoint1 = new Rectangle(385, 270, 50, 50);
    private Rectangle brushPoint2 = new Rectangle(340, 420, 40, 60);
    private Rectangle brushPoint3 = new Rectangle(410, 380, 40, 60);
    private AnimatedTexture hotspot1 = new AnimatedTexture(new Vector2(16,16), 0f, 2f, 1f);
    private AnimatedTexture hotspot2 = new AnimatedTexture(new Vector2(16,16), 0f, 2f, 1f);
    private AnimatedTexture hotspot3 = new AnimatedTexture(new Vector2(16,16), 0f, 2f, 1f);
    private AnimatedTexture catJump = new AnimatedTexture(new Vector2(32,16), 0f, 3f, 0.5f);
    private double hsCooldown1 = 0;
    private double hsCooldown2 = 0;
    private double hsCooldown3 = 0;
    private double progressCooldown = 0;
    private int hotspot1Frame = 0;
    private int hotspot2Frame = 0;
    private int hotspot3Frame = 0;

    
    
    private Rectangle catNailsBounds = new Rectangle(330,430, 130, 75);

    private SoundEffectInstance catPurr;
    private SoundEffectInstance snipSfx;
    private SoundEffectInstance brushSfx;
    private SoundEffectInstance spraySfx;

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
        Rectangle brushHanging = new Rectangle(0,64,32,32);
        Rectangle brushHeld = new Rectangle(0,32,32,32);
        Rectangle markX = new Rectangle(48,0,16,16);
        Rectangle checkbox = new Rectangle(16,16,16,16);
        Rectangle checkmark = new Rectangle(0,16,16,16);

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

        if(currentStage == GameStage.Bath) {
            if(isJumping) {
                catJump.DrawFrame(spriteBatch, jumpFrame, catPos, SpriteEffects.None, 6f);
            } else {
                GameHandler.catRun.DrawFrame(spriteBatch, catPos, SpriteEffects.None, 6f);
            }
        } else {
            //cat attack
            if(tempermentGauge.GetValue() <= 0) {
                GameHandler.catAttack.DrawFrame(spriteBatch, catPos, SpriteEffects.None, 6f);
            //cat irritated
            } else if(tempermentGauge.GetValue() <= 4) {
                GameHandler.catIrritated.DrawFrame(spriteBatch, catPos, SpriteEffects.None, 6f);
            //cat idle
            } else if(tempermentGauge.GetValue() > 4){
                GameHandler.catIdle.DrawFrame(spriteBatch, catPos, SpriteEffects.None, 6f);
            }
        }
        

        //bounding box of spray bottle
        sprayBottleBounds = new Rectangle(sprayBottlePos, new Point(96, 96));

        spriteBatch.Draw(atlas, sprayBottleBounds, sprayBottle, Color.White, 0f, sprayBottleOrigin, SpriteEffects.None, 1f);

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

        //if nail goal is completed, draw X over nail clippers to indicate disabled
        if(nailGoal.GetCompletion()) {
            spriteBatch.Draw(GameHandler.coreTextureAtlas, clipperBounds, markX, Color.SlateGray);
        }

        //draw brush when brushing stage not active: hanging
        brushBounds = new Rectangle(brushPos, new Point(96,96));
        if(currentStage != GameStage.Brushing) {//brush is held, use held sprite
            spriteBatch.Draw(atlas, brushBounds, brushHanging, Color.White, 0f, brushOrigin, SpriteEffects.None, 1f);
        }

        //if brush goal is completed, draw X over brush to indicate disabled
        if(brushGoal) {
            spriteBatch.Draw(GameHandler.coreTextureAtlas, brushBounds, markX, Color.SlateGray);
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
        tempermentGauge.Draw(gameTime, spriteBatch);

        // HANDLE DRAWING GAME STAGES HERE

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
        } else if(currentStage == GameStage.Brushing) {
            /*** //debug for brush zones
            spriteBatch.Draw(GameHandler.plainWhiteTexture, brushPoint1, Color.Red);
            spriteBatch.Draw(GameHandler.plainWhiteTexture, brushPoint2, Color.Green);
            spriteBatch.Draw(GameHandler.plainWhiteTexture, brushPoint3, Color.Blue);***/

            progressGauge.Draw(gameTime, spriteBatch);

            spriteBatch.Draw(atlas, brushBounds, brushHeld, Color.White, 0f, brushOrigin, SpriteEffects.None, 1f);

            //use to draw the points for brush contact
            hotspot1.DrawFrame(spriteBatch, hotspot1Frame, new Vector2(410,295), SpriteEffects.None);
            hotspot2.DrawFrame(spriteBatch, hotspot2Frame, new Vector2(360,450), SpriteEffects.None);
            hotspot3.DrawFrame(spriteBatch, hotspot3Frame, new Vector2(430,410), SpriteEffects.None);

            //debug for brush head
            spriteBatch.Draw(GameHandler.plainWhiteTexture, new Rectangle(brushHeadOffset, new Point(8,8)), Color.Lime);
        } else if(currentStage == GameStage.Bath) {
            //debug for cat bounds
            /*
            spriteBatch.Draw(GameHandler.plainWhiteTexture, jumpBounds, Color.Green);
            spriteBatch.Draw(GameHandler.plainWhiteTexture, catBounds, Color.Red);
            spriteBatch.Draw(GameHandler.plainWhiteTexture, jumpGate, Color.Blue);
            */
        }

        //draw checklist on any stage but instructions mode
        if(currentStage != GameStage.Instructions) {
            spriteBatch.Draw(GameHandler.coreTextureAtlas, new Rectangle(20, 525, 64, 64), checkbox, Color.White);
            spriteBatch.DrawString(GameHandler.highPixel22, "Nails", new Vector2(100, 545), Color.White);
            if(nailGoal.GetCompletion()) {
                spriteBatch.Draw(GameHandler.coreTextureAtlas, new Rectangle(20, 525, 64, 64), checkmark, Color.LimeGreen);
            }

            spriteBatch.Draw(GameHandler.coreTextureAtlas, new Rectangle(240, 525, 64, 64), checkbox, Color.White);
            spriteBatch.DrawString(GameHandler.highPixel22, "Brushing", new Vector2(320, 545), Color.White);
            if(brushGoal) {
                spriteBatch.Draw(GameHandler.coreTextureAtlas, new Rectangle(240, 525, 64, 64), checkmark, Color.LimeGreen);
            }

            spriteBatch.Draw(GameHandler.coreTextureAtlas, new Rectangle(520, 525, 64, 64), checkbox, Color.White);
            spriteBatch.DrawString(GameHandler.highPixel22, "Bath", new Vector2(600, 545), Color.White);
            if(false == true) {//put bath goal in here
                spriteBatch.Draw(GameHandler.coreTextureAtlas, new Rectangle(520, 525, 64, 64), checkmark, Color.LimeGreen);
            }
        }

        spriteBatch.DrawString(GameHandler.highPixel22, catPos.ToString(), new Vector2(0,60), Color.Black);
    }

    public void HandleInput(GameTime gameTime)
    {


        //Console.WriteLine(cooldown.Milliseconds);
        //sprays water particles
        if(currentObject == ObjectHeld.SprayBottle && GameHandler.mouseState.LeftButton == ButtonState.Pressed) {
            
        }

        //closes clipper when used  
        if(currentObject == ObjectHeld.NailClippers && GameHandler.mouseState.LeftButton == ButtonState.Pressed) {
            clippersUse = true;
        } else {
            clippersUse = false;
        }

        double cooldownBuffer = 0.25;

        //handle input for different stages in here, switch between them below
        if(GameHandler.mouseState.LeftButton == ButtonState.Pressed) {
            //runs repeatedly while mouse is pressed so you don't have to click
            //over and over

            //Console.WriteLine(hsCooldown1 + 0.5 < gameTime.TotalGameTime.TotalSeconds);
            if(currentStage == GameStage.Brushing) {
                if(brushPoint1.Contains(brushHeadOffset) && (hsCooldown1 + cooldownBuffer < gameTime.TotalGameTime.TotalSeconds)) {
                    if(hotspot1Frame < 8) {
                        hotspot1Frame++;
                    }
                    hsCooldown1 = gameTime.TotalGameTime.TotalSeconds;
                } else if(brushPoint2.Contains(brushHeadOffset) && (hsCooldown2 + cooldownBuffer < gameTime.TotalGameTime.TotalSeconds)) {
                    if(hotspot2Frame < 8) {
                        hotspot2Frame++;
                    }
                    hsCooldown2 = gameTime.TotalGameTime.TotalSeconds;
                } else if(brushPoint3.Contains(brushHeadOffset) && (hsCooldown3 + cooldownBuffer < gameTime.TotalGameTime.TotalSeconds)) {
                    if(hotspot3Frame < 8) {
                        hotspot3Frame++;
                    }
                    hsCooldown3 = gameTime.TotalGameTime.TotalSeconds;
                }

                if(GameHandler.allowAudio && !GameHandler.muted) {
                    if(brushPoint1.Contains(brushHeadOffset) || brushPoint2.Contains(brushHeadOffset) || brushPoint3.Contains(brushHeadOffset)) {
                        brushSfx.IsLooped = true;
                        brushSfx.Play();
                    }
                }
            }
            
            //runs only once after mouse pressed
            if(!mouseDown) {
                mouseDown = true;

                //if instructions are being displayed
                if(currentStage == GameStage.Instructions) {
                    //if start button is clicked
                    if(startButton.CheckIfSelectButtonWasClicked()) {
                        currentStage = GameStage.Idle;
                    }
                //if game stage is nail trimming
                } else if(currentStage == GameStage.NailTrim) {

                    //infer that if nail clipping stage, they are holding trimmers
                    if(GameHandler.allowAudio && !GameHandler.muted) {
                        snipSfx.Play();
                    }
                    //check if use has successfully entered input
                    if (
                        (catNailsBounds.X < clipperBounds.X && catNailsBounds.X + catNailsBounds.Width > clipperBounds.X &&
                        catNailsBounds.Y < clipperBounds.Y && catNailsBounds.Y + catNailsBounds.Height > clipperBounds.Y) || 
                        (clipperBounds.X < catNailsBounds.X && clipperBounds.X + clipperBounds.Width > catNailsBounds.X &&
                        clipperBounds.Y < catNailsBounds.Y && clipperBounds.Y + clipperBounds.Height > catNailsBounds.Y)
                    ) {
                        int returnState = gameInputGauge.CheckForSuccess();
                        if(returnState == 1) {
                            tempermentGauge.Increment();
                            //returns true if goal met
                            if(nailGoal.Increment()) {
                                //isComplete will be marked as true inside Goal
                                nailGoal.SetCompletion(true);

                                //returns to idle stage
                                currentStage = GameStage.Idle;
                                currentObject = ObjectHeld.None;                        
                                clippersPos = new Point(650, 415);
                                clippersOrigin = Vector2.Zero;
                                tempermentGauge.SetCurrentValue(8);
                                Console.WriteLine("TRIMMING stage won, returning to IDLE");
                            }
                        } else if(returnState == -1) {
                            //returns false when unable to decrease anymore
                            if(!tempermentGauge.Decrement(2)) {
                                nailGoal.ResetGoal();
                            }
                        }
                    }
                } else if(currentStage == GameStage.Bath) {//handle input for bath level
                    if(currentObject == ObjectHeld.SprayBottle) {
                        int y = (int)GameHandler.relativeMousePos.Y;
                        if(y > 450) {
                            y = 450;
                        } else if(y < 210) {
                            y = 210;
                        }
                        particles.Add(new Particle(
                            new Point(100, y-60),
                            new Point(16, 8),
                            16,
                            GameHandler.plainWhiteTexture)
                        );

                        if(GameHandler.allowAudio && !GameHandler.muted) {
                            spraySfx.Play();
                        }
                    }
                }


                /***********
                    End of game input handling, begin input handling for stage switching
                **********/

                //Activates gamestate based on object pressed
                if(currentObject == ObjectHeld.None && currentStage == GameStage.Idle) {
                    //switch to bath stage
                    if(sprayBottleBounds.Contains(GameHandler.relativeMousePos.X, GameHandler.relativeMousePos.Y)) {
                        currentObject = ObjectHeld.SprayBottle;
                        currentStage = GameStage.Bath;
                    } else if(towelBounds.Contains(GameHandler.relativeMousePos.X, GameHandler.relativeMousePos.Y)) {
                        currentObject = ObjectHeld.Towel;
                        currentStage = GameStage.Bath;

                    //switch to nail clipping stage
                    } else if(!nailGoal.GetCompletion() && clipperBounds.Contains(GameHandler.relativeMousePos.X, GameHandler.relativeMousePos.Y)) {
                        //sets held object to nail clippers
                        currentObject = ObjectHeld.NailClippers;
                        //sets game stage to nail trimming
                        currentStage = GameStage.NailTrim;
                        //shows input gauge
                        gameInputGauge.SetVisibility(true);
                    //switch to brushing stage
                    } else if(!brushGoal && brushBounds.Contains(GameHandler.relativeMousePos.X, GameHandler.relativeMousePos.Y)) {
                        currentObject = ObjectHeld.Brush;
                        currentStage = GameStage.Brushing;
                    }
                }
            }
        } else if(GameHandler.mouseState.LeftButton == ButtonState.Released) {
            mouseDown = false;
            if(GameHandler.allowAudio) {
                brushSfx.IsLooped = false;
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
        startButton = new Button(GameHandler.coreTextureAtlas, GameHandler.coreTextureAtlas, new Point(250, 72), new Vector2(startButtonPos.X,startButtonPos.Y), "Start", 42, true);

        hotspot1.Load(_manager, "Sprites/hotspot_gauge_32", 9, 1);
        hotspot2.Load(_manager, "Sprites/hotspot_gauge_32", 9, 1);
        hotspot3.Load(_manager, "Sprites/hotspot_gauge_32", 9, 1);
        catJump.Load(_coreAssets, "Sprites/Animal/jump", 7, 15);
        
        //to prevent crashes if audio driver is missing
        if(GameHandler.allowAudio) {
            catPurr = GameHandler.catPurr.CreateInstance();
            catPurr.IsLooped = true;
            snipSfx = _manager.Load<SoundEffect>("Sounds/snip").CreateInstance();
            brushSfx = _manager.Load<SoundEffect>("Sounds/brush").CreateInstance();
            spraySfx = _manager.Load<SoundEffect>("Sounds/spray").CreateInstance();
            brushSfx.IsLooped = true;
        }        
    }

    public void LoadLevel()
    {
        //create gauges
        tempermentGauge = new ProgressGauge(new Rectangle(25, 20, 300, 60), 0, 16, 8, ProgressGauge.GaugeType.GoodBad, true);
        gameInputGauge = new ProgressGauge(new Rectangle(350, 20, 300, 60), 0, 30, 15, ProgressGauge.GaugeType.HitInRange, false);
        progressGauge = new ProgressGauge(new Rectangle(350, 20, 300, 60), 0, 10, 0, ProgressGauge.GaugeType.Progress, false);
        startButtonBounds = new Rectangle(startButtonPos.X, startButtonPos.Y, 250, 72);

        //add statement to not force instructions if played before
    }

    public void Update(GameTime gameTime)
    {
        //game is paused
        if(GameHandler.isPaused) {
            if(GameHandler.allowAudio) {
                catPurr.Pause();
            }

        //game is running
        } else {
            tempermentGauge.Update(gameTime);

            //no game has been started
            if(currentStage == GameStage.Idle) { //no stage running
                if(GameHandler.allowAudio && !GameHandler.muted) {
                    catPurr.Play();
                } else {
                    catPurr.Pause();
                }
            } else if(currentStage == GameStage.NailTrim) { //nail trimming
                if(nailGoal.GetCompletion()) {
                    gameInputGauge.SetVisibility(false);
                }
                gameInputGauge.Update(gameTime);
            } else if(currentStage == GameStage.Brushing) { //brushing
                if(progressGauge.CheckForSuccess() == 1) { //marker has reached end, set goal to true
                    //marks brush goal as complete
                    brushGoal = true;

                    //returns to idle stage
                    progressGauge.SetVisibility(false);
                    currentObject = ObjectHeld.None;
                    currentStage = GameStage.Idle;
                    brushPos = new Point(600,200);
                    brushOrigin = Vector2.Zero;
                    Console.WriteLine("BRUSH stage won, returning to IDLE");                    
                }
                if(!brushGoal) { //brushing goal not reached
                    progressGauge.SetVisibility(true);

                    //handles decrementing hotspots
                    double cooldownBuffer = 1.5;

                    if(hotspot1Frame > 0 && (hsCooldown1 + cooldownBuffer < gameTime.TotalGameTime.TotalSeconds)) {
                        hotspot1Frame--;
                        hsCooldown1 = gameTime.TotalGameTime.TotalSeconds;
                    }
                    if(hotspot2Frame > 0 && (hsCooldown2 + cooldownBuffer < gameTime.TotalGameTime.TotalSeconds)) {
                        hotspot2Frame--;
                        hsCooldown2 = gameTime.TotalGameTime.TotalSeconds;
                    }
                    if(hotspot3Frame > 0 && (hsCooldown3 + cooldownBuffer < gameTime.TotalGameTime.TotalSeconds)) {
                        hotspot3Frame--;
                        hsCooldown3 = gameTime.TotalGameTime.TotalSeconds;
                    }

                    //handles temperment meter
                    if(hotspot1Frame == 8 || hotspot2Frame == 8 || hotspot3Frame == 8) {
                        tempermentGauge.SetCurrentValue(0);
                    } else if (hotspot1Frame >= 6 || hotspot2Frame >= 6 || hotspot3Frame >= 6) {
                        tempermentGauge.SetCurrentValue(4);
                    } else if(
                        hotspot1Frame >= 3 && hotspot1Frame < 6 &&
                        hotspot2Frame >= 3 && hotspot2Frame < 6 &&
                        hotspot3Frame >= 3 && hotspot3Frame < 6
                    ) {
                        tempermentGauge.SetCurrentValue(14);
                    } else {
                        tempermentGauge.SetCurrentValue(8);
                    }

                    //progress cooldown handler
                    if(progressCooldown + 0.75 < gameTime.TotalGameTime.TotalSeconds) {
                        if(tempermentGauge.GetValue() == 4) {
                            progressGauge.Decrement();
                        } else if (tempermentGauge.GetValue() == 0) {
                            progressGauge.Decrement(4);
                        } else if(tempermentGauge.GetValue() == 14) {
                            if(!progressGauge.Increment()) { //win
                                
                            }
                        }
                        progressCooldown = gameTime.TotalGameTime.TotalSeconds;
                    }
                    progressGauge.Update(gameTime);
                }
            } else if(currentStage == GameStage.Bath) {
                if(isJumping && !waiting) {
                    catPos.X += 6;
                } else if(!waiting) {
                    catPos.X += 10;
                //cooldown expires
                } else if(waiting && (jumpCooldown + jumpCooldownDuration) < gameTime.TotalGameTime.TotalSeconds) {
                    waiting = false;
                }
                //prepares for jump and decides height
                if(jumpGate.Contains(catPos)) {
                    
                    //jumpIndex 0 means no jump
                    if(jumpIndex != 0) {
                        allowJump = true;
                    }
                }

                //move back to left side of screen
                if(catPos.X >= 950) {
                    catPos.X = -150;
                    catPos.Y = 305;
                    jumpFrame = 2;
                    isJumping = false;
                    waiting = true;

                    //prevents from receiving the same value multiple times in a row
                    do {
                        jumpIndex = (int)rand.NextInt64(0,3);
                    } while(jumpIndex == prevJumpIndex);
                    Console.WriteLine(jumpIndex);
                    //updates previous index with most recent index
                    prevJumpIndex = jumpIndex;

                    jumpCooldown = gameTime.TotalGameTime.TotalSeconds;
                    jumpCooldownDuration = (int)rand.NextInt64(1,6);
                }
                if(catPos.X >= 405 && allowJump) {
                    isJumping = true;
                    catPos.Y = 305;
                }

                if(isJumping) {
                    switch(jumpIndex) {
                        case 1:
                            catPos.Y = (int)GetJumpY(catPos.X, 150);
                            break;
                        case 2:
                            catPos.Y = (int)GetJumpY(catPos.X, 20);
                            break;
                    }
                    //sets back to running state once it's on ground again
                    if(catPos.Y > 304) {
                        isJumping = false;
                    }
                }

                //controls jump animation frames
                if(catPos.X >= 750) {
                    jumpFrame = 6;
                } else if(catPos.X >= 700) {
                    jumpFrame = 5;
                } else if(catPos.X >= 650) {
                    jumpFrame = 4;
                } else if(catPos.X >= 600) {
                    jumpFrame = 3;
                }
                catBounds = new Rectangle((int)(catPos.X-50), (int)(catPos.Y-20), 100, 200);

                int index = 0;
                while (index < particles.Count) {
                    particles[index].Update(gameTime);
                    if(particles[index].CheckToDestroy(catBounds)) {
                        particles.RemoveAt(index);
                    } else {
                        index++;
                    }
                }
            }

            //spray bottle held
            if(currentObject == ObjectHeld.SprayBottle) {
                int y = (int)GameHandler.relativeMousePos.Y;

                if(y > 450) {
                    y = 450;
                } else if(y < 210) {
                    y = 210;
                }
                sprayBottlePos = new Point(64, y);

                sprayBottleOrigin = new Vector2(0,20);

            } else if(currentObject == ObjectHeld.NailClippers) {
                clippersPos = new Point((int)GameHandler.relativeMousePos.X, (int)GameHandler.relativeMousePos.Y);
                
                //snaps clippers to mouse and changes origin
                clippersOrigin = new Vector2(16, 8);
            } else if(currentObject == ObjectHeld.Towel) {
                //snaps towel to mouse and changes origin
                towelPos = new Point((int)GameHandler.relativeMousePos.X, (int)GameHandler.relativeMousePos.Y);
                towelOrigin = new Vector2(16, 8);
            } else if(currentObject == ObjectHeld.Brush) {
                brushPos = new Point((int)GameHandler.relativeMousePos.X, (int)GameHandler.relativeMousePos.Y);
                brushOrigin = new Vector2(28, 10);
                brushHeadOffset = new Point((int)GameHandler.relativeMousePos.X - 55, (int)GameHandler.relativeMousePos.Y + 20);
            }
        }
    }

    private float GetJumpY(float posX, float destHeight) {
        float destX = 600f;
        int jumpStart = 400;
        int groundLevel = 305;
        float a = (groundLevel - destHeight) / MathF.Pow(jumpStart - destX, 2);
        float y = a * MathF.Pow(posX - destX, 2) + destHeight;

        
        if(y >= groundLevel) {
            return groundLevel;
        } 

        
        return y;
    }

    public void CleanupProcesses()
    {
        catPos = new Vector2(400, 305);
        sprayBottlePos = new Point(64, 385);
        sprayBottleOrigin = Vector2.Zero;
        isJumping = false;
        allowJump = false;
        waiting = false;
        jumpFrame = 2;
        prevJumpIndex = -1;

        clippersPos = new Point(650, 415);
        clippersOrigin = Vector2.Zero;
        clippersUse = false;

        towelPos = new Point(55,185);
        towelOrigin = Vector2.Zero;

        brushPos = new Point(600,200);
        brushOrigin = Vector2.Zero;

        mouseDown = false;
        
        currentObject = ObjectHeld.None;
        currentStage = GameStage.Instructions;
        startButtonPos = new Point(270,510);

        hotspot1Frame = 0;
        hotspot2Frame = 0;
        hotspot3Frame = 0;

        tempermentGauge.SetCurrentValue(8);
    }

    public void SaveData()
    {
        
    }

    public void LoadData()
    {
        
    }
}