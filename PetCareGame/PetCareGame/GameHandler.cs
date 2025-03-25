using System;
using System.Diagnostics;
using System.Net.Mime;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Input.Touch;

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
    private DisplayManager _displayManager;

    private Button _petCareButton;
    private Vector2 _petCareButtonPosition;
    private Button _waldoButton;
    private Vector2 _waldoButtonPosition;
    private Button _slidingButton;
    private Vector2 _slidingButtonPosition;
    private Button _fishingButton;
    private Vector2 _fishingButtonPosition;
    public static MouseState _mouseState;
    public static bool _mouseLeftPressed;

    private Button pauseButton;
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

    public static bool isPaused = false;

    public static int windowHeight = 600;
    public static int windowWidth = 800;
    public static Vector2 _relativeMousePos;
    public static AnimatedTexture catIdle = new AnimatedTexture(new Vector2(32,16), 0f, 3f, 0.5f);
    public static AnimatedTexture catIrritated = new AnimatedTexture(new Vector2(32,16), 0f, 3f, 0.5f);
    public static AnimatedTexture catAttack = new AnimatedTexture(new Vector2(32,16), 0f, 3f, 0.5f);
    public static AnimatedTexture catWalk = new AnimatedTexture(new Vector2(32,16), 0f, 3f, 0.5f);
    public static SoundEffect catPurr;
    public static SoundEffectInstance selectSfx;
    public static SoundEffectInstance failSfx;
    public static SoundEffectInstance successSfx;

    public static Texture2D coreTextureAtlas;
    public static Texture2D plainWhiteTexture;
    public static Texture2D gaugeTextureAtlas;

    //fonts
    public static SpriteFont courierNew36;
    public static SpriteFont courierNew52;
    public static SpriteFont highPixel22;
    public static SpriteFont highPixel36;

    public static Vector2 baseScreenSize = new Vector2(800, 600);

    private bool _isResizing;

    public static bool _allowAudio = true;

    
    public GameHandler()
    {
        _graphics = new GraphicsDeviceManager(this);

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

        Window.AllowUserResizing = true;
        Window.ClientSizeChanged += OnClientSizeChange;
    }

    private void OnClientSizeChange(object sender, EventArgs e)
    {
        if(!_isResizing && Window.ClientBounds.Width > 0 && Window.ClientBounds.Height > 0)
        {
            _isResizing = true;
            _displayManager.UpdateScreenScaleMatrix();
            _isResizing = false;
        }
    }

    protected override void Initialize()
    {
        // TODO: Add your initialization logic here
        _displayManager = new(this, _graphics, GraphicsDevice);
        _displayManager.UpdateScreenScaleMatrix();

        _petCareButtonPosition = new Vector2(100, 100);
        _waldoButtonPosition = new Vector2(164, 100);
        _slidingButtonPosition = new Vector2(228, 100);
        _fishingButtonPosition = new Vector2(292, 100);

        _mouseState = OneShotMouseButtons.GetState();
        pausePos = new Vector2(750,10);
        _mouseLeftPressed = false;

        //Window.AllowUserResizing = true;

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
        

        //Core assets
        catIdle.Load(_coreAssets, "Sprites/Animal/idle", 7, 5);
        catIrritated.Load(_coreAssets, "Sprites/Animal/irritated", 4, 6);
        catAttack.Load(_coreAssets, "Sprites/Animal/attack", 3, 4);
        catWalk.Load(_coreAssets, "Sprites/Animal/walk", 7, 5);

        try {
            catPurr = _coreAssets.Load<SoundEffect>("Sounds/Animal/cat_purr");
            selectSfx = _coreAssets.Load<SoundEffect>("Sounds/UI/select").CreateInstance();
            selectSfx.Volume = 0.5f;
            failSfx = _coreAssets.Load<SoundEffect>("Sounds/UI/fail").CreateInstance();
            failSfx.Volume = 0.5f;
            successSfx = _coreAssets.Load<SoundEffect>("Sounds/UI/success").CreateInstance();
            successSfx.Volume = 0.5f;
        } catch (NoAudioHardwareException e) {
            _allowAudio = false;
            Console.WriteLine("No audio drivers found, disabling audio");
            Console.WriteLine(e.StackTrace);
        }
        

        coreTextureAtlas = _coreAssets.Load<Texture2D>("Sprites/core_textureatlas");
        gaugeTextureAtlas = _coreAssets.Load<Texture2D>("Sprites/gauge_atlas");
        pauseButton = new Button(coreTextureAtlas, coreTextureAtlas, new Point(48,48), pausePos, "Pause", 37, true);
        plainWhiteTexture = _coreAssets.Load<Texture2D>("Sprites/plain_white");
        
        //fonts
        courierNew36 = _coreAssets.Load<SpriteFont>("Fonts/courier_new_36");
        courierNew52 = _coreAssets.Load<SpriteFont>("Fonts/courier_new_52");
        highPixel22 = _coreAssets.Load<SpriteFont>("Fonts/high_pixel_22");
        highPixel36 = _coreAssets.Load<SpriteFont>("Fonts/high_pixel_36");

    }

    public void HandleInput(GameTime gameTime)
    {
        
        _mouseState = OneShotMouseButtons.GetState();

        if(_mouseState.LeftButton == ButtonState.Pressed)
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
            //checks if already paused to prevent spamming of button
            if(pauseButton.CheckIfButtonWasClicked() && !isPaused) {
                pauseButton.Clicked();
                isPaused = true;
                _pauseMenu.LoadContent(null,_coreAssets);
                _pauseMenu.LoadLevel();

            //prevents checking of other buttons while in pause menu
            } else if (!isPaused) {
                if(_petCareButton.CheckIfButtonWasClicked())
                {
                    SetVisiblity(false); //hides buttons to prevent them from being pressed again
                    _petCareButton.Clicked();
                    CurrentState = GameState.PetCareGame;
                    _petCareLevel.LoadContent(_petcareAssets, _coreAssets);
                    _petCareLevel.LoadLevel();
                } else if(_waldoButton.CheckIfButtonWasClicked())
                {
                    SetVisiblity(false);
                    _waldoButton.Clicked();
                    CurrentState = GameState.WaldoGame;
                    _waldoLevel.LoadContent(_waldoAssets, _coreAssets);
                    _waldoLevel.LoadLevel();
                } else if(_slidingButton.CheckIfButtonWasClicked())
                {
                    SetVisiblity(false);
                    _slidingButton.Clicked();
                    CurrentState = GameState.SlidingGame;
                    _slidingLevel.LoadContent(_slidingAssets, _coreAssets);
                    _slidingLevel.LoadLevel();
                } else if(_fishingButton.CheckIfButtonWasClicked())
                {
                    SetVisiblity(false);
                    _fishingButton.Clicked();
                    CurrentState = GameState.FishingGame;
                    _fishingLevel.LoadContent(_fishingAssets, _coreAssets);
                    _fishingLevel.LoadLevel();
                }
            }
        }
        
        if(isPaused) {
            _pauseMenu.HandleInput(gameTime);
        } else {
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
    }

    protected override void Update(GameTime gameTime)
    {
        var mousePosition = new Vector2(_mouseState.X, _mouseState.Y);
        _relativeMousePos = Vector2.Transform(mousePosition, Matrix.Invert(_displayManager._scaleMatrix));

        float elapsed = (float)gameTime.ElapsedGameTime.TotalSeconds;
        catIdle.UpdateFrame(elapsed);
        catIrritated.UpdateFrame(elapsed);
        catAttack.UpdateFrame(elapsed);

        HandleInput(gameTime);

        //if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
          //  Exit();

        if(isPaused) {
            _pauseMenu.Update(gameTime);
        }
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

        base.Update(gameTime);
    }

    protected override void Draw(GameTime gameTime)
    {
        GraphicsDevice.Clear(Color.CornflowerBlue);

        _spriteBatch.Begin(sortMode: SpriteSortMode.Deferred, samplerState: SamplerState.PointClamp, transformMatrix: _displayManager._scaleMatrix);

        switch(CurrentState) {
            case GameState.MainMenu:
                DrawMainMenuButtons();
                Rectangle pauseDestination = new Rectangle((int)pausePos.X, (int)pausePos.Y, pauseButton.CellWidth, pauseButton.CellHeight);
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
        if(!isPaused) {
            _spriteBatch.Draw(pauseButton.Texture, new Rectangle(750,10,48,48), new Rectangle(0,0,16,16), Color.White);
        }

        if(isPaused) {
            _pauseMenu.Draw(gameTime, _spriteBatch, _graphics);
        }
        
        //end drawing
        _spriteBatch.End();

        base.Draw(gameTime);
    }

    //hide or show visiblity of items in here
    private void SetVisiblity(bool isVisible) {
        _petCareButton.Visible = isVisible;
        _fishingButton.Visible = isVisible;
        _waldoButton.Visible = isVisible;
        _slidingButton.Visible = isVisible;
    }


    // Draws Button's in the current main menu's textures and hitbox
    private void DrawMainMenuButtons(){
        Rectangle sourceRectangle = new Rectangle(0, 0, _petCareButton.CellWidth, _petCareButton.CellHeight);
        Rectangle sourceRectangle1 = new Rectangle(0, 0, _waldoButton.CellWidth, _waldoButton.CellHeight);
        Rectangle sourceRectangle2 = new Rectangle(0, 0, _slidingButton.CellWidth, _slidingButton.CellHeight);
        Rectangle sourceRectangle3 = new Rectangle(0, 0, _fishingButton.CellWidth, _fishingButton.CellHeight);

        Rectangle destinationRectangle = new Rectangle((int)_petCareButtonPosition.X, (int)_petCareButtonPosition.Y, _petCareButton.CellWidth, _petCareButton.CellHeight);
        Rectangle destinationRectangle1 = new Rectangle((int)_waldoButtonPosition.X, (int)_waldoButtonPosition.Y, _waldoButton.CellWidth, _waldoButton.CellHeight);
        Rectangle destinationRectangle2 = new Rectangle((int)_slidingButtonPosition.X, (int)_slidingButtonPosition.Y, _slidingButton.CellWidth, _slidingButton.CellHeight);
        Rectangle destinationRectangle3 = new Rectangle((int)_fishingButtonPosition.X, (int)_fishingButtonPosition.Y, _fishingButton.CellWidth, _fishingButton.CellHeight);

        _spriteBatch.Draw(_petCareButton.Texture, destinationRectangle, sourceRectangle, Color.White, 0.0f, Vector2.Zero, SpriteEffects.None, 1.0f);
        _spriteBatch.Draw(_waldoButton.Texture, destinationRectangle1, sourceRectangle1, Color.White, 0.0f, Vector2.Zero, SpriteEffects.None, 1.0f);
        _spriteBatch.Draw(_slidingButton.Texture, destinationRectangle2, sourceRectangle2, Color.White, 0.0f, Vector2.Zero, SpriteEffects.None, 1.0f);
        _spriteBatch.Draw(_fishingButton.Texture, destinationRectangle3, sourceRectangle3, Color.White, 0.0f, Vector2.Zero, SpriteEffects.None, 1.0f);
    }
}