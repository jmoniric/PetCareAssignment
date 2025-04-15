using System;
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

        private PetCare PetCareLevel { get; set; }
        private CatFishing FishingLevel { get; set; }
        private WheresWaldo WaldoLevel { get; set; }
        private SlidingGame SlidingLevel { get; set; }
        private PauseMenu PauseMenu { get; set; }
        private ContentManager _coreAssets { get; set; }

        public Overworld(PetCare pet, WheresWaldo waldo, SlidingGame sliding, PauseMenu pauseMenu, Button pauseB)
        {
            PetCareLevel = pet;
            WaldoLevel = waldo;
            SlidingLevel = sliding;
            PauseMenu = pauseMenu;
            _coreAssets = GameHandler.coreAssets;
            pauseButton = pauseB;
            LoadLevel();
        }

        public void HandleInput(GameTime gameTime)
        {
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
                    PauseMenu.LoadContent(null, _coreAssets);
                    //prevents checking of other buttons while in pause menu
                }
                else if (!GameHandler.isPaused)
                {
                    if (petCareButton.CheckIfSelectButtonWasClicked())
                    {
                        SetVisiblity(false); //hides buttons to prevent them from being pressed again
                        petCareButton.Clicked();
                        GameHandler.CurrentState = GameHandler.GameState.PetCareGame;
                        PetCareLevel.LoadContent(GameHandler.petcareAssets, _coreAssets);
                        PetCareLevel.LoadLevel();
                    }
                    else if (waldoButton.CheckIfSelectButtonWasClicked())
                    {
                        SetVisiblity(false);
                        waldoButton.Clicked();
                        GameHandler.CurrentState = GameHandler.GameState.WaldoGame;
                        WaldoLevel.LoadContent(GameHandler.waldoAssets, _coreAssets);
                        WaldoLevel.LoadLevel();
                    }
                    else if (slidingButton.CheckIfSelectButtonWasClicked())
                    {
                        SetVisiblity(false);
                        slidingButton.Clicked();
                        GameHandler.CurrentState = GameHandler.GameState.SlidingGame;
                        SlidingLevel.LoadContent(GameHandler.slidingAssets, _coreAssets);
                        SlidingLevel.LoadLevel();
                    }
                    else if (fishingButton.CheckIfSelectButtonWasClicked())
                    {
                        SetVisiblity(false);
                        fishingButton.Clicked();
                        GameHandler.CurrentState = GameHandler.GameState.FishingGame;
                        FishingLevel.LoadContent(GameHandler.fishingAssets, _coreAssets);
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
            Rectangle sourceRectangle = new Rectangle(0, 0, petCareButton.CellWidth, petCareButton.CellHeight);
            Rectangle sourceRectangle1 = new Rectangle(0, 0, waldoButton.CellWidth, waldoButton.CellHeight);
            Rectangle sourceRectangle2 = new Rectangle(0, 0, slidingButton.CellWidth, slidingButton.CellHeight);
            Rectangle sourceRectangle3 = new Rectangle(0, 0, fishingButton.CellWidth, fishingButton.CellHeight);

            Rectangle destinationRectangle = new Rectangle((int)petCareButtonPosition.X, (int)petCareButtonPosition.Y, petCareButton.CellWidth, petCareButton.CellHeight);
            Rectangle destinationRectangle1 = new Rectangle((int)waldoButtonPosition.X, (int)waldoButtonPosition.Y, waldoButton.CellWidth, waldoButton.CellHeight);
            Rectangle destinationRectangle2 = new Rectangle((int)slidingButtonPosition.X, (int)slidingButtonPosition.Y, slidingButton.CellWidth, slidingButton.CellHeight);
            Rectangle destinationRectangle3 = new Rectangle((int)fishingButtonPosition.X, (int)fishingButtonPosition.Y, fishingButton.CellWidth, fishingButton.CellHeight);

            spriteBatch.Draw(petCareButton.Texture, destinationRectangle, sourceRectangle, Color.White, 0.0f, Vector2.Zero, SpriteEffects.None, 1.0f);
            spriteBatch.Draw(waldoButton.Texture, destinationRectangle1, sourceRectangle1, Color.White, 0.0f, Vector2.Zero, SpriteEffects.None, 1.0f);
            spriteBatch.Draw(slidingButton.Texture, destinationRectangle2, sourceRectangle2, Color.White, 0.0f, Vector2.Zero, SpriteEffects.None, 1.0f);
            spriteBatch.Draw(fishingButton.Texture, destinationRectangle3, sourceRectangle3, Color.White, 0.0f, Vector2.Zero, SpriteEffects.None, 1.0f);
        }

        public void Update(GameTime gameTime)
        {
            HandleInput(gameTime);
        }

        public void LoadContent(ContentManager _manager, ContentManager assets)
        {
            throw new NotImplementedException();
        }

        public void LoadLevel()
        {
            petCareButtonPosition = new Vector2(100, 100);
            waldoButtonPosition = new Vector2(164, 100);
            slidingButtonPosition = new Vector2(228, 100);
            fishingButtonPosition = new Vector2(292, 100);

            petCareButton = new Button(_coreAssets.Load<Texture2D>("Sprites/Buttons/PetCareMiniGame"), _coreAssets.Load<Texture2D>("Sprites/Buttons/PetCareMiniGameClicked"),
                                                new Point(64, 33), petCareButtonPosition, "Pet Care Minigame", 33, true);
            waldoButton = new Button(_coreAssets.Load<Texture2D>("Sprites/Buttons/WaldoMiniGame"), _coreAssets.Load<Texture2D>("Sprites/Buttons/WaldoMiniGameClicked"),
                                                new Point(64, 33), waldoButtonPosition, "Where's Waldo Minigame", 34, true);
            slidingButton = new Button(_coreAssets.Load<Texture2D>("Sprites/Buttons/SlideMiniGame"), _coreAssets.Load<Texture2D>("Sprites/Buttons/SlideMiniGameClicked"),
                                                new Point(64, 33), slidingButtonPosition, "Sliding Minigame", 35, true);
            fishingButton = new Button(_coreAssets.Load<Texture2D>("Sprites/Buttons/FishingMiniGame"), _coreAssets.Load<Texture2D>("Sprites/Buttons/FishingMiniGameClicked"),
                                                new Point(64, 33), fishingButtonPosition, "Fishing Minigame", 36, true);
        }

        public void CleanupProcesses()
        {
            throw new NotImplementedException();
        }

        public void SaveData()
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