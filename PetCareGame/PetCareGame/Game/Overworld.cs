using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace PetCareGame
{
    public class Overworld : LevelInterface
    {

        private static Button _petCareButton;
        private Vector2 _petCareButtonPosition;
        private static Button _waldoButton;
        private Vector2 _waldoButtonPosition;
        private static Button _slidingButton;
        private Vector2 _slidingButtonPosition;
        private static Button _fishingButton;
        private Vector2 _fishingButtonPosition;
        private static Button pauseButton;

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
            _coreAssets = GameHandler._coreAssets;
            pauseButton = pauseB;
            LoadLevel();
        }

        public void HandleInput(GameTime gameTime)
        {
            //for now, loading the next minigame will be handeled via the four buttons
            //but in the future, there will be conditional logic to allow for whether a
            //minigame should be loaded, whether the button is enabled, etc
            if (GameHandler._mouseLeftPressed)
            {
                GameHandler._mouseLeftPressed = false;
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
                    if (_petCareButton.CheckIfSelectButtonWasClicked())
                    {
                        SetVisiblity(false); //hides buttons to prevent them from being pressed again
                        _petCareButton.Clicked();
                        GameHandler.CurrentState = GameHandler.GameState.PetCareGame;
                        PetCareLevel.LoadContent(GameHandler._petcareAssets, _coreAssets);
                        PetCareLevel.LoadLevel();
                    }
                    else if (_waldoButton.CheckIfSelectButtonWasClicked())
                    {
                        SetVisiblity(false);
                        _waldoButton.Clicked();
                        GameHandler.CurrentState = GameHandler.GameState.WaldoGame;
                        WaldoLevel.LoadContent(GameHandler._waldoAssets, _coreAssets);
                        WaldoLevel.LoadLevel();
                    }
                    else if (_slidingButton.CheckIfSelectButtonWasClicked())
                    {
                        SetVisiblity(false);
                        _slidingButton.Clicked();
                        GameHandler.CurrentState = GameHandler.GameState.SlidingGame;
                        SlidingLevel.LoadContent(GameHandler._slidingAssets, _coreAssets);
                        SlidingLevel.LoadLevel();
                    }
                    else if (_fishingButton.CheckIfSelectButtonWasClicked())
                    {
                        SetVisiblity(false);
                        _fishingButton.Clicked();
                        GameHandler.CurrentState = GameHandler.GameState.FishingGame;
                        FishingLevel.LoadContent(GameHandler._fishingAssets, _coreAssets);
                        FishingLevel.LoadLevel();
                    }
                }
            }
        }

        //hide or show visiblity of items in here
        public static void SetVisiblity(bool isVisible)
        {
            _petCareButton.Visible = isVisible;
            _fishingButton.Visible = isVisible;
            _waldoButton.Visible = isVisible;
            _slidingButton.Visible = isVisible;
        }

        public void Draw(GameTime gameTime, SpriteBatch spriteBatch, GraphicsDeviceManager _graphics)
        {
            Rectangle sourceRectangle = new Rectangle(0, 0, _petCareButton.CellWidth, _petCareButton.CellHeight);
            Rectangle sourceRectangle1 = new Rectangle(0, 0, _waldoButton.CellWidth, _waldoButton.CellHeight);
            Rectangle sourceRectangle2 = new Rectangle(0, 0, _slidingButton.CellWidth, _slidingButton.CellHeight);
            Rectangle sourceRectangle3 = new Rectangle(0, 0, _fishingButton.CellWidth, _fishingButton.CellHeight);

            Rectangle destinationRectangle = new Rectangle((int)_petCareButtonPosition.X, (int)_petCareButtonPosition.Y, _petCareButton.CellWidth, _petCareButton.CellHeight);
            Rectangle destinationRectangle1 = new Rectangle((int)_waldoButtonPosition.X, (int)_waldoButtonPosition.Y, _waldoButton.CellWidth, _waldoButton.CellHeight);
            Rectangle destinationRectangle2 = new Rectangle((int)_slidingButtonPosition.X, (int)_slidingButtonPosition.Y, _slidingButton.CellWidth, _slidingButton.CellHeight);
            Rectangle destinationRectangle3 = new Rectangle((int)_fishingButtonPosition.X, (int)_fishingButtonPosition.Y, _fishingButton.CellWidth, _fishingButton.CellHeight);

            spriteBatch.Draw(_petCareButton.Texture, destinationRectangle, sourceRectangle, Color.White, 0.0f, Vector2.Zero, SpriteEffects.None, 1.0f);
            spriteBatch.Draw(_waldoButton.Texture, destinationRectangle1, sourceRectangle1, Color.White, 0.0f, Vector2.Zero, SpriteEffects.None, 1.0f);
            spriteBatch.Draw(_slidingButton.Texture, destinationRectangle2, sourceRectangle2, Color.White, 0.0f, Vector2.Zero, SpriteEffects.None, 1.0f);
            spriteBatch.Draw(_fishingButton.Texture, destinationRectangle3, sourceRectangle3, Color.White, 0.0f, Vector2.Zero, SpriteEffects.None, 1.0f);
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
            _petCareButtonPosition = new Vector2(100, 100);
            _waldoButtonPosition = new Vector2(164, 100);
            _slidingButtonPosition = new Vector2(228, 100);
            _fishingButtonPosition = new Vector2(292, 100);

            _petCareButton = new Button(_coreAssets.Load<Texture2D>("Sprites/Buttons/PetCareMiniGame"), _coreAssets.Load<Texture2D>("Sprites/Buttons/PetCareMiniGameClicked"),
                                    new Point(64, 33), _petCareButtonPosition, "Pet Care Minigame", 33, true);
            _waldoButton = new Button(_coreAssets.Load<Texture2D>("Sprites/Buttons/WaldoMiniGame"), _coreAssets.Load<Texture2D>("Sprites/Buttons/WaldoMiniGameClicked"),
                                                new Point(64, 33), _waldoButtonPosition, "Where's Waldo Minigame", 34, true);
            _slidingButton = new Button(_coreAssets.Load<Texture2D>("Sprites/Buttons/SlideMiniGame"), _coreAssets.Load<Texture2D>("Sprites/Buttons/SlideMiniGameClicked"),
                                                new Point(64, 33), _slidingButtonPosition, "Sliding Minigame", 35, true);
            _fishingButton = new Button(_coreAssets.Load<Texture2D>("Sprites/Buttons/FishingMiniGame"), _coreAssets.Load<Texture2D>("Sprites/Buttons/FishingMiniGameClicked"),
                                                new Point(64, 33), _fishingButtonPosition, "Fishing Minigame", 36, true);
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