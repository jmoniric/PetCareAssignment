using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
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

        private Rectangle T00 = new Rectangle(0,0,16,16); //grass
        private Rectangle T01 = new Rectangle(16,16,16,16); //vertical path
        private Rectangle T02 = new Rectangle(48,16,16,16); //T-intersection pointing up
        private Rectangle T03 = new Rectangle(32,0,16,16); //horizontal path
        private Rectangle T04 = new Rectangle(0,32,16,16); //path elbow from top to right
        private Rectangle T05 = new Rectangle(16,32,16,16); //path elbow from left to bottom
        private Rectangle T06 = new Rectangle(16,0,16,16); //flower field
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

            //updates cat position, ensuring requested movement doesn't go out of bounds
            if(!GameHandler.isPaused) {
                if(kybdState.GetPressedKeys().Contains(Keys.A) || kybdState.GetPressedKeys().Contains(Keys.Left)) {
                    if(ValidateMovement("H", speedH*-1)) {
                        catPos.X -= speedH;
                    }
                } else if(kybdState.GetPressedKeys().Contains(Keys.D) || kybdState.GetPressedKeys().Contains(Keys.Right)) {
                    if(ValidateMovement("H", speedH)) {
                        catPos.X += speedH;
                    }
                } else if(kybdState.GetPressedKeys().Contains(Keys.W) || kybdState.GetPressedKeys().Contains(Keys.Up)) {
                    if(ValidateMovement("V", speedV*-1)) {
                        catPos.Y -= speedV;
                    }
                } else if(kybdState.GetPressedKeys().Contains(Keys.S) || kybdState.GetPressedKeys().Contains(Keys.Down)) {
                    if(ValidateMovement("V", speedV)) {
                        catPos.Y += speedV;
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
                    if (petCareButton.CheckIfSelectButtonWasClicked())
                    {
                        SetVisiblity(false); //hides buttons to prevent them from being pressed again
                        petCareButton.Clicked();
                        GameHandler.CurrentState = GameHandler.GameState.PetCareGame;
                        PetCareLevel.LoadContent(GameHandler.petcareAssets, coreAssets);
                        PetCareLevel.LoadLevel();
                    }
                    else if (waldoButton.CheckIfSelectButtonWasClicked())
                    {
                        SetVisiblity(false);
                        waldoButton.Clicked();
                        GameHandler.CurrentState = GameHandler.GameState.WaldoGame;
                        WaldoLevel.LoadContent(GameHandler.waldoAssets, coreAssets);
                        WaldoLevel.LoadLevel();
                    }
                    else if (slidingButton.CheckIfSelectButtonWasClicked())
                    {
                        SetVisiblity(false);
                        slidingButton.Clicked();
                        GameHandler.CurrentState = GameHandler.GameState.SlidingGame;
                        SlidingLevel.LoadContent(GameHandler.slidingAssets, coreAssets);
                        SlidingLevel.LoadLevel();
                    }
                    else if (fishingButton.CheckIfSelectButtonWasClicked())
                    {
                        SetVisiblity(false);
                        fishingButton.Clicked();
                        GameHandler.CurrentState = GameHandler.GameState.FishingGame;
                        FishingLevel.LoadContent(GameHandler.fishingAssets, coreAssets);
                        FishingLevel.LoadLevel();
                    }
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

            spriteBatch.Draw(atlas, new Rectangle(448,256,256,256), settlement, Color.White);
            spriteBatch.Draw(atlas, new Rectangle(576,64,64,64), house, Color.White);

            //debug draw colliders
            /*for(int i = 0; i < colliders.Count; i++) {
                spriteBatch.Draw(GameHandler.plainWhiteTexture, colliders[i], Color.Orange);
            }*/

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
            
            
            
            spriteBatch.Draw(petCareButton.Texture, destinationRectangle, sourceRectangle, Color.White, 0.0f, Vector2.Zero, SpriteEffects.None, 1.0f);
            spriteBatch.Draw(waldoButton.Texture, destinationRectangle1, sourceRectangle1, Color.White, 0.0f, Vector2.Zero, SpriteEffects.None, 1.0f);
            spriteBatch.Draw(slidingButton.Texture, destinationRectangle2, sourceRectangle2, Color.White, 0.0f, Vector2.Zero, SpriteEffects.None, 1.0f);
            spriteBatch.Draw(fishingButton.Texture, destinationRectangle3, sourceRectangle3, Color.White, 0.0f, Vector2.Zero, SpriteEffects.None, 1.0f);
            
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

            //adds containment collision rectangles for movement
            colliders.Add(new Rectangle(0, 192, 256, 64));
            colliders.Add(new Rectangle(192, 192, 64, 384));
            colliders.Add(new Rectangle(192, 512, 576, 64));
            colliders.Add(new Rectangle(704, 64, 64, 512));
            colliders.Add(new Rectangle(640, 64, 128, 64));
            colliders.Add(new Rectangle(576, 448, 64, 128));
        }

        public void LoadLevel()
        {
            petCareButtonPosition = new Vector2(100, 100);
            waldoButtonPosition = new Vector2(164, 100);
            slidingButtonPosition = new Vector2(228, 100);
            fishingButtonPosition = new Vector2(292, 100);

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
                { T09, T06, T06, T01, T06, T06, T08, T00, T00, T00, T00, T01, T00 },
                { T09, T06, T06, T01, T06, T06, T07, T00, T00, T00, T00, T01, T00 },
                { T09, T12, T12, T04, T03, T03, T03, T03, T03, T02, T03, T13, T00 },
                { T09, T00, T00, T00, T00, T00, T00, T00, T00, T00, T00, T00, T00 }
            };
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
            throw new NotImplementedException();
        }

        public void SaveData(SaveFile saveFile)
        {
            throw new NotImplementedException();
        }

        public void LoadData()
        {
            throw new NotImplementedException();
        }

        public void Dispose()
        {
            throw new NotImplementedException();
        }
    }
}