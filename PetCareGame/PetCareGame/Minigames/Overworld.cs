using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace PetCareGame
{
    public class Overworld : LevelInterface
    {

        private static Button petCareButton;
        private static Button waldoButton;
        private static Button slidingButton;
        private static Button fishingButton;
        private static Button pauseButton;

        private Vector2 slidingButtonPosition;
        private Vector2 waldoButtonPosition;
        private Vector2 fishingButtonPosition;
        private Vector2 petCareButtonPosition;

        private KeyboardState kybdState;
        private Vector2 catPos = new Vector2(0, 192);
        private Rectangle catBounds;
        private AnimatedTexture catWalkRight = new AnimatedTexture(Vector2.Zero, 0f, 1f, 1f);
        private AnimatedTexture catWalkUp = new AnimatedTexture(Vector2.Zero, 0f, 1f, 1f);
        private AnimatedTexture catWalkDown = new AnimatedTexture(Vector2.Zero, 0f, 1f, 1f);
        private AnimatedTexture catIdle = new AnimatedTexture(Vector2.Zero, 0f, 1f, 1f);
        private List<Rectangle> colliders = new List<Rectangle>();

        private PetCare PetCareLevel { get; set; }
        private CatFishing FishingLevel { get; set; }
        private WheresWaldo WaldoLevel { get; set; }
        private SlidingGame SlidingLevel { get; set; }
        private PauseMenu PauseMenu { get; set; }
        private ContentManager coreAssets { get; set; }

        private Texture2D atlas;
        private Rectangle[,] overworldBlueprint;
        private int mult = 64; //tiles are 64x64

        private SoundEffectInstance forestAmbience;
        private SoundEffectInstance forestMusic;
        private SoundEffectInstance footsteps;

        private bool roadBlock1;
        private bool roadBlock2;

        private Rectangle missionPad1;
        private Rectangle missionPad2;
        private Rectangle missionPad3;

        private Rectangle T00 = new Rectangle(0,0,16,16); //grass
        private Rectangle T01 = new Rectangle(16,16,16,16); //vertical path
        private Rectangle T02 = new Rectangle(48,16,16,16); //T-intersection pointing up
        private Rectangle T03 = new Rectangle(32,0,16,16); //horizontal path
        private Rectangle T04 = new Rectangle(0,32,16,16); //path elbow from top to right
        private Rectangle T05 = new Rectangle(16,32,16,16); //path elbow from left to bottom
        private Rectangle T06 = new Rectangle(16,0,16,16); //flower field pink
        private Rectangle T07 = new Rectangle(48,0,16,16); //rough grass
        private Rectangle T08 = new Rectangle(32,16,16,16); //rough grass top
        private Rectangle T09 = new Rectangle(32,32,16,16); //tree edge right
        private Rectangle T10 = new Rectangle(48,32,16,16); //tree corner top right
        private Rectangle T11 = new Rectangle(0,48,16,16); //tree corner bottom right
        private Rectangle T12 = new Rectangle(16,48,16,16); //rough grass bottom
        private Rectangle T13 = new Rectangle(64,0,16,16); //path elbow from left to top
        private Rectangle T14 = new Rectangle(64,16,16,16); //water cliff edge left
        private Rectangle T15 = new Rectangle(64,32,16,16); //water cliff corner bottom left
        private Rectangle T16 = new Rectangle(64,48,16,16); //water cliff edge bottom
        private Rectangle T17 = new Rectangle(64,64,16,16); //water
        private Rectangle T18 = new Rectangle(64,80,16,16); //water cliff corner bottom right
        private Rectangle T19 = new Rectangle(64,96,16,16); //water cliff corner top right
        private Rectangle T20 = new Rectangle(64,112,16,16); //water cliff edge right
        private Rectangle T21 = new Rectangle(80,0,16,16); //water cliff inner corner top right
        private Rectangle T22 = new Rectangle(0,16,16,16); //path cap top
        private Rectangle T23 = new Rectangle(80,32,16,16); //flower field yellow


        public Overworld(PetCare pet, WheresWaldo waldo, SlidingGame sliding, PauseMenu pauseMenu, Button pauseB)
        {
            PetCareLevel = pet;
            WaldoLevel = waldo;
            SlidingLevel = sliding;
            PauseMenu = pauseMenu;
            coreAssets = GameHandler.coreAssets;
            pauseButton = pauseB;
            LoadLevel();
        }

        public void HandleInput(GameTime gameTime)
        {
            int speedH = 2;
            int speedV = 2;

            bool footstepsFlag = false;
            //updates cat position, ensuring requested movement doesn't go out of bounds
            if(!GameHandler.isPaused) {
                if(kybdState.GetPressedKeys().Contains(Keys.A) || kybdState.GetPressedKeys().Contains(Keys.Left)) {
                    if(ValidateMovement("H", speedH*-1)) {
                        catPos.X -= speedH;
                        footstepsFlag = true;
                    }
                } else if(kybdState.GetPressedKeys().Contains(Keys.D) || kybdState.GetPressedKeys().Contains(Keys.Right)) {
                    if(ValidateMovement("H", speedH)) {
                        catPos.X += speedH;
                        footstepsFlag = true;
                    }
                } else if(kybdState.GetPressedKeys().Contains(Keys.W) || kybdState.GetPressedKeys().Contains(Keys.Up)) {
                    if(ValidateMovement("V", speedV*-1)) {
                        catPos.Y -= speedV;
                        footstepsFlag = true;
                    }
                } else if(kybdState.GetPressedKeys().Contains(Keys.S) || kybdState.GetPressedKeys().Contains(Keys.Down)) {
                    if(ValidateMovement("V", speedV)) {
                        catPos.Y += speedV;
                        footstepsFlag = true;
                    }
                }

                //handle the mission pads here
                if(kybdState.IsKeyDown(Keys.E)) {
                    if(missionPad1.Contains(catBounds)) {
                        GameHandler.UnloadCurrentLevel();
                        GameHandler.CurrentState = GameHandler.GameState.SlidingGame;
                        SlidingLevel.LoadContent(GameHandler.slidingAssets, coreAssets);
                        SlidingLevel.LoadLevel();
                    } else if(missionPad2.Contains(catBounds)) {
                        GameHandler.UnloadCurrentLevel();
                        GameHandler.CurrentState = GameHandler.GameState.WaldoGame;
                        WaldoLevel.LoadContent(GameHandler.waldoAssets, coreAssets);
                        WaldoLevel.LoadLevel();
                    } else if(missionPad3.Contains(catBounds)) {
                        GameHandler.UnloadCurrentLevel();
                        GameHandler.CurrentState = GameHandler.GameState.PetCareGame;
                        PetCareLevel.LoadContent(GameHandler.petcareAssets, coreAssets);
                        PetCareLevel.LoadLevel();
                    }
                }



                if(GameHandler.allowAudio) {
                    if(!GameHandler.muted && footstepsFlag) {
                        footsteps.Play();
                    } else {
                        footsteps.Pause();
                    }
                }
            }
            //for now, loading the next minigame will be handeled via the four buttons
            //but in the future, there will be conditional logic to allow for whether a
            //minigame should be loaded, whether the button is enabled, etc
            if (GameHandler.mouseLeftPressed)
            {
                GameHandler.mouseLeftPressed = false;
                //checks if already paused to prevent spamming of button
                if (pauseButton.CheckIfSelectButtonWasClicked() && !GameHandler.isPaused)
                {
                    pauseButton.Clicked();
                    GameHandler.isPaused = true;
                    PauseMenu.LoadLevel();
                    PauseMenu.LoadContent(null, coreAssets);
                    //prevents checking of other buttons while in pause menu
                }
                else if (!GameHandler.isPaused)
                {
                    //this is the input handler for our debug buttons
                    /*if (petCareButton.CheckIfSelectButtonWasClicked())
                    {
                        GameHandler.UnloadCurrentLevel();
                        SetVisiblity(false); //hides buttons to prevent them from being pressed again
                        petCareButton.Clicked();
                        GameHandler.CurrentState = GameHandler.GameState.PetCareGame;
                        PetCareLevel.LoadContent(GameHandler.petcareAssets, coreAssets);
                        PetCareLevel.LoadLevel();
                    }
                    else if (waldoButton.CheckIfSelectButtonWasClicked())
                    {
                        GameHandler.UnloadCurrentLevel();
                        SetVisiblity(false);
                        waldoButton.Clicked();
                        GameHandler.CurrentState = GameHandler.GameState.WaldoGame;
                        WaldoLevel.LoadContent(GameHandler.waldoAssets, coreAssets);
                        WaldoLevel.LoadLevel();
                    }
                    else if (slidingButton.CheckIfSelectButtonWasClicked())
                    {
                        GameHandler.UnloadCurrentLevel();
                        SetVisiblity(false);
                        slidingButton.Clicked();
                        GameHandler.CurrentState = GameHandler.GameState.SlidingGame;
                        SlidingLevel.LoadContent(GameHandler.slidingAssets, coreAssets);
                        SlidingLevel.LoadLevel();
                    }
                    else if (fishingButton.CheckIfSelectButtonWasClicked())
                    {
                        GameHandler.UnloadCurrentLevel();
                        SetVisiblity(false);
                        fishingButton.Clicked();
                        GameHandler.CurrentState = GameHandler.GameState.FishingGame;
                        FishingLevel.LoadContent(GameHandler.fishingAssets, coreAssets);
                        FishingLevel.LoadLevel();
                    }*/
                }
            }
        }

        //hide or show visiblity of items in here
        public static void SetVisiblity(bool isVisible)
        {
            petCareButton.Visible = isVisible;
            fishingButton.Visible = isVisible;
            waldoButton.Visible = isVisible;
            slidingButton.Visible = isVisible;
        }

        public void Draw(GameTime gameTime, SpriteBatch spriteBatch, GraphicsDeviceManager _graphics)
        {
            Rectangle settlement = new Rectangle(0,64,64,64);
            Rectangle house = new Rectangle(80,16,16,16);
            Rectangle rock1 = new Rectangle(80,48,16,16);
            Rectangle rock2 = new Rectangle(80,64,16,16);
            Rectangle highlight = new Rectangle(32,48,16,16);

            Rectangle missionPadBlue = new Rectangle(80,80,16,16);
            Rectangle missionPadRed = new Rectangle(80,96,16,16);
            Rectangle missionPadGray = new Rectangle(80,112,16,16);
            Rectangle tileDebug = new Rectangle(48,48,16,16);

            Rectangle sourceRectangle = new Rectangle(0, 0, petCareButton.CellWidth, petCareButton.CellHeight);
            Rectangle sourceRectangle1 = new Rectangle(0, 0, waldoButton.CellWidth, waldoButton.CellHeight);
            Rectangle sourceRectangle2 = new Rectangle(0, 0, slidingButton.CellWidth, slidingButton.CellHeight);
            Rectangle sourceRectangle3 = new Rectangle(0, 0, fishingButton.CellWidth, fishingButton.CellHeight);

            Rectangle destinationRectangle = new Rectangle((int)petCareButtonPosition.X, (int)petCareButtonPosition.Y, petCareButton.CellWidth, petCareButton.CellHeight);
            Rectangle destinationRectangle1 = new Rectangle((int)waldoButtonPosition.X, (int)waldoButtonPosition.Y, waldoButton.CellWidth, waldoButton.CellHeight);
            Rectangle destinationRectangle2 = new Rectangle((int)slidingButtonPosition.X, (int)slidingButtonPosition.Y, slidingButton.CellWidth, slidingButton.CellHeight);
            Rectangle destinationRectangle3 = new Rectangle((int)fishingButtonPosition.X, (int)fishingButtonPosition.Y, fishingButton.CellWidth, fishingButton.CellHeight);

            for(int h = 0; h < 13; h++) {
                for(int v = 0; v < 10; v++) {
                    spriteBatch.Draw(atlas, new Rectangle(h*64, v*64, 64, 64), overworldBlueprint[v,h], Color.White);
                    

                    //draws a debug grid to make assembling this easier
                    //spriteBatch.Draw(atlas, new Rectangle(h*64, v*64, 64, 64), tileDebug, Color.White);
                }
            }

            

            spriteBatch.Draw(atlas, new Rectangle(7*mult,4*mult,4*mult,4*mult), settlement, Color.White);
            spriteBatch.Draw(atlas, new Rectangle(9*mult,mult,mult,mult), house, Color.White);

            //debug draw colliders
            /*for(int i = 0; i < colliders.Count; i++) {
                spriteBatch.Draw(GameHandler.plainWhiteTexture, colliders[i], Color.Orange);
            }*/

            //roadblock 1
            if(roadBlock1) {
                spriteBatch.Draw(atlas, new Rectangle(4*mult,8*mult,mult,mult), rock1, Color.White);
            }
            if(roadBlock2) {
                spriteBatch.Draw(atlas, new Rectangle(11*mult,6*mult,mult,mult), rock2, Color.White);
            }
            
            //debug draw mission pad bounds
            /*spriteBatch.Draw(GameHandler.plainWhiteTexture, missionPad1, Color.Turquoise);
            spriteBatch.Draw(GameHandler.plainWhiteTexture, missionPad2, Color.Turquoise);
            spriteBatch.Draw(GameHandler.plainWhiteTexture, missionPad3, Color.Turquoise);*/

            //draw default states for mission pads
            spriteBatch.Draw(atlas, missionPad1, missionPadRed, Color.White);
            spriteBatch.Draw(atlas, missionPad2, missionPadGray, Color.White);
            spriteBatch.Draw(atlas, missionPad3, missionPadGray, Color.White);

            //minigame 1 complete
            if(GameHandler.saveFile.SlidingGameDone) {
                spriteBatch.Draw(atlas, missionPad1, missionPadBlue, Color.White);
                spriteBatch.Draw(atlas, missionPad2, missionPadRed, Color.White);
            }
            //minigame 2 complete
            if(GameHandler.saveFile.WheresWaldoDone) {
                spriteBatch.Draw(atlas, missionPad2, missionPadBlue, Color.White);
                spriteBatch.Draw(atlas, missionPad3, missionPadRed, Color.White);
            }
            //minigame 3 complete
            if((
                GameHandler.saveFile.BathDone &&
                GameHandler.saveFile.BrushingDone &&
                GameHandler.saveFile.NailTrimDone) ||
                GameHandler.saveFile.PetCareDone
            ) {
                spriteBatch.Draw(atlas, missionPad3, missionPadBlue, Color.White);
            }

            

            //debug draw cat bounds
            //spriteBatch.Draw(GameHandler.plainWhiteTexture, catBounds, Color.Lime);

            //draws movement sprites
            if(kybdState.GetPressedKeys().Contains(Keys.A) || kybdState.GetPressedKeys().Contains(Keys.Left)) {
                catWalkRight.DrawFrame(spriteBatch, catPos, SpriteEffects.FlipHorizontally);
            } else if(kybdState.GetPressedKeys().Contains(Keys.D) || kybdState.GetPressedKeys().Contains(Keys.Right)) {
                catWalkRight.DrawFrame(spriteBatch, catPos, SpriteEffects.None);
            } else if(kybdState.GetPressedKeys().Contains(Keys.W) || kybdState.GetPressedKeys().Contains(Keys.Up)) {
                catWalkUp.DrawFrame(spriteBatch, catPos, SpriteEffects.None);
            } else if(kybdState.GetPressedKeys().Contains(Keys.S) || kybdState.GetPressedKeys().Contains(Keys.Down)) {
                catWalkDown.DrawFrame(spriteBatch, catPos, SpriteEffects.None);
            } else {
                catIdle.DrawFrame(spriteBatch, catPos, SpriteEffects.None);
            }

            //draw start hint
            if(missionPad1.Contains(catBounds) || missionPad2.Contains(catBounds) || missionPad3.Contains(catBounds)) {
                spriteBatch.Draw(GameHandler.coreTextureAtlas, new Rectangle(180,60,220,90), new Rectangle(16,32,16,16), Color.DeepSkyBlue);
                spriteBatch.DrawString(GameHandler.highPixel22, "Press [E]\nto start!", new Vector2(210,75), Color.Black);
                //awful-looking flashy highlight thing
                //spriteBatch.Draw(atlas, new Rectangle(100,25,510,150), highlight, Color.Gold);
            }
            
            
            //the numbered buttons we used
            /*
            spriteBatch.Draw(petCareButton.Texture, destinationRectangle, sourceRectangle, Color.White, 0.0f, Vector2.Zero, SpriteEffects.None, 1.0f);
            spriteBatch.Draw(waldoButton.Texture, destinationRectangle1, sourceRectangle1, Color.White, 0.0f, Vector2.Zero, SpriteEffects.None, 1.0f);
            spriteBatch.Draw(slidingButton.Texture, destinationRectangle2, sourceRectangle2, Color.White, 0.0f, Vector2.Zero, SpriteEffects.None, 1.0f);
            spriteBatch.Draw(fishingButton.Texture, destinationRectangle3, sourceRectangle3, Color.White, 0.0f, Vector2.Zero, SpriteEffects.None, 1.0f);
            */
        }

        public void Update(GameTime gameTime)
        {
            kybdState = Keyboard.GetState();
            float elapsed = (float)gameTime.ElapsedGameTime.TotalSeconds;
            catWalkRight.UpdateFrame(elapsed);
            catWalkUp.UpdateFrame(elapsed);
            catWalkDown.UpdateFrame(elapsed);
            catIdle.UpdateFrame(elapsed);

            catBounds = new Rectangle((int)catPos.X+10,(int)catPos.Y+10,40,40);
            
            if(GameHandler.allowAudio) {
                if(!GameHandler.isPaused && !GameHandler.muted) {
                    forestAmbience.Play();
                } else {
                    forestAmbience.Pause();
                }

                if(!GameHandler.isPaused && !GameHandler.musicMuted) {
                    forestMusic.Play();
                } else {
                    forestMusic.Pause();
                }
            }

            if(!GameHandler.isPaused) {
                GameHandler.saveFile.catPosX = (int)catPos.X;
                GameHandler.saveFile.catPosY = (int)catPos.Y;
            }
            
            
            HandleInput(gameTime);
        }

        public void LoadContent(ContentManager _manager, ContentManager assets)
        {
            atlas = _manager.Load<Texture2D>("Sprites/overworld_atlas");

            int fps = 8;
            catWalkRight.Load(_manager, "Sprites/walk_right", 8, fps);
            catWalkUp.Load(_manager, "Sprites/walk_up", 8, fps);
            catWalkDown.Load(_manager, "Sprites/walk_down", 8, fps);
            catIdle.Load(coreAssets, "Sprites/Animal/idle", 7, 5);

            if(GameHandler.allowAudio) {
                forestAmbience = _manager.Load<SoundEffect>("Sounds/forest_ambience").CreateInstance();
                forestAmbience.Volume = 0.3f;
                forestAmbience.IsLooped = true;

                forestMusic = _manager.Load<SoundEffect>("Sounds/forest_music").CreateInstance();
                forestMusic.Volume = 0.4f;
                forestMusic.IsLooped = true;

                footsteps = _manager.Load<SoundEffect>("Sounds/gravel_footsteps").CreateInstance();
                footsteps.Volume = 0.15f;
                footsteps.IsLooped = true;
                footsteps.Pitch = 0.5f;
            }
        }

        public void LoadLevel()
        {
            petCareButtonPosition = new Vector2(100, 100);
            waldoButtonPosition = new Vector2(164, 100);
            slidingButtonPosition = new Vector2(228, 100);
            fishingButtonPosition = new Vector2(292, 100);

            //roadblocks are true if goal not met
            roadBlock1 = !GameHandler.saveFile.SlidingGameDone;
            roadBlock2 = !GameHandler.saveFile.WheresWaldoDone;

            missionPad1 = new Rectangle(3*mult, 6*mult, mult, mult);
            missionPad2 = new Rectangle(9*mult, 7*mult, mult, mult);
            missionPad3 = new Rectangle(10*mult, mult, mult, mult);

            petCareButton = new Button(coreAssets.Load<Texture2D>("Sprites/Buttons/PetCareMiniGame"), coreAssets.Load<Texture2D>("Sprites/Buttons/PetCareMiniGameClicked"),
                                                new Point(64, 33), petCareButtonPosition, "Pet Care Minigame", 33, true);
            waldoButton = new Button(coreAssets.Load<Texture2D>("Sprites/Buttons/WaldoMiniGame"), coreAssets.Load<Texture2D>("Sprites/Buttons/WaldoMiniGameClicked"),
                                                new Point(64, 33), waldoButtonPosition, "Where's Waldo Minigame", 34, true);
            slidingButton = new Button(coreAssets.Load<Texture2D>("Sprites/Buttons/SlideMiniGame"), coreAssets.Load<Texture2D>("Sprites/Buttons/SlideMiniGameClicked"),
                                                new Point(64, 33), slidingButtonPosition, "Sliding Minigame", 35, true);
            fishingButton = new Button(coreAssets.Load<Texture2D>("Sprites/Buttons/FishingMiniGame"), coreAssets.Load<Texture2D>("Sprites/Buttons/FishingMiniGameClicked"),
                                                new Point(64, 33), fishingButtonPosition, "Fishing Minigame", 36, true);

            overworldBlueprint = new Rectangle[10,13] {
                { T09, T00, T00, T00, T14, T17, T17, T20, T00, T00, T00, T00, T00 },
                { T09, T00, T00, T00, T14, T17, T17, T21, T19, T00, T22, T05, T00 },
                { T11, T00, T00, T00, T15, T17, T17, T17, T21, T19, T00, T01, T00 },
                { T03, T03, T03, T05, T00, T15, T16, T16, T16, T18, T00, T01, T00 },
                { T10, T00, T07, T01, T08, T08, T00, T00, T00, T00, T00, T01, T00 },
                { T09, T06, T06, T01, T06, T07, T00, T00, T00, T00, T00, T01, T00 },
                { T09, T06, T23, T01, T23, T06, T08, T00, T00, T00, T00, T01, T00 },
                { T09, T06, T06, T01, T23, T06, T07, T00, T00, T00, T00, T01, T00 },
                { T09, T12, T12, T04, T03, T03, T03, T03, T03, T02, T03, T13, T00 },
                { T09, T00, T00, T00, T00, T00, T00, T00, T00, T00, T00, T00, T00 }
            };

            

            //adds containment collision rectangles for movement
            colliders.Add(new Rectangle(0, 3*mult, 4*mult, mult));
            colliders.Add(new Rectangle(3*mult, 3*mult, mult, 6*mult));
            colliders.Add(new Rectangle(5*mult, 8*mult, 7*mult, mult));
            colliders.Add(new Rectangle(11*mult, mult, mult, 5*mult));
            //gap between these two
            colliders.Add(new Rectangle(11*mult, 7*mult, mult, 2*mult));
            colliders.Add(new Rectangle(10*mult, mult, 2*mult, mult));
            colliders.Add(new Rectangle(9*mult, 7*mult, mult, 2*mult));

            //adds the colliders so that player can move on path if roadblock is missing
            if(!roadBlock1) {
                colliders.Add(new Rectangle(3*mult, 8*mult, 3*mult, mult));
            }
            if(!roadBlock2) {
                colliders.Add(new Rectangle(11*mult, 5*mult, mult, 3*mult));
            }

            if(SaveFile.doesFileExist()) {
                LoadData();
            }
        }

        //use this to see if the requested movement moves out of allowed bounds
        //rectangles must overlap each other in some way so that softlock edge doesn't
        //form between two rectangles
        private bool ValidateMovement(string direction, int amount) {
            bool isValid = false;

            Rectangle testBounds;
            if(direction == "H") { //horizontal movement
                testBounds = new Rectangle(catBounds.X+amount, catBounds.Y, catBounds.Width, catBounds.Height);

                for(int i = 0; i < colliders.Count; i++) {
                    if(colliders[i].Contains(testBounds)) { //moment cat box may leave
                        isValid = true;
                    }
                }
            } else if(direction == "V") {//vertical movement
                testBounds = new Rectangle(catBounds.X, catBounds.Y+amount, catBounds.Width, catBounds.Height);

                for(int i = 0; i < colliders.Count; i++) {
                    if(colliders[i].Contains(testBounds)) { //moment cat box may leave
                        isValid = true;
                    }
                }
            }
            return isValid;
        }

        public void CleanupProcesses()
        {
            catPos = new Vector2(0, 192);
        }

        public void SaveData(SaveFile saveFile)
        {
            saveFile.catPosX = (int)catPos.X;
            saveFile.catPosY = (int)catPos.Y;
        }

        public void LoadData()
        {
            catPos = new Vector2(GameHandler.saveFile.catPosX, GameHandler.saveFile.catPosY);
        }

        public void Dispose()
        {
            throw new NotImplementedException();
        }
    }
}