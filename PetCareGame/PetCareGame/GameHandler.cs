using System;
using System.Diagnostics;
using System.Net.Mime;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Input.Touch;
using System.Text.Json;
using System.IO;

namespace PetCareGame;

public class GameHandler : Game
{
    public enum GameState
    {
        MainMenu,
        Overworld,
        PetCareGame,
        WaldoGame,
        FishingGame,
        SlidingGame
    }
    public static GameState CurrentState = GameState.MainMenu;

    private GraphicsDeviceManager _graphics;
    private SpriteBatch _spriteBatch;
    private DisplayManager _displayManager;
    public SaveFile saveFile;

    // private Button _petCareButton;
    // private Vector2 _petCareButtonPosition;
    // private Button _waldoButton;
    // private Vector2 _waldoButtonPosition;
    // private Button _slidingButton;
    // private Vector2 _slidingButtonPosition;
    // private Button _fishingButton;
    // private Vector2 _fishingButtonPosition;
    private Button pauseButton;
    private Vector2 pausePos;

    public static MouseState _mouseState;
    public static bool _mouseLeftPressed;

    private static PetCare _petCareLevel = new PetCare();
    private static CatFishing _fishingLevel = new CatFishing();
    private static WheresWaldo _waldoLevel = new WheresWaldo();
    private static SlidingGame _slidingLevel = new SlidingGame();
    public static Overworld _overworldLevel;
    private static PauseMenu _pauseMenu = new PauseMenu();
    private static MainMenuScreen _mainMenu = new MainMenuScreen();
    public static ContentManager _coreAssets;
    public static ContentManager _slidingAssets;
    public static ContentManager _waldoAssets;
    public static ContentManager _fishingAssets;
    public static ContentManager _petcareAssets;
    
    public static bool isPaused = false;

    public static int windowHeight = 600;
    public static int windowWidth = 800;
    public static Vector2 _relativeMousePos;
    public static AnimatedTexture catIdle = new AnimatedTexture(new Vector2(32,16), 0f, 3f, 0.5f);
    public static AnimatedTexture catIrritated = new AnimatedTexture(new Vector2(32,16), 0f, 3f, 0.5f);
    public static AnimatedTexture catAttack = new AnimatedTexture(new Vector2(32,16), 0f, 3f, 0.5f);
    public static AnimatedTexture catWalk = new AnimatedTexture(new Vector2(32,16), 0f, 3f, 0.5f);
    public static AnimatedTexture catRun = new AnimatedTexture(new Vector2(32,16), 0f, 3f, 0.5f);
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
    public static bool muted = false;

    
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

        saveFile = new SaveFile();

        _mouseState = OneShotMouseButtons.GetState();
        pausePos = new Vector2(750,10);
        _mouseLeftPressed = false;

        base.Initialize();
    }

    protected override void LoadContent()
    {
        _spriteBatch = new SpriteBatch(GraphicsDevice);
        // TODO: use this.Content to load your game content here

        //Core assets
        catIdle.Load(_coreAssets, "Sprites/Animal/idle", 7, 5);
        catIrritated.Load(_coreAssets, "Sprites/Animal/irritated", 4, 6);
        catAttack.Load(_coreAssets, "Sprites/Animal/attack", 3, 4);
        catWalk.Load(_coreAssets, "Sprites/Animal/walk", 7, 15);
        catRun.Load(_coreAssets, "Sprites/Animal/run", 7, 15);

        //tries to load audio assets; if device is missing audio drivers,
        //marks global bool _allowAudio as false which prevents game from
        //trying to call on audio that it can't handle or doesn't exist
        try { 
            catPurr = _coreAssets.Load<SoundEffect>("Sounds/Animal/cat_purr");
            selectSfx = _coreAssets.Load<SoundEffect>("Sounds/UI/select").CreateInstance();
            selectSfx.Volume = 0.2f;
            failSfx = _coreAssets.Load<SoundEffect>("Sounds/UI/fail").CreateInstance();
            failSfx.Volume = 0.4f;
            successSfx = _coreAssets.Load<SoundEffect>("Sounds/UI/success").CreateInstance();
            successSfx.Volume = 0.2f;
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

        _overworldLevel = new Overworld(_petCareLevel, _waldoLevel, _slidingLevel, _pauseMenu, pauseButton);

    }

    private void HandleInput(GameTime gameTime){
        _mouseState = OneShotMouseButtons.GetState();

        if (_mouseState.LeftButton == ButtonState.Pressed)
        {
            if (OneShotMouseButtons.HasNotBeenPressed(true))
            {
                _mouseLeftPressed = true;
            }
            else if (pauseButton.CheckIfSelectButtonWasClicked() && !isPaused)
            {
                pauseButton.Clicked();
                isPaused = true;
                _pauseMenu.LoadLevel();
                _pauseMenu.LoadContent(null, _coreAssets);
            }
        }

        if (isPaused)
        {
            _pauseMenu.HandleInput(gameTime);
        }
        else
        {
            switch (CurrentState)
            {
                case GameState.PetCareGame:
                    _petCareLevel.HandleInput(gameTime);
                    break;
                case GameState.WaldoGame:
                    _waldoLevel.HandleInput(gameTime);
                    break;
                case GameState.SlidingGame:
                    _slidingLevel.HandleInput(gameTime);
                    break;
                case GameState.FishingGame:
                    _fishingLevel.HandleInput(gameTime);
                    break;
            }
        }
    }

    protected override void Update(GameTime gameTime)
    {
        var mousePosition = new Vector2(_mouseState.X, _mouseState.Y);
        _relativeMousePos = Vector2.Transform(mousePosition, Matrix.Invert(_displayManager._scaleMatrix));
        
        if(!isPaused) {
            float elapsed = (float)gameTime.ElapsedGameTime.TotalSeconds;
            catIdle.UpdateFrame(elapsed);
            catIrritated.UpdateFrame(elapsed);
            catAttack.UpdateFrame(elapsed);
            catWalk.UpdateFrame(elapsed);
            catRun.UpdateFrame(elapsed);
        }

        HandleInput(gameTime);

        //if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
        //  Exit();

        if (isPaused) {
            _pauseMenu.Update(gameTime);
        }
        switch(CurrentState) {
            case GameState.MainMenu:
                //handle the update for main menu directly here
                _mainMenu.LoadLevel();
                _mainMenu.Update(gameTime, this);
                //SetVisiblity(true);
                break;
            case GameState.Overworld:
                _overworldLevel.Update(gameTime);
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
                _mainMenu.Draw(gameTime, _spriteBatch, _graphics);
                break;
            case GameState.Overworld:
                //DrawMainMenuButtons();
                _overworldLevel.Draw(gameTime, _spriteBatch, _graphics);
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
        if(!isPaused && CurrentState != GameState.MainMenu) {
            _spriteBatch.Draw(pauseButton.Texture, new Rectangle(750,10,48,48), new Rectangle(0,0,16,16), Color.White);
        }

        if(isPaused) {
            _pauseMenu.Draw(gameTime, _spriteBatch, _graphics);
        }

        //FOR DEV PURPOSES: prints (X,Y) of mouse in top left corner
        _spriteBatch.DrawString(highPixel22, "("+ _relativeMousePos.X + ", " + _relativeMousePos.Y + ")", new Vector2(0, 20), Color.Black);
        
        //end drawing
        _spriteBatch.End();

        base.Draw(gameTime);
    }

    //allows level to cleanup and reset, then unloads its assets and sets state to main menu
    public static void UnloadCurrentLevel() {
        switch(CurrentState) {
            case GameState.PetCareGame:
                _petCareLevel.CleanupProcesses();
                ((LevelInterface)_petCareLevel).UnloadLevel(_petcareAssets);
                break;
            case GameState.WaldoGame:
                _waldoLevel.CleanupProcesses();
                ((LevelInterface)_waldoLevel).UnloadLevel(_waldoAssets);
                break;
            case GameState.FishingGame:
                _fishingLevel.CleanupProcesses();
                ((LevelInterface)_fishingLevel).UnloadLevel(_fishingAssets);
                break;
            case GameState.SlidingGame:
                _slidingLevel.CleanupProcesses();
                ((LevelInterface)_slidingLevel).UnloadLevel(_slidingAssets);
                break;
            default:
                break;
        }
        CurrentState = GameState.MainMenu;
    }

    public void Quit()
    {
        this.Exit();
    }
}