using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace PetCareGame;

public class PetCare : Game
{
    enum GameState
    {
        MainMenu,
        Playing
    }
    GameState CurrentState = GameState.MainMenu;

    private GraphicsDeviceManager _graphics;
    private SpriteBatch _spriteBatch;
    private Button _quarantineButton;
    private Vector2 _quarantineButtonPosition;
    private MouseState _oneShotMouseState;
    private bool _mouseLeftPressed;
    

    public PetCare()
    {
        _graphics = new GraphicsDeviceManager(this);
        Content.RootDirectory = "Content";
        IsMouseVisible = true;
        
    }

    protected override void Initialize()
    {
        // TODO: Add your initialization logic here
        _quarantineButtonPosition = new Vector2(100, 100);
        _oneShotMouseState = OneShotMouseButtons.GetState();
        _mouseLeftPressed = false;

        base.Initialize();
    }

    protected override void LoadContent()
    {
        _spriteBatch = new SpriteBatch(GraphicsDevice);

        // TODO: use this.Content to load your game content here
        _quarantineButton = new Button(Content.Load<Texture2D>("Sprites/Buttons/Sprite-0001"), Content.Load<Texture2D>("Sprites/Buttons/Sprite-0005"), 
                                            new Point(64, 33), _quarantineButtonPosition, "Button1", 33, true);
        
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
    }

    protected override void Update(GameTime gameTime)
    {
        HandleInput(gameTime);
        _quarantineButton.UpdateButton();

        if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
            Exit();

        // TODO: Add your update logic here

        if(_mouseLeftPressed)
        {
            _mouseLeftPressed = false;
            if(CheckIfButtonWasClicked())
            {
                _quarantineButton.Clicked();
            }
        }

        base.Update(gameTime);
    }

    protected override void Draw(GameTime gameTime)
    {
        GraphicsDevice.Clear(Color.CornflowerBlue);

        // TODO: Add your drawing code here
        Rectangle sourceRectangle = new Rectangle(0, 0, _quarantineButton.CellWidth, _quarantineButton.CellHeight);
        Rectangle destinationRectangle = new Rectangle((int)_quarantineButtonPosition.X, (int)_quarantineButtonPosition.Y, _quarantineButton.CellWidth, _quarantineButton.CellHeight);

        _spriteBatch.Begin(SpriteSortMode.FrontToBack);
        _spriteBatch.Draw(_quarantineButton.Texture, destinationRectangle, sourceRectangle, Color.White, 0.0f, Vector2.Zero, SpriteEffects.None, 1.0f);
        _spriteBatch.End();
        
        base.Draw(gameTime);
    }

    private bool CheckIfButtonWasClicked()
    {
        if(_oneShotMouseState.X >= _quarantineButton.Position.X && _oneShotMouseState.X <= (_quarantineButton.Position.Y + _quarantineButton.Dimensions.Y) && _quarantineButton.Visible)
        {
            return true;
        }
        return false;
    }
}
