using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework.Audio;
using System.Linq;

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
        BathWashing,
        BathDrying,
        NailTrim,
        Brushing,
        Win
    }

    enum InfoPanel {
        SummaryPanel,
        BathInfoPanel,
        BrushInfoPanel,
        NailsInfoPanel
    }
    private Vector2 catPos = new Vector2(400, 305);
    private Rectangle catBounds;
    private Color backgroundColour = new Color(197, 118, 38);
    private Texture2D atlas;

    private Point sprayBottlePos = new Point(64, 385);
    private Vector2 sprayBottleOrigin = Vector2.Zero;
    private Rectangle sprayBottleBounds;
    private Rectangle jumpBounds = new Rectangle(530,100,150,400);
    private Rectangle jumpGate = new Rectangle(100,100,50,450);
    private bool isJumping = false;
    private bool allowJump = false;
    private bool waiting = false;
    private bool finalLap = false;
    private bool failState = false;
    private double sprayCooldown = 0;
    private double jumpCooldown = 0;
    private int jumpCooldownDuration;
    private int jumpFrame = 2;
    private int water = 16;

    private readonly Random rand = new();
    
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
    private bool isReady = false;
    
    private Point brushPos = new Point(600,200);
    private Vector2 brushOrigin = Vector2.Zero;
    private Rectangle brushBounds;
    private Point brushHeadOffset;

    private Vector2 waterDropOrigin = new Vector2(16,0);
    private Vector2 waterDropPos;
    private float waterDropScale = 0f;
    private double simonCooldown = 0;
    private double simonDelay = 2;
    private int maxSimonIterations = 5;
    private List<Rectangle> spotOrder = new List<Rectangle>();
    private int spotIndex = 0;
    private int simonIteration = 0;
    private bool isPlayerTurn = false;
    

    private bool mouseDown = false;
    
    private List<Particle> particles = new List<Particle>();

    private ObjectHeld currentObject = ObjectHeld.None;
    private GameStage currentStage = GameStage.Instructions;
    private InfoPanel currentInfo = InfoPanel.SummaryPanel;

    private ProgressGauge tempermentGauge;
    private ProgressGauge gameInputGauge;
    private ProgressGauge progressGauge;

    private Goal nailGoal = new Goal(10);
    private bool brushGoal = false;
    private Goal sprayGoal = new Goal(5);
    private Goal dryGoal;

    private Button startButton;
    private Button infoBath;
    private Button infoBrush;
    private Button infoNails;
    private Button returnButton;

    private Point startButtonPos = new Point(540,510);
    private Point infoBathPos = new Point(25,510);
    private Point infoBrushPos = new Point(185,510);
    private Point infoNailsPos = new Point(380,510);
    private Point returnButtonPos = new Point(250,510);

    private Rectangle startButtonBounds;
    private Rectangle infoBathBounds;
    private Rectangle infoBrushBounds;
    private Rectangle infoNailsBounds;
    private Rectangle returnButtonBounds;
    

    private Rectangle hotspotBounds1 = new Rectangle(385, 270, 50, 50);
    private Rectangle hotspotBounds2 = new Rectangle(340, 420, 40, 60);
    private Rectangle hotspotBounds3 = new Rectangle(410, 380, 40, 60);

    private Rectangle dryspotBounds1 = new Rectangle(365, 270, 50, 50);
    private Rectangle dryspotBounds2 = new Rectangle(340, 420, 50, 50);
    private Rectangle dryspotBounds3 = new Rectangle(410, 380, 50, 50);
    private Rectangle dryspotBounds4 = new Rectangle(415, 320, 50, 50);

    private Rectangle lightBounds = new Rectangle(175, 245, 128, 128);
    

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
    private SoundEffectInstance smallWin;
    private SoundEffectInstance pixelMusic18;
    private SoundEffectInstance pixelMusic13;
    private SoundEffectInstance currentSong;
    private SoundEffect waterDrip;

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
        Rectangle sprayZone = new Rectangle(0,96,32,32);
        Rectangle wetSpot = new Rectangle(64,96,32,32);
        Rectangle waterDrop = new Rectangle(32,96,32,32);
        Rectangle lightHousing = new Rectangle(96,96,32,32);
        Rectangle lightBacking = new Rectangle(0,128,32,32);
        Rectangle wetSpotRing = new Rectangle(32,128,32,32);

        _graphics.GraphicsDevice.Clear(backgroundColour);
        
        //draw wall paneling
        for(int h = 0; h < 8; h++) {
            for(int v = 0; v < 4; v++) {
                spriteBatch.Draw(atlas, new Rectangle(h*128, v*128, 128, 128), wall, Color.White, 0f, Vector2.Zero, SpriteEffects.None, 0f);
            }
        }

        if(currentStage == GameStage.BathWashing) {
            spriteBatch.Draw(atlas, jumpBounds, sprayZone, Color.Green);
        }

        //draw flooring
        for(int i = 0; i < 8; i++) {
            spriteBatch.Draw(atlas, new Rectangle(i*128, 480, 128, 128), floor, Color.DimGray, 0f, Vector2.Zero, SpriteEffects.None, 1f);
            spriteBatch.Draw(atlas, new Rectangle(i*128, 500, 128, 128), floorFiller, Color.DimGray, 0f, Vector2.Zero, SpriteEffects.None, 1f);
        }

        //draw cat

        if(currentStage == GameStage.BathWashing) {
            if(isJumping) {
                catJump.DrawFrame(spriteBatch, jumpFrame, catPos, SpriteEffects.None, 6f);
            } else {
                GameHandler.catRun.DrawFrame(spriteBatch, catPos, SpriteEffects.None, 6f);
            }
        } else {
            //cat attack
            if(tempermentGauge.GetCurrentValue() <= 0) {
                GameHandler.catAttack.DrawFrame(spriteBatch, catPos, SpriteEffects.None, 6f);
            //cat irritated
            } else if(tempermentGauge.GetCurrentValue() <= 4) {
                GameHandler.catIrritated.DrawFrame(spriteBatch, catPos, SpriteEffects.None, 6f);
            //cat idle
            } else if(tempermentGauge.GetCurrentValue() > 4){
                GameHandler.catIdle.DrawFrame(spriteBatch, catPos, SpriteEffects.None, 6f);
            }
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
        
        //draw visuals for drying spots
        if(currentStage == GameStage.BathDrying && isReady) {
            /***
            //debug draw dry hotspots
            spriteBatch.Draw(GameHandler.plainWhiteTexture, dryspotBounds1, Color.Red);
            spriteBatch.Draw(GameHandler.plainWhiteTexture, dryspotBounds2, Color.Green);
            spriteBatch.Draw(GameHandler.plainWhiteTexture, dryspotBounds3, Color.Blue);
            spriteBatch.Draw(GameHandler.plainWhiteTexture, dryspotBounds4, Color.Yellow);
            ***/


            
            if(!dryGoal.GetCompletion()) {
                //draw traffic light dim lights
                spriteBatch.Draw(atlas, lightBounds, lightBacking, Color.White);
            
                //draws illuminated lights
                if(isPlayerTurn) { //green light
                    spriteBatch.Draw(GameHandler.plainWhiteTexture, new Rectangle(220,330,40,40), Color.Lime);
                } else if(!isPlayerTurn && (simonCooldown + simonDelay > gameTime.TotalGameTime.TotalSeconds)) {
                    spriteBatch.Draw(GameHandler.plainWhiteTexture, new Rectangle(220,250,40,40), Color.Red);
                } else { //yellow light - sequence is being displayed
                    spriteBatch.Draw(GameHandler.plainWhiteTexture, new Rectangle(220,295,40,40), Color.Yellow);
                }

                //draw traffic light housing
                spriteBatch.Draw(atlas, lightBounds, lightHousing, Color.White);

                Color waterColour = new Color(33,216,243, 255);
                spriteBatch.Draw(atlas,dryspotBounds1, wetSpot, waterColour);
                spriteBatch.Draw(atlas,dryspotBounds2, wetSpot, waterColour);
                spriteBatch.Draw(atlas,dryspotBounds3, wetSpot, waterColour);
                spriteBatch.Draw(atlas,dryspotBounds4, wetSpot, waterColour);

                //draws selection ring around spot being triggered
                if(GameHandler.mouseState.LeftButton == ButtonState.Pressed && !failState) {
                    if(dryspotBounds1.Contains(GameHandler.relativeMousePos)) {
                        spriteBatch.Draw(atlas, dryspotBounds1, wetSpotRing, Color.White);
                    } else if(dryspotBounds2.Contains(GameHandler.relativeMousePos)) {
                        spriteBatch.Draw(atlas, dryspotBounds2, wetSpotRing, Color.White);
                    } else if(dryspotBounds3.Contains(GameHandler.relativeMousePos)) {
                        spriteBatch.Draw(atlas, dryspotBounds3, wetSpotRing, Color.White);
                    } else if(dryspotBounds4.Contains(GameHandler.relativeMousePos)) {
                        spriteBatch.Draw(atlas, dryspotBounds4, wetSpotRing, Color.White);
                    }
                }

                //draw water drop
                spriteBatch.Draw(atlas, waterDropPos, waterDrop, Color.White, 0f, waterDropOrigin, waterDropScale, SpriteEffects.None, 1f);
            }
            

            if(failState) {
                spriteBatch.Draw(GameHandler.plainWhiteTexture, new Rectangle(160,155,520,120), Color.Black);
                spriteBatch.DrawString(GameHandler.highPixel22, "Wrong drying order!\nPress [SPACEBAR] to restart;\n[BKSPACE] to see instructions", new Vector2(175,170), Color.Red);
            }
        }

        //draw towel
        towelBounds = new Rectangle(towelPos, new Point(128,128));
        spriteBatch.Draw(atlas, towelBounds, towel, Color.White, 0f, towelOrigin, SpriteEffects.None, 1f);
        if(dryGoal.GetCompletion() && currentStage != GameStage.BathDrying) {
            spriteBatch.Draw(GameHandler.coreTextureAtlas, towelBounds, markX, Color.SlateGray);
        }

        //bounding box of spray bottle
        sprayBottleBounds = new Rectangle(sprayBottlePos, new Point(96, 96));

        //draw spray bottle
        spriteBatch.Draw(atlas, sprayBottleBounds, sprayBottle, Color.White, 0f, sprayBottleOrigin, SpriteEffects.None, 1f);
        //if spray goal is completed, draw X over spray bottle to indicate disabled
        //also checks that gamestage isn't still bath to prevent X from being drawn on section transition
        if(sprayGoal.GetCompletion() && currentStage != GameStage.BathWashing) {
            spriteBatch.Draw(GameHandler.coreTextureAtlas, sprayBottleBounds, markX, Color.SlateGray);
        }

        //render water particles
        for(int i = 0; i < particles.Count; i++) {
            particles[i].Draw(gameTime, spriteBatch, _graphics, new Color(0,127,255));
        }

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

            //changes text on info page according to button selected
            switch (currentInfo) {
                case InfoPanel.SummaryPanel:
                    spriteBatch.DrawString(GameHandler.highPixel36, "Summary", new Vector2(260,50),Color.Black);
                    spriteBatch.DrawString(
                        GameHandler.highPixel22,
                        """
                        Owning a pet requires some work! Your pet
                        needs your help to stay clean and happy.
                        Your pet needs you to brush their fur,
                        trim their nails, and give them a gentle
                        bath. But watch out! The gauge at the top
                        represents happiness. If it strays into the
                        red, your pet will get very upset and you
                        will need to start over! Good Luck!

                        Click any of the buttons below to see
                        instructions for each stage.
                        """,
                        new Vector2(50, 110),
                        Color.Black
                    );
                    break;

                case InfoPanel.BathInfoPanel:
                    spriteBatch.DrawString(GameHandler.highPixel36, "Bath", new Vector2(300,50),Color.Black);
                    spriteBatch.DrawString(
                        GameHandler.highPixel18,
                        """
                        This stage is separated into two parts:
                        washing and drying.

                        Part 1:
                        Your cat will enter the green box at 1 of 3 heights.
                        You must spray it 5 times while inside the box,
                        but keep an eye on your water supply!

                        Part 2:
                        Pay close attention to the order that the water
                        drops fall, then dry the wet spots in that same
                        order! The traffic light indicates red means timeout,
                        yelow means watch the pattern, and green means your
                        turn.
                        """,
                        new Vector2(50, 110),
                        Color.Black
                    );
                    break;

                case InfoPanel.NailsInfoPanel:
                    spriteBatch.DrawString(GameHandler.highPixel36, "Nail Trimming", new Vector2(240,50),Color.Black);
                    spriteBatch.DrawString(
                        GameHandler.highPixel22,
                        """
                        Position the nail clippers over your
                        cat's paws. Notice the bar at the top
                        with the rapidly-moving marker. When
                        the marker is inside the green zone,
                        click the mouse. But watch out!
                        Clicking outside of the green zone
                        irritates the cat, and when the
                        mood meter dips into red, you must
                        restart!

                        Trim 10 nails to win!
                        """,
                        new Vector2(50, 110),
                        Color.Black
                    );
                    break;

                case InfoPanel.BrushInfoPanel:
                    spriteBatch.DrawString(GameHandler.highPixel36, "Brushing", new Vector2(260,50),Color.Black);
                    spriteBatch.DrawString(
                        GameHandler.highPixel22,
                        """
                        Click and hold the mouse, moving the brush
                        head over each of the hotspots. When in
                        contact with the brush, they will increment
                        and change colour. Get all hotspots to
                        green to progress the counter. But
                        watch out! Brushing hotspots into yellow
                        can reverse progress, and brushing
                        hotspots to red resets the stage!

                        Move the counter to the end to win!
                        """,
                        new Vector2(50, 110),
                        Color.Black
                    );
                    break;
            }
            
            //start game button
            spriteBatch.Draw(GameHandler.coreTextureAtlas, startButtonBounds, new Rectangle(16,0,16,16), Color.White);
            spriteBatch.DrawString(GameHandler.highPixel22, "Start", new Vector2(610, startButtonPos.Y+25), Color.Black);

            //minigame stage info buttons - colours button green if selected
            if(currentInfo == InfoPanel.BathInfoPanel) {
                spriteBatch.Draw(GameHandler.coreTextureAtlas, infoBathBounds, new Rectangle(16,0,16,16), Color.Lime);
            } else {
                spriteBatch.Draw(GameHandler.coreTextureAtlas, infoBathBounds, new Rectangle(16,0,16,16), Color.White);
            }
            spriteBatch.DrawString(GameHandler.highPixel22, "Bath", new Vector2(65, infoBathPos.Y+25), Color.Black);
            
            if(currentInfo == InfoPanel.BrushInfoPanel) {
                spriteBatch.Draw(GameHandler.coreTextureAtlas, infoBrushBounds, new Rectangle(16,0,16,16), Color.Lime);
            } else {
                spriteBatch.Draw(GameHandler.coreTextureAtlas, infoBrushBounds, new Rectangle(16,0,16,16), Color.White);
            }
            spriteBatch.DrawString(GameHandler.highPixel22, "Brushing", new Vector2(215, infoBrushPos.Y+25), Color.Black);

            if(currentInfo == InfoPanel.NailsInfoPanel) {
                spriteBatch.Draw(GameHandler.coreTextureAtlas, infoNailsBounds, new Rectangle(16,0,16,16), Color.Lime);
            } else {
                spriteBatch.Draw(GameHandler.coreTextureAtlas, infoNailsBounds, new Rectangle(16,0,16,16), Color.White);
            }
            spriteBatch.DrawString(GameHandler.highPixel22, "Nails", new Vector2(415, infoNailsPos.Y+25), Color.Black);

        } else if(currentStage == GameStage.NailTrim) {
            //calls draw for input gauge - will only draw if isVisible == true
            gameInputGauge.Draw(gameTime, spriteBatch);
            nailGoal.DrawOutput(spriteBatch, GameHandler.highPixel22, new Vector2(355, 120), Color.Black, "Nails");

            if(failState) {
                spriteBatch.Draw(GameHandler.plainWhiteTexture, new Rectangle(160,155,520,120), Color.Black);
                spriteBatch.DrawString(GameHandler.highPixel22, "The cat is angry!\nPress [SPACEBAR] to restart;\n[BKSPACE] to see instructions", new Vector2(175,170), Color.Red);
            }

        } else if(currentStage == GameStage.Brushing) {
            /***
            //debug for brush zones
            spriteBatch.Draw(GameHandler.plainWhiteTexture, hotspotBounds1, Color.Red);
            spriteBatch.Draw(GameHandler.plainWhiteTexture, hotspotBounds2, Color.Green);
            spriteBatch.Draw(GameHandler.plainWhiteTexture, hotspotBounds3, Color.Blue);
            ***/

            progressGauge.Draw(gameTime, spriteBatch);

            spriteBatch.Draw(atlas, brushBounds, brushHeld, Color.White, 0f, brushOrigin, SpriteEffects.None, 1f);

            //use to draw the points for brush contact
            hotspot1.DrawFrame(spriteBatch, hotspot1Frame, new Vector2(410,295), SpriteEffects.None);
            hotspot2.DrawFrame(spriteBatch, hotspot2Frame, new Vector2(360,450), SpriteEffects.None);
            hotspot3.DrawFrame(spriteBatch, hotspot3Frame, new Vector2(430,410), SpriteEffects.None);

            if(failState) {
                spriteBatch.Draw(GameHandler.plainWhiteTexture, new Rectangle(160,155,520,120), Color.Black);
                spriteBatch.DrawString(GameHandler.highPixel22, "Brushed too hard!\nPress [SPACEBAR] to restart;\n[BKSPACE] to see instructions", new Vector2(175,170), Color.Red);
            }

            //debug for brush head
            spriteBatch.Draw(GameHandler.plainWhiteTexture, new Rectangle(brushHeadOffset, new Point(8,8)), Color.Lime);

        } else if(currentStage == GameStage.BathWashing) {
            progressGauge.Draw(gameTime, spriteBatch);
            //print remaining water count
            spriteBatch.DrawString(GameHandler.highPixel22, "Water: " + water, new Vector2(620,115), Color.Black);
            //print progress count
            spriteBatch.DrawString(GameHandler.highPixel22, "Sprays: " + sprayGoal.GetCurrentValue() + "/5", new Vector2(400,115), Color.Black);

            if(failState) {
                spriteBatch.Draw(GameHandler.plainWhiteTexture, new Rectangle(160,155,520,120), Color.Black);
                spriteBatch.DrawString(GameHandler.highPixel22, "Ran out of water!\nPress [SPACEBAR] to restart;\n[BKSPACE] to see instructions", new Vector2(175,170), Color.Red);
            }
            //debug for cat bounds
            /*
            //spriteBatch.Draw(GameHandler.plainWhiteTexture, jumpBounds, Color.Green);
            spriteBatch.Draw(GameHandler.plainWhiteTexture, catBounds, Color.Red);
            spriteBatch.Draw(GameHandler.plainWhiteTexture, jumpGate, Color.Blue);
            */
        } else if(currentStage == GameStage.BathDrying) {
            //draw progress gauge
            progressGauge.Draw(gameTime, spriteBatch);
            if(!isReady) {
                spriteBatch.Draw(GameHandler.plainWhiteTexture, new Rectangle(160,155,520,50), Color.DeepPink);
                spriteBatch.DrawString(GameHandler.highPixel22, "Press [SPACEBAR] to continue!", new Vector2(175,170), Color.White);
            }
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
            //section 1 and section 2 both complete
            if(sprayGoal.GetCompletion() && dryGoal.GetCompletion()) {//put bath goal in here
                spriteBatch.Draw(GameHandler.coreTextureAtlas, new Rectangle(520, 525, 64, 64), checkmark, Color.LimeGreen);
            
            //only section 1 has been completed
            } else if(sprayGoal.GetCompletion()) {
                spriteBatch.Draw(GameHandler.coreTextureAtlas, new Rectangle(520, 525, 64, 64), checkmark, Color.Yellow);
            }
        }

        //draw win screen over everything
        if(currentStage == GameStage.Win) {
            spriteBatch.Draw(GameHandler.plainWhiteTexture, new Rectangle(0,0,800,600), Color.LightPink);
            spriteBatch.DrawString(GameHandler.highPixel36, "You Win!", new Vector2(240,50),Color.Black);
            spriteBatch.DrawString(
                        GameHandler.highPixel22,
                        """
                        You successfully cared for and groomed
                        your cat, and now it is happy! Return
                        to the overworld for your prize.
                        """,
                        new Vector2(50, 110),
                        Color.Black
            );
            spriteBatch.Draw(GameHandler.coreTextureAtlas, returnButtonBounds, new Rectangle(16,0,16,16), Color.White);
            spriteBatch.DrawString(GameHandler.highPixel22, "Return", new Vector2(310, returnButtonPos.Y+25), Color.Black);
        }

        //debug for cat pos coords
        //spriteBatch.DrawString(GameHandler.highPixel22, catPos.ToString(), new Vector2(0,60), Color.Black);
    }

    public void HandleInput(GameTime gameTime)
    {
        if(!failState) {
            //closes clipper when used  
            if(currentObject == ObjectHeld.NailClippers && GameHandler.mouseState.LeftButton == ButtonState.Pressed) {
                clippersUse = true;
            } else {
                clippersUse = false;
            }
        } else { //opens clippers if failstate triggers while mouse down
            clippersUse = false;
        }
        

        if(failState) {
            if(Keyboard.GetState().IsKeyDown(Keys.Space)) {
                if(currentStage == GameStage.BathWashing) {
                    CleanupProcesses();
                    currentStage = GameStage.BathWashing;
                    currentObject = ObjectHeld.SprayBottle;
                } else if(currentStage == GameStage.NailTrim) {
                    nailGoal.ResetGoal();
                    CleanupProcesses();
                    currentStage = GameStage.NailTrim;
                    currentObject = ObjectHeld.NailClippers;
                } else if(currentStage == GameStage.Brushing) {
                    brushGoal = false;
                    progressGauge.SetCurrentValue(0);
                    CleanupProcesses();
                    currentStage = GameStage.Brushing;
                    currentObject = ObjectHeld.Brush;
                } else if(currentStage == GameStage.BathDrying) {
                    dryGoal.ResetGoal();
                    spotOrder = ShuffleList(spotOrder);
                    simonIteration = 0;
                    spotIndex = 0;
                    waterDropPos = spotOrder[0].Center.ToVector2();
                    waterDropScale = 0f;
                    isPlayerTurn = false;
                    isReady = true;
                    failState = false;
                }
            } else if(Keyboard.GetState().IsKeyDown(Keys.Back)) {
                CleanupProcesses();
                progressGauge.SetCurrentValue(0);
                currentStage = GameStage.Instructions;
                currentObject = ObjectHeld.None;
                currentInfo = InfoPanel.SummaryPanel;
                if(GameHandler.allowAudio) {
                    catPurr.Pause();
                }
            }
        }

        //begins "simon says"
        if(currentStage == GameStage.BathDrying) {
            if(!isReady && Keyboard.GetState().IsKeyDown(Keys.Space)) {
                isReady = true;
            }
        }

        double cooldownBuffer = 0.25;

        //handle input for different stages in here, switch between them below
        if(GameHandler.mouseState.LeftButton == ButtonState.Pressed) {
            //runs repeatedly while mouse is pressed so you don't have to click
            //over and over

            //Console.WriteLine(hsCooldown1 + 0.5 < gameTime.TotalGameTime.TotalSeconds);
            if(currentStage == GameStage.Brushing && !failState) {
                if(hotspotBounds1.Contains(brushHeadOffset) && (hsCooldown1 + cooldownBuffer < gameTime.TotalGameTime.TotalSeconds)) {
                    if(hotspot1Frame < 8) {
                        hotspot1Frame++;
                    }
                    hsCooldown1 = gameTime.TotalGameTime.TotalSeconds;
                } else if(hotspotBounds2.Contains(brushHeadOffset) && (hsCooldown2 + cooldownBuffer < gameTime.TotalGameTime.TotalSeconds)) {
                    if(hotspot2Frame < 8) {
                        hotspot2Frame++;
                    }
                    hsCooldown2 = gameTime.TotalGameTime.TotalSeconds;
                } else if(hotspotBounds3.Contains(brushHeadOffset) && (hsCooldown3 + cooldownBuffer < gameTime.TotalGameTime.TotalSeconds)) {
                    if(hotspot3Frame < 8) {
                        hotspot3Frame++;
                    }
                    hsCooldown3 = gameTime.TotalGameTime.TotalSeconds;
                }

                if(GameHandler.allowAudio && !GameHandler.muted) {
                    if(hotspotBounds1.Contains(brushHeadOffset) || hotspotBounds2.Contains(brushHeadOffset) || hotspotBounds3.Contains(brushHeadOffset)) {
                        brushSfx.IsLooped = true;
                        brushSfx.Play();
                    }
                }
            }
            
            //runs only once after mouse pressed
            if(!mouseDown) {
                mouseDown = true;

                if(currentStage == GameStage.Win) {
                    if(returnButton.CheckIfSelectButtonWasClicked()) {
                        if(GameHandler.allowAudio) {
                            catPurr.Stop();
                        }

                        //sets save file bool for this game to be true
                        GameHandler.saveFile.PetCareDone = true;
                        //unloads assets this game is using
                        GameHandler.UnloadCurrentLevel();
                        GameHandler.LoadOverworld();
                    }
                }

                //if instructions are being displayed
                if(currentStage == GameStage.Instructions) {
                    //if start button is clicked
                    if(startButton.CheckIfSelectButtonWasClicked()) {
                        currentStage = GameStage.Idle;
                    } else if(infoBath.CheckIfSelectButtonWasClicked()) {
                        //toggles back to main summary if button is clicked again
                        if(currentInfo == InfoPanel.BathInfoPanel) {
                            currentInfo = InfoPanel.SummaryPanel;
                        } else {
                            currentInfo = InfoPanel.BathInfoPanel;
                        }
                    } else if(infoBrush.CheckIfSelectButtonWasClicked()) {
                        //toggles back to main summary if button is clicked again
                        if(currentInfo == InfoPanel.BrushInfoPanel) {
                            currentInfo = InfoPanel.SummaryPanel;
                        } else {
                            currentInfo = InfoPanel.BrushInfoPanel;
                        }
                    } else if(infoNails.CheckIfSelectButtonWasClicked()) {
                        //toggles back to main summary if button is clicked again
                        if(currentInfo == InfoPanel.NailsInfoPanel) {
                            currentInfo = InfoPanel.SummaryPanel;
                        } else {
                            currentInfo = InfoPanel.NailsInfoPanel;
                        }
                    }
                //if game stage is nail trimming and not in fail limbo state
                } else if(currentStage == GameStage.NailTrim && !failState) {

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
                                if(GameHandler.allowAudio && !GameHandler.muted) {
                                    if(GetAllCompletion()) {
                                        GameHandler.bigWin.Play();
                                    } else {
                                        smallWin.Play();
                                    }
                                }
                            }
                        } else if(returnState == -1) {
                            //returns false when unable to decrease anymore
                            if(!tempermentGauge.Decrement(2)) {
                                //failState = true;
                            }
                        }
                    }
                //handle input for bath level only if not in fail limbo
                } else if(currentStage == GameStage.BathWashing && !failState) {
                    if((sprayCooldown + 0.5) < gameTime.TotalGameTime.TotalSeconds) {
                        sprayCooldown = gameTime.TotalGameTime.TotalSeconds;
                        int y = (int)GameHandler.relativeMousePos.Y;
                        //keeps particle within appropriate vertical range
                        if(y > 450) {
                            y = 450;
                        } else if(y < 210) {
                            y = 210;
                        }
                        //spray goal hasn't been completed, prevents player from spamming
                        //bottle after completion and causing false failure
                        if(!sprayGoal.GetCompletion() && water > 0) {
                            particles.Add(new Particle(
                                new Point(100, y-60),
                                new Point(16, 8),
                                20,
                                GameHandler.plainWhiteTexture)
                            );
                            water--;
                        }
                        

                        if(GameHandler.allowAudio && !GameHandler.muted && !sprayGoal.GetCompletion()) {
                            spraySfx.Play();
                        }
                    }
                } else if(currentStage == GameStage.BathDrying && !failState && !dryGoal.GetCompletion()) {
                    if(isPlayerTurn) {
                        //current attempt index is within range or last index before next cycle
                        if(spotIndex <= simonIteration) {
                            //correct spot clicked
                            if(spotOrder[spotIndex].Contains(GameHandler.relativeMousePos)) {
                                spotIndex++; //increase index
                                Console.WriteLine("Correct spot");
                                if(GameHandler.allowAudio && !GameHandler.muted) {
                                    GameHandler.successSfx.Play();
                                }
                            } else { //check if wrong click or clicking somewhere else
                                
                                //looks through the list of other spots to see if click was
                                //misclick or genuinely wrong click
                                for(int i = spotIndex+1; i < spotOrder.Count; i++) {
                                    //confirms genuine wrong input
                                    if(spotOrder[i].Contains(GameHandler.relativeMousePos)) {
                                        failState = true;
                                        if(GameHandler.allowAudio && !GameHandler.muted) {
                                            GameHandler.failSfx.Play();
                                        }
                                        Console.WriteLine("fail");
                                    }
                                }
                                
                            }
                        }
                        //index is outside of current iteration, switch back to npc playback
                        if(spotIndex > simonIteration) {
                            simonCooldown = gameTime.TotalGameTime.TotalSeconds;
                            spotIndex = 0;
                            dryGoal.Increment();
                            simonIteration++;
                            isPlayerTurn = false;
                        }
                    }
                }


                /***********
                    End of game input handling, begin input handling for stage switching
                **********/

                //Activates gamestate based on object pressed
                if(currentObject == ObjectHeld.None && currentStage == GameStage.Idle) {
                    //switch to bath stage
                    if(sprayBottleBounds.Contains(GameHandler.relativeMousePos)) {
                        currentObject = ObjectHeld.SprayBottle;
                        currentStage = GameStage.BathWashing;
                        progressGauge.UpdateParameters(0,5,0);

                    //switch to nail clipping stage
                    } else if(!nailGoal.GetCompletion() && clipperBounds.Contains(GameHandler.relativeMousePos)) {
                        //sets mood to centre of gauge
                        tempermentGauge.SetCurrentValue(8);
                        //sets held object to nail clippers
                        currentObject = ObjectHeld.NailClippers;
                        //sets game stage to nail trimming
                        currentStage = GameStage.NailTrim;
                        //shows input gauge
                        gameInputGauge.SetVisibility(true);
                    //switch to brushing stage
                    } else if(!brushGoal && brushBounds.Contains(GameHandler.relativeMousePos)) {
                        //sets mood to centre of gauge
                        tempermentGauge.SetCurrentValue(8);
                        currentObject = ObjectHeld.Brush;
                        currentStage = GameStage.Brushing;
                        progressGauge.UpdateParameters(0,10,0);
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
        atlas = _manager.Load<Texture2D>("Sprites/petcare_textureatlas");
        startButton = new Button(GameHandler.coreTextureAtlas, GameHandler.coreTextureAtlas, new Point(225, 72), startButtonPos.ToVector2(), "Start", 42, true);
        infoBath = new Button(GameHandler.coreTextureAtlas, GameHandler.coreTextureAtlas, new Point(150, 72), infoBathPos.ToVector2(), "Bath", 43, true);
        infoBrush = new Button(GameHandler.coreTextureAtlas, GameHandler.coreTextureAtlas, new Point(190, 72), infoBrushPos.ToVector2(), "Brushing", 44, true);
        infoNails = new Button(GameHandler.coreTextureAtlas, GameHandler.coreTextureAtlas, new Point(150, 72), infoNailsPos.ToVector2(), "Nails", 45, true);
        returnButton = new Button(GameHandler.coreTextureAtlas, GameHandler.coreTextureAtlas, new Point(225, 72), returnButtonPos.ToVector2(), "Return", 46, true);

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
            waterDrip = _manager.Load<SoundEffect>("Sounds/water_drip");
            smallWin = _manager.Load<SoundEffect>("Sounds/small_win").CreateInstance();
            smallWin.Volume = 0.35f;
            pixelMusic18 = _manager.Load<SoundEffect>("Sounds/pixel_music_18").CreateInstance();
            pixelMusic13 = _manager.Load<SoundEffect>("Sounds/pixel_music_13").CreateInstance();
            
            pixelMusic18.IsLooped = false;
            pixelMusic13.IsLooped = false;
            pixelMusic13.Volume = 0.4f;
            pixelMusic18.Volume = 0.4f;

            currentSong = pixelMusic13;
            currentSong.Volume = 0.4f;

            currentSong.IsLooped = false;
            brushSfx.IsLooped = true;
        }        
    }

    public void LoadLevel()
    {
        //create gauges
        tempermentGauge = new ProgressGauge(new Rectangle(25, 20, 300, 60), 0, 16, 8, ProgressGauge.GaugeType.GoodBad, true);
        gameInputGauge = new ProgressGauge(new Rectangle(350, 20, 300, 60), 0, 30, 15, ProgressGauge.GaugeType.HitInRange, false);
        progressGauge = new ProgressGauge(new Rectangle(350, 20, 300, 60), 0, 10, 0, ProgressGauge.GaugeType.Progress, false);
        
        startButtonBounds = new Rectangle(startButtonPos.X, startButtonPos.Y, 225, 72);
        infoBathBounds = new Rectangle(infoBathPos.X, infoBathPos.Y, 150, 72);
        infoNailsBounds = new Rectangle(infoNailsPos.X, infoNailsPos.Y, 150, 72);
        infoBrushBounds = new Rectangle(infoBrushPos.X, infoBrushPos.Y, 190, 72);
        returnButtonBounds = new Rectangle(returnButtonPos.X, returnButtonPos.Y, 225, 72);

        //clears list if returning to minigame in current session
        spotOrder.Clear();
        //populates list
        spotOrder.Add(dryspotBounds1);
        spotOrder.Add(dryspotBounds2);
        spotOrder.Add(dryspotBounds3);
        spotOrder.Add(dryspotBounds4);

        //adds n number of random spots to fill any remaining slots after the initial 4
        //have been added, i.e.: maxSimonInterations 6 means one instance of each spot
        //plus two random spots (4 + 2)
        //maxSimonIterations will be minimum of 4
        for(int i = 0; i < maxSimonIterations - 4; i++) {
            spotOrder.Add(spotOrder[(int)rand.NextInt64(0, 4)]);
        }

        dryGoal = new Goal(maxSimonIterations);
        

        //randomizes order of list, ensuring different order every time
        spotOrder = ShuffleList(spotOrder);

        if(SaveFile.doesFileExist()) {
            LoadData();
        }

        DebugSkip(true);

        //puts waterdrop at starting spot
        waterDropPos = spotOrder[0].Center.ToVector2();


        //for simon says debug purposes:
        //sprayGoal.SetCompletion(true);
        

        //add statement to not force instructions if played before
    }

    public void Update(GameTime gameTime)
    {
        if(GameHandler.allowAudio) {
            //changes between two different songs
            if(currentSong.State == SoundState.Stopped) {
                if(currentSong == pixelMusic13) {
                    currentSong = pixelMusic18;
                    currentSong.IsLooped = false;
                } else {
                    currentSong = pixelMusic13;
                    currentSong.IsLooped = false;
                }
                currentSong.Play();
            }

            //turns down volume of music to better hear success sound
            if(smallWin.State == SoundState.Playing) {
                currentSong.Volume = 0.05f;
            } else {
                currentSong.Volume = 0.4f;
            }
        }
        //game is paused
        if(GameHandler.isPaused) {
            if(GameHandler.allowAudio) {
                catPurr.Pause();
                currentSong.Pause();
            }

        //game is running
        } else {
            if(GameHandler.allowAudio && !GameHandler.musicMuted) {
                currentSong.Play();
            }
            
            tempermentGauge.Update(gameTime);

            if(currentStage != GameStage.Instructions) {
                if(GameHandler.allowAudio && !GameHandler.muted) {
                    catPurr.Play();
                } else {
                    catPurr.Pause();
                }
            }
            
            //no game has been started
            if(currentStage == GameStage.Idle) { //no stage running
                if(GetAllCompletion()) { //transitions to win stage
                    currentStage = GameStage.Win;
                }
            } else if(currentStage == GameStage.NailTrim) { //nail trimming
                if(nailGoal.GetCompletion()) {
                    gameInputGauge.SetVisibility(false);
                }
                if(tempermentGauge.GetCurrentValue() == 0) {
                    failState = true;
                }
                if(!failState) {
                    gameInputGauge.Update(gameTime);
                }
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

                    //plays big win if all 3 stages are complete
                    if(GameHandler.allowAudio && !GameHandler.muted) {
                        if(GetAllCompletion()) {
                            GameHandler.bigWin.Play();
                        } else {
                            smallWin.Play();
                        }
                    }
                }
                if(!brushGoal && !failState) { //brushing goal not reached
                    progressGauge.SetVisibility(true);

                    
                    //delay duration
                    double cooldownBuffer = 1.5;

                    //handles decrementing hotspots
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

                    // HANDLES TEMPERMENT METER //

                    //pushing any hotspot to red fails
                    if(hotspot1Frame == 8 || hotspot2Frame == 8 || hotspot3Frame == 8) {
                        tempermentGauge.SetCurrentValue(0);
                        failState = true;

                    //pushing any hotspot to yellow causes drastic decrease
                    } else if (hotspot1Frame >= 6 || hotspot2Frame >= 6 || hotspot3Frame >= 6) {
                        tempermentGauge.SetCurrentValue(4);

                    //all hotspots are green, increment
                    } else if(
                        hotspot1Frame >= 3 && hotspot1Frame < 6 &&
                        hotspot2Frame >= 3 && hotspot2Frame < 6 &&
                        hotspot3Frame >= 3 && hotspot3Frame < 6
                    ) {
                        tempermentGauge.SetCurrentValue(14);

                    //one or more hotspots are gray
                    } else {
                        tempermentGauge.SetCurrentValue(8);
                    }

                    //progress cooldown handler
                    if(progressCooldown + 0.75 < gameTime.TotalGameTime.TotalSeconds) {
                        if(tempermentGauge.GetCurrentValue() == 4) {
                            progressGauge.Decrement();
                        } else if (tempermentGauge.GetCurrentValue() == 0) {
                            progressGauge.Decrement(4);
                        } else if(tempermentGauge.GetCurrentValue() == 14) {
                            if(!progressGauge.Increment()) { //win
                                
                            }
                        }
                        progressCooldown = gameTime.TotalGameTime.TotalSeconds;
                    }
                    progressGauge.Update(gameTime);
                }
            } else if(currentStage == GameStage.BathWashing) {
                progressGauge.SetVisibility(true);

                //makes progress gauge reflect goal value
                progressGauge.SetCurrentValue(sprayGoal.GetCurrentValue());

                if(sprayGoal.GetCompletion()) {
                    sprayGoal.SetCompletion(true);
                }

                progressGauge.Update(gameTime);

                //stops updates when in fail limbo
                if(!failState) {
                    //makes cat run slower in jump zone
                    if(catPos.X >= 405 && !waiting) {
                        catPos.X += 6;
                    
                    //makes cat run faster before jump zone
                    } else if(!waiting) {
                        catPos.X += 10;

                        //executes after last spray, final lap completed, and transitions to part 2 of minigame
                        if(finalLap && catPos.X >= 400) {
                            allowJump = false;
                            catPos = new Vector2(400, 305);
                            currentObject = ObjectHeld.Towel;
                            sprayBottlePos = new Point(64, 385);
                            sprayBottleOrigin = Vector2.Zero;

                            Console.WriteLine("BATHWASHING complete, progressing to BATHDRYING");
                            currentStage = GameStage.BathDrying;
                            progressGauge.UpdateParameters(0, maxSimonIterations, 0);
                        }

                    //cooldown expires
                    } else if(waiting && (jumpCooldown + jumpCooldownDuration) < gameTime.TotalGameTime.TotalSeconds) {
                        waiting = false;
                    }
                    //sets jump state based on jump index
                    if(jumpGate.Contains(catPos)) {
                        
                        //spray portion completed, prepares to transition to 2nd portion
                        if(sprayGoal.GetCompletion()) {
                            finalLap = true;

                        //jumpIndex 0 means no jump
                        }  else if(jumpIndex != 0) {
                            allowJump = true;
                        }
                    }

                    //move back to left side of screen
                    if(catPos.X >= 950) {
                        //moves cat back to left side and on floor
                        catPos.X = -150;
                        catPos.Y = 305;
                        //resets animation frame
                        jumpFrame = 2;
                        //is currently not jumping
                        isJumping = false;
                        //is waiting until next run cycle
                        waiting = true;

                        //prevents from receiving the same value multiple times in a row
                        do {
                            jumpIndex = (int)rand.NextInt64(0,3);
                        } while(jumpIndex == prevJumpIndex);
                        
                        //Console.WriteLine(jumpIndex); //prints out the index used for next jump
                        //updates previous index with most recent index
                        prevJumpIndex = jumpIndex;

                        jumpCooldown = gameTime.TotalGameTime.TotalSeconds;
                        jumpCooldownDuration = (int)rand.NextInt64(1,6);
                    }
                    //allows cat to jump when reaching specified jump position
                    if(catPos.X >= 405 && allowJump) {
                        isJumping = true;
                        catPos.Y = 305;
                    }

                    if(isJumping) {
                        //handles the two different jump heights
                        //case 0 is not included because cat does not jump at index 0
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

                    //calculates collision box for cat using offsets
                    catBounds = new Rectangle((int)(catPos.X-50), (int)(catPos.Y-20), 100, 200);
                }

                
                
                int index = 0;
                //cycles through list of existing particles, destroying them if they meet
                //destruction criteria and incrementing goal if they meet success criteria
                while (index < particles.Count) {
                    particles[index].Update(gameTime);
                    if(particles[index].CheckToDestroy(catBounds) == 1 ||
                    particles[index].CheckToDestroy(catBounds) == 2) {
                        //increments goal if stage is Bath, particle hits cat
                        //and cat is within spray zone
                        if(particles[index].CheckToDestroy(catBounds) == 2 &&
                        currentStage == GameStage.BathWashing &&
                        jumpBounds.Intersects(catBounds)) {
                            sprayGoal.Increment();
                            if(GameHandler.allowAudio && !GameHandler.muted) {
                                GameHandler.successSfx.Play();
                            }
                        }
                        //removes the particle at said index but doesn't increment index
                        //since the next particle in list now moves to current index
                        particles.RemoveAt(index);
                    } else {
                        //increments if no particle was removed
                        index++;
                    }
                }

                //update failstate last to prevent false failure should player use all water
                //and still win
                if(water <= 0 && particles.Count <= 0 && !sprayGoal.GetCompletion()) {
                    failState = true;
                }


            } else if(currentStage == GameStage.BathDrying) {
                progressGauge.SetVisibility(true);
                progressGauge.SetCurrentValue(dryGoal.GetCurrentValue());
                progressGauge.Update(gameTime);

                if(dryGoal.GetCompletion()) {
                    currentStage = GameStage.Idle;
                    currentObject = ObjectHeld.None;                        
                    towelPos = new Point(55,185);
                    towelOrigin = Vector2.Zero;
                    Console.WriteLine("BATHDRYING stage won, returning to IDLE");
                    if(GameHandler.allowAudio && !GameHandler.muted) {
                        if(GetAllCompletion()) {
                            GameHandler.bigWin.Play();
                        } else {
                            smallWin.Play();
                        }
                    }
                }

                if(isReady && !failState && !dryGoal.GetCompletion()) { 
                    //is npc turn
                    if(!isPlayerTurn && (simonCooldown + simonDelay < gameTime.TotalGameTime.TotalSeconds)) {
                        if(waterDropScale < 1.5f) {
                            waterDropScale += 0.025f;
                        } else {
                            waterDropPos.Y += 3;
                            if(waterDropPos.Y > 525) {
                                if(spotIndex < simonIteration) {
                                    spotIndex++;
                                } else { //transitions to player turn
                                    isPlayerTurn = true;
                                    spotIndex = 0;
                                }

                                waterDropScale = 0f;
                                waterDropPos = spotOrder[spotIndex].Center.ToVector2();
                                if(GameHandler.allowAudio && !GameHandler.muted) {
                                    waterDrip.Play();
                                }
                            }
                        }
                    }
                }
            }

            //spray bottle held and not in fail limbo state
            if(currentObject == ObjectHeld.SprayBottle && !failState) {
                int y = (int)GameHandler.relativeMousePos.Y;

                //keeps bottle within appropriate vertical range
                if(y > 450) {
                    y = 450;
                } else if(y < 210) {
                    y = 210;
                }
                sprayBottlePos = new Point(64, y);

                sprayBottleOrigin = new Vector2(0,20);

            } else if(currentObject == ObjectHeld.NailClippers && !failState) {
                clippersPos = GameHandler.relativeMousePos.ToPoint();
                //snaps clippers to mouse and changes origin
                clippersOrigin = new Vector2(16, 8);
            } else if(currentObject == ObjectHeld.Towel) {
                //snaps towel to mouse and changes origin
                towelPos = GameHandler.relativeMousePos.ToPoint();
                towelOrigin = new Vector2(16, 8);
            } else if(currentObject == ObjectHeld.Brush) {
                brushPos = GameHandler.relativeMousePos.ToPoint();
                brushOrigin = new Vector2(28, 10);
                brushHeadOffset = new Point((int)GameHandler.relativeMousePos.X - 55, (int)GameHandler.relativeMousePos.Y + 20);
            }
        }
    }

    //calculates the Y position of the cat, given its X pos and desired peak height
    //used for relatively fluid jump movements
    private float GetJumpY(float posX, float destHeight) {
        float destX = 600f;
        int jumpStart = 400;
        int groundLevel = 305;
        float a = (groundLevel - destHeight) / MathF.Pow(jumpStart - destX, 2);
        float y = a * MathF.Pow(posX - destX, 2) + destHeight;

        //prevents cat from falling through floor
        if(y >= groundLevel) {
            return groundLevel;
        } 

        return y;
    }

    //resets all variables that stages use to allow player to come back and resume without
    //games breaking. will not reset goals once completed
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
        finalLap = false;
        failState = false;
        water = 16;

        isReady = false;
        spotIndex = 0;
        simonIteration = 0;
        isPlayerTurn = false;

        //resets the goals for the bath stage only if both sections weren't completed
        //will not preserve progress if only part 1 was completed
        if(!(sprayGoal.GetCompletion() && dryGoal.GetCompletion())) {
            sprayGoal.ResetGoal();
            dryGoal.ResetGoal();
        }

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
        currentInfo = InfoPanel.SummaryPanel;

        hotspot1Frame = 0;
        hotspot2Frame = 0;
        hotspot3Frame = 0;

        tempermentGauge.SetCurrentValue(8);
        progressGauge.SetCurrentValue(0);

        // nailGoal.ResetGoal();
        if(!nailGoal.GetCompletion()) {
            nailGoal.ResetGoal();
        }
    }

    public void SaveData(SaveFile saveFile)
    {
        GameHandler.saveFile.BathDone = sprayGoal.GetCompletion() && dryGoal.GetCompletion();
        GameHandler.saveFile.BrushingDone = brushGoal;
        GameHandler.saveFile.NailTrimDone = nailGoal.GetCompletion();
        GameHandler.saveFile.PetCareDone = GameHandler.saveFile.BathDone && GameHandler.saveFile.BrushingDone && GameHandler.saveFile.NailTrimDone;
    }

    public void LoadData()
    {
        brushGoal = GameHandler.saveFile.BrushingDone;
        nailGoal.SetCompletion(GameHandler.saveFile.NailTrimDone);
        sprayGoal.SetCompletion(GameHandler.saveFile.BathDone);
        dryGoal.SetCompletion(GameHandler.saveFile.BathDone);
    }

    private List<T> ShuffleList<T>(List<T> listToShuffle)
    {
        var shuffledList = listToShuffle.OrderBy(_ => rand.Next()).ToList();
        return shuffledList;
    }

    private bool GetAllCompletion() {
        return brushGoal && dryGoal.GetCompletion() && sprayGoal.GetCompletion() && nailGoal.GetCompletion();
    }

    private void DebugSkip(bool enabled) {
        if(enabled) {
            dryGoal.SetCompletion(true);
            sprayGoal.SetCompletion(true);
            nailGoal.SetCompletion(true);
            brushGoal = true;
        }
    }
}