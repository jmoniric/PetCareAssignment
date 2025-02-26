using System;
using System.Diagnostics;
using System.Net.Mime;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace PetCareGame;

public class GameHandler : Game
{
    enum GameState
    {
        MainMenu,
        PetCareGame,
        WaldoGame,
        FishingGame,
        SlidingGame
    }
    GameState CurrentState = GameState.MainMenu;

    private GraphicsDeviceManager _graphics;
    private SpriteBatch _spriteBatch;
    private Button _petCareButton;
    private Vector2 _petCareButtonPosition;
    private Button _waldoButton;
    private Vector2 _waldoButtonPosition;
    private Button _slidingButton;
    private Vector2 _slidingButtonPosition;
    private Button _fishingButton;
    private Vector2 _fishingButtonPosition;
    private MouseState _oneShotMouseState;
    private bool _mouseLeftPressed;

    private Button pauseButton;
    private Texture2D pauseButtonTexture;
    private Vector2 pausePos;

    private PetCare _petCareLevel = new PetCare();
    private CatFishing _fishingLevel = new CatFishing();
    private WheresWaldo _waldoLevel = new WheresWaldo();
    private SlidingGame _slidingLevel = new SlidingGame();
    private PauseMenu _pauseMenu = new PauseMenu();
    ContentManager _coreAssets;
    ContentManager _slidingAssets;
    ContentManager _waldoAssets;
    ContentManager _fishingAssets;
    ContentManager _petcareAssets;

    public bool isPaused = false;

    public static int windowHeight = 1080;
    public static int windowWidth = 1920;

    public static AnimatedTexture catIdle = new AnimatedTexture(new Vector2(32,16), 0f, 3f, 0.5f);

    
    public GameHandler()
    {
        _graphics = new GraphicsDeviceManager(this);

        //setting our preffered window size
        _graphics.PreferredBackBufferHeight = windowHeight;
        _graphics.PreferredBackBufferWidth = windowWidth;

        //rather than using the static methods from the content class, we should make separate content managers for separate sets of assets
        _coreAssets = new ContentManager(Content.ServiceProvider);
        _coreAssets.RootDirectory = "Content/Core";

        _slidingAssets = new ContentManager(Content.ServiceProvider);
        _slidingAssets.RootDirectory = "Content/SlidingGame";

        _waldoAssets = new ContentManager(Content.ServiceProvider);
        _waldoAssets.RootDirectory = "Content/WaldoGame";

        _fishingAssets = new ContentManager(Content.ServiceProvider);
        _fishingAssets.RootDirectory = "Content/FishingGame";

        _petcareAssets = new ContentManager(Content.ServiceProvider);
        _petcareAssets.RootDirectory = "Content/PetcareGame";

        IsMouseVisible = true;
    }

    protected override void Initialize()
    {
        // TODO: Add your initialization logic here
        _petCareButtonPosition = new Vector2(100, 100);
        _waldoButtonPosition = new Vector2(164, 100);
        _slidingButtonPosition = new Vector2(228, 100);
        _fishingButtonPosition = new Vector2(292, 100);
        _oneShotMouseState = OneShotMouseButtons.GetState();
        pausePos = Vector2.Zero;
        _mouseLeftPressed = false;

        base.Initialize();
    }

    protected override void LoadContent()
    {
        _spriteBatch = new SpriteBatch(GraphicsDevice);
        // TODO: use this.Content to load your game content here
        
        _petCareButton = new Button(_coreAssets.Load<Texture2D>("Sprites/Buttons/PetCareMiniGame"), _coreAssets.Load<Texture2D>("Sprites/Buttons/PetCareMiniGameClicked"), 
                                            new Point(64, 33), _petCareButtonPosition, "Pet Care Minigame", 33, true);
        _waldoButton = new Button(_coreAssets.Load<Texture2D>("Sprites/Buttons/WaldoMiniGame"), _coreAssets.Load<Texture2D>("Sprites/Buttons/WaldoMiniGameClicked"),
                                            new Point(64, 33), _waldoButtonPosition, "Where's Waldo Minigame", 34, true);
        _slidingButton = new Button(_coreAssets.Load<Texture2D>("Sprites/Buttons/SlideMiniGame"), _coreAssets.Load<Texture2D>("Sprites/Buttons/SlideMiniGameClicked"),
                                            new Point(64, 33), _slidingButtonPosition, "Sliding Minigame", 35, true);
        _fishingButton = new Button(_coreAssets.Load<Texture2D>("Sprites/Buttons/FishingMiniGame"), _coreAssets.Load<Texture2D>("Sprites/Buttons/FishingMiniGameClicked"),
                                            new Point(64, 33), _fishingButtonPosition, "Fishing Minigame", 36, true);
        catIdle.Load(_coreAssets, "Sprites/Animal/idle", 7, 5);

        pauseButtonTexture = _coreAssets.Load<Texture2D>("Sprites/core_textureatlas");
        pauseButton = new Button(pauseButtonTexture, pauseButtonTexture, new Point(64,64), Vector2.Zero, "Pause", 37, true);

    }

    public void HandleInput(GameTime gameTime)
    {
        
        _oneShotMouseState = OneShotMouseButtons.GetState();

        if(_oneShotMouseState.LeftButton == ButtonState.Pressed)
        {
            if(OneShotMouseButtons.HasNotBeenPressed(true))
            {
                _mouseLeftPressed = true;
            }
        }

        _petCareButton.UpdateButton();
        _waldoButton.UpdateButton();
        _slidingButton.UpdateButton();
        _fishingButton.UpdateButton();
        pauseButton.UpdateButton();

        //for now, loading the next minigame will be handeled via the four buttons
        //but in the future, there will be conditional logic to allow for whether a
        //minigame should be loaded, whether the button is enabled, etc
        if(_mouseLeftPressed)
        {
            _mouseLeftPressed = false;
            if(CheckIfButtonWasClicked(pauseButton)) {
                pauseButton.Clicked();
                isPaused = true;
                _pauseMenu.LoadLevel();
            } else if(CheckIfButtonWasClicked(_petCareButton))
            {
                SetVisiblity(false); //hides buttons to prevent them from being pressed again
                _petCareButton.Clicked();
                CurrentState = GameState.PetCareGame;
                _petCareLevel.LoadContent(_petcareAssets, _coreAssets);
                _petCareLevel.LoadLevel();
            } else if(CheckIfButtonWasClicked(_waldoButton))
            {
                SetVisiblity(false);
                _waldoButton.Clicked();
                CurrentState = GameState.WaldoGame;
                _waldoLevel.LoadContent(_waldoAssets, _coreAssets);
                _waldoLevel.LoadLevel();
            } else if(CheckIfButtonWasClicked(_slidingButton))
            {
                SetVisiblity(false);
                _slidingButton.Clicked();
                CurrentState = GameState.SlidingGame;
                _slidingLevel.LoadContent(_slidingAssets, _coreAssets);
                _slidingLevel.LoadLevel();
            } else if(CheckIfButtonWasClicked(_fishingButton))
            {
                SetVisiblity(false);
                _fishingButton.Clicked();
                CurrentState = GameState.FishingGame;
                _fishingLevel.LoadContent(_fishingAssets, _coreAssets);
                _fishingLevel.LoadLevel();
            }
        }

        switch(CurrentState) {
            case GameState.MainMenu:
                //handle the input for main menu directly here
                break;
            case GameState.PetCareGame:
                _petCareLevel.HandleInput(gameTime);
                break;
            case GameState.FishingGame:
                _fishingLevel.HandleInput(gameTime);
                break;
            case GameState.WaldoGame:
                _waldoLevel.HandleInput(gameTime);
                break;
            case GameState.SlidingGame:
                _slidingLevel.HandleInput(gameTime);
                break;   
        }
    }

    protected override void Update(GameTime gameTime)
    {
        HandleInput(gameTime);
            
        //if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
          //  Exit();
        if(isPaused) {
            _pauseMenu.Update(gameTime);
        } else {
            switch(CurrentState) {
                case GameState.MainMenu:
                    //handle the update for main menu directly here
                    break;
                case GameState.PetCareGame:
                    _petCareLevel.Update(gameTime);
                    break;
                case GameState.FishingGame:
                    _fishingLevel.Update(gameTime);
                    break;
                case GameState.WaldoGame:
                    _waldoLevel.Update(gameTime);
                    break;
                case GameState.SlidingGame:
                    _slidingLevel.Update(gameTime);
                    break;   
            }
        }

        base.Update(gameTime);
    }

    protected override void Draw(GameTime gameTime)
    {
        GraphicsDevice.Clear(Color.CornflowerBlue);

        _spriteBatch.Begin(SpriteSortMode.Deferred, null, SamplerState.PointClamp);

        switch(CurrentState) {
            case GameState.MainMenu:
                Rectangle sourceRectangle = new Rectangle(0, 0, _petCareButton.CellWidth, _petCareButton.CellHeight);
                Rectangle sourceRectangle1 = new Rectangle(0, 0, _waldoButton.CellWidth, _waldoButton.CellHeight);
                Rectangle sourceRectangle2 = new Rectangle(0, 0, _slidingButton.CellWidth, _slidingButton.CellHeight);
                Rectangle sourceRectangle3 = new Rectangle(0, 0, _fishingButton.CellWidth, _fishingButton.CellHeight);
                
                Rectangle destinationRectangle = new Rectangle((int)_petCareButtonPosition.X, (int)_petCareButtonPosition.Y, _petCareButton.CellWidth, _petCareButton.CellHeight);
                Rectangle destinationRectangle1 = new Rectangle((int)_waldoButtonPosition.X, (int)_waldoButtonPosition.Y, _waldoButton.CellWidth, _waldoButton.CellHeight);
                Rectangle destinationRectangle2 = new Rectangle((int)_slidingButtonPosition.X, (int)_slidingButtonPosition.Y, _slidingButton.CellWidth, _slidingButton.CellHeight);
                Rectangle destinationRectangle3 = new Rectangle((int)_fishingButtonPosition.X, (int)_fishingButtonPosition.Y, _fishingButton.CellWidth, _fishingButton.CellHeight);
                Rectangle pauseDestination = new Rectangle((int)pausePos.X, (int)pausePos.Y, pauseButton.CellWidth, pauseButton.CellHeight);
                
                _spriteBatch.Draw(_petCareButton.Texture, destinationRectangle, sourceRectangle, Color.White, 0.0f, Vector2.Zero, SpriteEffects.None, 1.0f);
                _spriteBatch.Draw(_waldoButton.Texture, destinationRectangle1, sourceRectangle1, Color.White, 0.0f, Vector2.Zero, SpriteEffects.None, 1.0f);
                _spriteBatch.Draw(_slidingButton.Texture, destinationRectangle2, sourceRectangle2, Color.White, 0.0f, Vector2.Zero, SpriteEffects.None, 1.0f);
                _spriteBatch.Draw(_fishingButton.Texture, destinationRectangle3, sourceRectangle3, Color.White, 0.0f,Vector2.Zero, SpriteEffects.None, 1.0f);
                break;
            case GameState.PetCareGame:
                _petCareLevel.Draw(gameTime, _spriteBatch, _graphics);
                break;
            case GameState.FishingGame:
                _fishingLevel.Draw(gameTime, _spriteBatch, _graphics);
                break;
            case GameState.WaldoGame:
                _waldoLevel.Draw(gameTime, _spriteBatch, _graphics);
                break;
            case GameState.SlidingGame:
                _slidingLevel.Draw(gameTime, _spriteBatch, _graphics);
                break;   
        }

        //draw pause button last
        _spriteBatch.Draw(pauseButton.Texture, new Rectangle(1840,10,64,64), new Rectangle(0,0,16,16), Color.White);
        _spriteBatch.End();

        if(isPaused) {
            _pauseMenu.Draw(gameTime, _spriteBatch, _graphics);
        }
        
        base.Draw(gameTime);
    }

    private bool CheckIfButtonWasClicked(Button button)
    {
        if(button.Visible) {
            if(_oneShotMouseState.X >= button.Position.X && _oneShotMouseState.X <= (button.Position.X + button.Dimensions.X))
            {
                if(_oneShotMouseState.Y >= button.Position.Y && _oneShotMouseState.Y <= (button.Position.Y + button.Dimensions.Y))
                {
                    return true;
                }
            }
        }
        return false;
    }

    //hide or show visiblity of items in here
    private void SetVisiblity(bool isVisible) {
        _petCareButton.Visible = isVisible;
        _fishingButton.Visible = isVisible;
        _waldoButton.Visible = isVisible;
        _slidingButton.Visible = isVisible;
    }
}
