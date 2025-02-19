using Microsoft.Xna.Framework;
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
        FoodGame
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
    private PetCare _petCareLevel = new PetCare();
    private CatFishing _fishingLevel = new CatFishing();
    private WheresWaldo _waldoLevel = new WheresWaldo();
    private FoodGame _foodLevel = new FoodGame();

    

    public GameHandler()
    {
        _graphics = new GraphicsDeviceManager(this);
        Content.RootDirectory = "Content";
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
        _mouseLeftPressed = false;

        base.Initialize();
    }

    protected override void LoadContent()
    {
        _spriteBatch = new SpriteBatch(GraphicsDevice);
        // TODO: use this.Content to load your game content here
        _petCareButton = new Button(Content.Load<Texture2D>("Sprites/Buttons/PetCareMiniGame"), Content.Load<Texture2D>("Sprites/Buttons/PetCareMiniGameClicked"), 
                                            new Point(64, 33), _petCareButtonPosition, "Pet Care Minigame", 33, true);
        _waldoButton = new Button(Content.Load<Texture2D>("Sprites/Buttons/WaldoMiniGame"), Content.Load<Texture2D>("Sprites/Buttons/WaldoMiniGameClicked"),
                                            new Point(64, 33), _waldoButtonPosition, "Where's Waldo Minigame", 34, true);
        _slidingButton = new Button(Content.Load<Texture2D>("Sprites/Buttons/SlideMiniGame"), Content.Load<Texture2D>("Sprites/Buttons/SlideMiniGameClicked"),
                                            new Point(64, 33), _slidingButtonPosition, "Sliding Minigame", 35, true);
        _fishingButton = new Button(Content.Load<Texture2D>("Sprites/Buttons/FishingMiniGame"), Content.Load<Texture2D>("Sprites/Buttons/FishingMiniGameClicked"),
                                            new Point(64, 33), _fishingButtonPosition, "Fishing Minigame", 36, true);

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
            case GameState.FoodGame:
                _foodLevel.HandleInput(gameTime);
                break;   
        }
    }

    protected override void Update(GameTime gameTime)
    {
        HandleInput(gameTime);
        _petCareButton.UpdateButton();
        _waldoButton.UpdateButton();
        _slidingButton.UpdateButton();
        _fishingButton.UpdateButton();

        

        if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
            Exit();

        if(_mouseLeftPressed)
        {
            _mouseLeftPressed = false;
            if(CheckIfButtonWasClicked(_petCareButton))
            {
                _petCareButton.Clicked();
                CurrentState = GameState.PetCareGame;
            }
            if(CheckIfButtonWasClicked(_waldoButton))
            {
                _waldoButton.Clicked();
                CurrentState = GameState.WaldoGame;
                _petCareLevel.LoadContent(_spriteBatch);
                _petCareLevel.LoadLevel();
            }
            if(CheckIfButtonWasClicked(_slidingButton))
            {
                _slidingButton.Clicked();
                CurrentState = GameState.FoodGame;
            }
            if(CheckIfButtonWasClicked(_fishingButton))
            {
                _fishingButton.Clicked();
                CurrentState = GameState.FishingGame;
            }
        }

        switch(CurrentState) {
            case GameState.MainMenu:
                //handle the input for main menu directly here
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
            case GameState.FoodGame:
                _foodLevel.Update(gameTime);
                break;   
        }

        

        base.Update(gameTime);
    }

    protected override void Draw(GameTime gameTime)
    {
        GraphicsDevice.Clear(Color.CornflowerBlue);

        // TODO: Add your drawing code here
        Rectangle sourceRectangle = new Rectangle(0, 0, _petCareButton.CellWidth, _petCareButton.CellHeight);
        Rectangle sourceRectangle1 = new Rectangle(0, 0, _waldoButton.CellWidth, _waldoButton.CellHeight);
        Rectangle sourceRectangle2 = new Rectangle(0, 0, _slidingButton.CellWidth, _slidingButton.CellHeight);
        Rectangle sourceRectangle3 = new Rectangle(0, 0, _fishingButton.CellWidth, _fishingButton.CellHeight);
        
        Rectangle destinationRectangle = new Rectangle((int)_petCareButtonPosition.X, (int)_petCareButtonPosition.Y, _petCareButton.CellWidth, _petCareButton.CellHeight);
        Rectangle destinationRectangle1 = new Rectangle((int)_waldoButtonPosition.X, (int)_waldoButtonPosition.Y, _waldoButton.CellWidth, _waldoButton.CellHeight);
        Rectangle destinationRectangle2 = new Rectangle((int)_slidingButtonPosition.X, (int)_slidingButtonPosition.Y, _slidingButton.CellWidth, _slidingButton.CellHeight);
        Rectangle destinationRectangle3 = new Rectangle((int)_fishingButtonPosition.X, (int)_fishingButtonPosition.Y, _fishingButton.CellWidth, _fishingButton.CellHeight);
        
        _spriteBatch.Begin(SpriteSortMode.FrontToBack);
        _spriteBatch.Draw(_petCareButton.Texture, destinationRectangle, sourceRectangle, Color.White, 0.0f, Vector2.Zero, SpriteEffects.None, 1.0f);
        _spriteBatch.Draw(_waldoButton.Texture, destinationRectangle1, sourceRectangle1, Color.White, 0.0f, Vector2.Zero, SpriteEffects.None, 1.0f);
        _spriteBatch.Draw(_slidingButton.Texture, destinationRectangle2, sourceRectangle2, Color.White, 0.0f, Vector2.Zero, SpriteEffects.None, 1.0f);
        _spriteBatch.Draw(_fishingButton.Texture, destinationRectangle3, sourceRectangle3, Color.White, 0.0f, Vector2.Zero, SpriteEffects.None, 1.0f);
        switch(CurrentState) {
            case GameState.MainMenu:
                //handle the input for main menu directly here
                break;
            case GameState.PetCareGame:
                _petCareLevel.Draw(gameTime, _spriteBatch);
                break;
            case GameState.FishingGame:
                _fishingLevel.Draw(gameTime, _spriteBatch);
                break;
            case GameState.WaldoGame:
                _waldoLevel.Draw(gameTime, _spriteBatch);
                break;
            case GameState.FoodGame:
                _foodLevel.Draw(gameTime, _spriteBatch);
                break;   
        }
        _spriteBatch.End();
        
        base.Draw(gameTime);
    }

    private bool CheckIfButtonWasClicked(Button button)
    {
        if(_oneShotMouseState.X >= button.Position.X && _oneShotMouseState.X <= (button.Position.X + button.Dimensions.X))
        {
            if(_oneShotMouseState.Y >= button.Position.Y && _oneShotMouseState.Y <= (button.Position.Y + button.Dimensions.Y) && button.Visible)
            {
                return true;
            }
        }
        return false;
    }
}
