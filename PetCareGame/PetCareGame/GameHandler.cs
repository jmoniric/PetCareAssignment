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

    private GraphicsDeviceManager graphics;
    private SpriteBatch spriteBatch;
    public static SaveFile saveFile;

    private Button pauseButton;

    private Vector2 pausePos;
    public static Vector2 relativeMousePos;
    public static Vector2 baseScreenSize = new Vector2(800, 600);

    public static MouseState mouseState;

    public static PetCare petCareLevel = new PetCare();
    public static CatFishing fishingLevel = new CatFishing();
    public static WheresWaldo waldoLevel = new WheresWaldo();
    public static SlidingGame slidingLevel = new SlidingGame();
    public static Overworld overworldLevel;
    private static PauseMenu pauseMenu = new PauseMenu();
    private static MainMenuScreen mainMenu = new MainMenuScreen();
    public static DisplayManager displayManager;

    public static ContentManager coreAssets;
    public static ContentManager slidingAssets;
    public static ContentManager waldoAssets;
    public static ContentManager fishingAssets;
    public static ContentManager petcareAssets;
    public static ContentManager overworldAssets;

    public static AnimatedTexture catIdle = new AnimatedTexture(new Vector2(32, 16), 0f, 3f, 0.5f);
    public static AnimatedTexture catIrritated = new AnimatedTexture(new Vector2(32, 16), 0f, 3f, 0.5f);
    public static AnimatedTexture catAttack = new AnimatedTexture(new Vector2(32, 16), 0f, 3f, 0.5f);
    public static AnimatedTexture catWalk = new AnimatedTexture(new Vector2(32, 16), 0f, 3f, 0.5f);
    public static AnimatedTexture catRun = new AnimatedTexture(new Vector2(32, 16), 0f, 3f, 0.5f);

    public static SoundEffect catPurr;
    public static SoundEffectInstance selectSfx;
    public static SoundEffectInstance failSfx;
    public static SoundEffectInstance successSfx;
    public static SoundEffectInstance bigWin;

    public static Texture2D coreTextureAtlas;
    public static Texture2D plainWhiteTexture;
    public static Texture2D gaugeTextureAtlas;

    // fonts
    public static SpriteFont courierNew36;
    public static SpriteFont courierNew52;
    public static SpriteFont highPixel18;
    public static SpriteFont highPixel22;
    public static SpriteFont highPixel36;

    private bool isResizing;
    public static bool allowAudio = true;
    public static bool muted = false;
    public static bool musicMuted = false;
    public static bool isPaused = false;
    public static bool mouseLeftPressed;

    public static int windowHeight = 600;
    public static int windowWidth = 800;

    public GameHandler()
    {
        graphics = new GraphicsDeviceManager(this);

        //rather than using the static methods from the content class, we should make separate content managers for separate sets of assets
        coreAssets = new ContentManager(Content.ServiceProvider);
        coreAssets.RootDirectory = "Content/Core";

        slidingAssets = new ContentManager(Content.ServiceProvider);
        slidingAssets.RootDirectory = "Content/SlidingGame";

        waldoAssets = new ContentManager(Content.ServiceProvider);
        waldoAssets.RootDirectory = "Content/WaldoGame";

        fishingAssets = new ContentManager(Content.ServiceProvider);
        fishingAssets.RootDirectory = "Content/FishingGame";

        petcareAssets = new ContentManager(Content.ServiceProvider);
        petcareAssets.RootDirectory = "Content/PetcareGame";

        overworldAssets = new ContentManager(Content.ServiceProvider);
        overworldAssets.RootDirectory = "Content/Overworld";

        IsMouseVisible = true;

        Window.AllowUserResizing = true;
        Window.ClientSizeChanged += OnClientSizeChange;
    }

    private void OnClientSizeChange(object sender, EventArgs e)
    {
        if (!isResizing && Window.ClientBounds.Width > 0 && Window.ClientBounds.Height > 0)
        {
            isResizing = true;
            displayManager.UpdateScreenScaleMatrix();
            isResizing = false;
        }
    }

    protected override void Initialize()
    {
        // TODO: Add your initialization logic here
        displayManager = new(this, graphics, GraphicsDevice);
        displayManager.UpdateScreenScaleMatrix();

        saveFile = new SaveFile();

        mouseState = OneShotMouseButtons.GetState();
        pausePos = new Vector2(750, 10);
        mouseLeftPressed = false;

        base.Initialize();
    }

    protected override void LoadContent()
    {
        spriteBatch = new SpriteBatch(GraphicsDevice);
        // TODO: use this.Content to load your game content here

        //Core assets
        catIdle.Load(coreAssets, "Sprites/Animal/idle", 7, 5);
        catIrritated.Load(coreAssets, "Sprites/Animal/irritated", 4, 6);
        catAttack.Load(coreAssets, "Sprites/Animal/attack", 3, 4);
        catWalk.Load(coreAssets, "Sprites/Animal/walk", 7, 15);
        catRun.Load(coreAssets, "Sprites/Animal/run", 7, 15);

        coreTextureAtlas = coreAssets.Load<Texture2D>("Sprites/core_textureatlas");
        gaugeTextureAtlas = coreAssets.Load<Texture2D>("Sprites/gauge_atlas");
        plainWhiteTexture = coreAssets.Load<Texture2D>("Sprites/plain_white");

        //fonts
        courierNew36 = coreAssets.Load<SpriteFont>("Fonts/courier_new_36");
        courierNew52 = coreAssets.Load<SpriteFont>("Fonts/courier_new_52");
        highPixel18 = coreAssets.Load<SpriteFont>("Fonts/high_pixel_18");
        highPixel22 = coreAssets.Load<SpriteFont>("Fonts/high_pixel_22");
        highPixel36 = coreAssets.Load<SpriteFont>("Fonts/high_pixel_36");

        

        //tries to load audio assets; if device is missing audio drivers,
        //marks global bool _allowAudio as false which prevents game from
        //trying to call on audio that it can't handle or doesn't exist
        try
        {
            catPurr = coreAssets.Load<SoundEffect>("Sounds/Animal/cat_purr");
            selectSfx = coreAssets.Load<SoundEffect>("Sounds/UI/select").CreateInstance();
            selectSfx.Volume = 0.2f;
            failSfx = coreAssets.Load<SoundEffect>("Sounds/UI/fail").CreateInstance();
            failSfx.Volume = 0.4f;
            successSfx = coreAssets.Load<SoundEffect>("Sounds/UI/success").CreateInstance();
            successSfx.Volume = 0.2f;
            bigWin = coreAssets.Load<SoundEffect>("Sounds/UI/big_win").CreateInstance();
            bigWin.Volume = 0.15f;
        }
        catch (NoAudioHardwareException e)
        {
            allowAudio = false;
            Console.WriteLine("No audio drivers found, disabling audio");
            Console.WriteLine(e.StackTrace);
        }

        pauseButton = new Button(coreTextureAtlas, coreTextureAtlas, new Point(48, 48), pausePos, "Pause", 37, true);

        overworldLevel = new Overworld(petCareLevel, waldoLevel, slidingLevel, pauseMenu, pauseButton);

    }

    private void HandleInput(GameTime gameTime)
    {
        mouseState = OneShotMouseButtons.GetState();

        if(Keyboard.GetState().IsKeyDown(Keys.Escape) && !isPaused && CurrentState != GameState.MainMenu) {
            isPaused = true;
            pauseMenu.LoadLevel();
            pauseMenu.LoadContent(null, coreAssets);
        }

        if (mouseState.LeftButton == ButtonState.Pressed)
        {
            if (OneShotMouseButtons.HasNotBeenPressed(true))
            {
                mouseLeftPressed = true;
            }
            else if (pauseButton.CheckIfSelectButtonWasClicked() && !isPaused && CurrentState != GameState.MainMenu)
            {
                pauseButton.Clicked();
                isPaused = true;
                pauseMenu.LoadLevel();
                pauseMenu.LoadContent(null, coreAssets);
            }
        }

        if (isPaused)
        {
            pauseMenu.HandleInput(gameTime, this);
        }
        else
        {
            switch (CurrentState)
            {
                case GameState.PetCareGame:
                    petCareLevel.HandleInput(gameTime);
                    break;
                case GameState.WaldoGame:
                    waldoLevel.HandleInput(gameTime);
                    break;
                case GameState.SlidingGame:
                    slidingLevel.HandleInput(gameTime);
                    break;
                case GameState.FishingGame:
                    fishingLevel.HandleInput(gameTime);
                    break;
            }
        }
    }

    protected override void Update(GameTime gameTime)
    {
        var mousePosition = new Vector2(mouseState.X, mouseState.Y);
        relativeMousePos = Vector2.Transform(mousePosition, Matrix.Invert(displayManager.scaleMatrix));

        if (!isPaused)
        {
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

        if (isPaused)
        {
            pauseMenu.Update(gameTime, this);
        }
        switch (CurrentState)
        {
            case GameState.MainMenu:
                //handle the update for main menu directly here
                mainMenu.LoadLevel();
                mainMenu.Update(gameTime, this);
                break;
            case GameState.Overworld:
                overworldLevel.Update(gameTime);
                break;
            case GameState.PetCareGame:
                petCareLevel.Update(gameTime);
                break;
            case GameState.FishingGame:
                fishingLevel.Update(gameTime);
                break;
            case GameState.WaldoGame:
                waldoLevel.Update(gameTime);
                break;
            case GameState.SlidingGame:
                slidingLevel.Update(gameTime);
                break;
        }

        base.Update(gameTime);
    }

    protected override void Draw(GameTime gameTime)
    {
        GraphicsDevice.Clear(Color.CornflowerBlue);

        spriteBatch.Begin(sortMode: SpriteSortMode.Deferred, samplerState: SamplerState.PointClamp, transformMatrix: displayManager.scaleMatrix);
        //prevents cluttering of text
        highPixel18.LineSpacing = 28;
        highPixel22.LineSpacing = 35;

        switch (CurrentState)
        {
            case GameState.MainMenu:
                mainMenu.Draw(gameTime, spriteBatch, graphics);
                break;
            case GameState.Overworld:
                overworldLevel.Draw(gameTime, spriteBatch, graphics);
                break;
            case GameState.PetCareGame:
                petCareLevel.Draw(gameTime, spriteBatch, graphics);
                break;
            case GameState.FishingGame:
                fishingLevel.Draw(gameTime, spriteBatch, graphics);
                break;
            case GameState.WaldoGame:
                waldoLevel.Draw(gameTime, spriteBatch, graphics);
                break;
            case GameState.SlidingGame:
                slidingLevel.Draw(gameTime, spriteBatch, graphics);
                break;
        }

        //draw pause button last
        if (!isPaused && CurrentState != GameState.MainMenu)
        {
            spriteBatch.Draw(pauseButton.Texture, new Rectangle(750, 10, 48, 48), new Rectangle(0, 0, 16, 16), Color.White);
        }

        if (isPaused)
        {
            pauseMenu.Draw(gameTime, spriteBatch, graphics);
        }

        //FOR DEV PURPOSES: prints (X,Y) of mouse in top left corner
        spriteBatch.DrawString(highPixel22, "(" + relativeMousePos.X + ", " + relativeMousePos.Y + ")", new Vector2(0, 20), Color.Black);

        //end drawing
        spriteBatch.End();

        base.Draw(gameTime);
    }

    //allows level to cleanup and reset, then unloads its assets and sets state to main menu
    public static void UnloadCurrentLevel()
    {
        switch (CurrentState)
        {
            case GameState.PetCareGame:
                petCareLevel.CleanupProcesses();
                ((LevelInterface)petCareLevel).UnloadLevel(petcareAssets);
                break;
            case GameState.WaldoGame:
                waldoLevel.CleanupProcesses();
                ((LevelInterface)waldoLevel).UnloadLevel(waldoAssets);
                break;
            case GameState.FishingGame:
                fishingLevel.CleanupProcesses();
                ((LevelInterface)fishingLevel).UnloadLevel(fishingAssets);
                break;
            case GameState.SlidingGame:
                slidingLevel.CleanupProcesses();
                ((LevelInterface)slidingLevel).UnloadLevel(slidingAssets);
                break;
            case GameState.Overworld:
                ((LevelInterface)overworldLevel).UnloadLevel(overworldAssets);
                break;
            default:
                break;
        }
        CurrentState = GameState.MainMenu;
    }

    public static void LoadOverworld() {
        CurrentState = GameState.Overworld;
        overworldLevel.LoadContent(overworldAssets, coreAssets);
        overworldLevel.LoadLevel();
    }

    public void Quit()
    {
        this.Exit();
    }
}