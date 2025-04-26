using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace PetCareGame;

public class DisplayManager
{
    private Game game;
    private GraphicsDeviceManager graphics;
    private GraphicsDevice graphicsDevice;

    // Desired Resolution
    private readonly int resolutionWidth = 800;
    private readonly int resolutionHeight = 600;

    public Matrix scaleMatrix;
    public Viewport viewport;

    public DisplayManager(Game game, GraphicsDeviceManager graphics, GraphicsDevice graphicsDevice)
    {
        this.game = game;
        this.graphics = graphics;
        this.graphicsDevice = graphicsDevice;
        SetResolution(resolutionWidth, resolutionHeight);
        UpdateScreenScaleMatrix();
    }

    // Changes the resolution of window to a specified resolution
    public void SetResolution(int width, int height)
    {
        graphics.PreferredBackBufferWidth = width;
        graphics.PreferredBackBufferHeight = height;
        graphics.ApplyChanges();
    }

    // Changes game window to fullscreen
    public void SetFullScreen()
    {
        graphicsDevice.PresentationParameters.BackBufferWidth = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Width;
        graphicsDevice.PresentationParameters.BackBufferHeight = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Height;
        graphics.IsFullScreen = true;
        graphics.ApplyChanges();
    }
    public void UpdateScreenScaleMatrix()
    {
        // Determine the size of the actual screen
        float screenWidth = graphicsDevice.PresentationParameters.BackBufferWidth;
        float screenHeight = graphicsDevice.PresentationParameters.BackBufferHeight;
        //fix window scaling before checking input
        if (screenHeight != graphicsDevice.PresentationParameters.BackBufferHeight ||
                screenWidth != graphicsDevice.PresentationParameters.BackBufferWidth)
        {
            //window size 480x800 reduces down to 3x5
            //checks that user set width is a factor of 5 or height is factor of 3
            //if this fails, it uses integer division (which rounds to nearest whole number)
            //to find other number and then multiplies 3 and 5 by that factor and sets
            //it to height and width, respectively, before updating graphics

            if (graphicsDevice.PresentationParameters.BackBufferWidth % 4 != 0)
            {
                //factor being multiplied by 5
                int factor = graphicsDevice.PresentationParameters.BackBufferWidth / 4;
                //set preferred dimensions to aspect-correct dimensions
                graphics.PreferredBackBufferWidth = factor * 4;
                graphics.PreferredBackBufferHeight = factor * 3;
                //applies changes
                graphics.ApplyChanges();
            }
            else if (graphicsDevice.PresentationParameters.BackBufferHeight % 3 != 0)
            {
                //factor being multiplied by 3
                int factor = graphicsDevice.PresentationParameters.BackBufferHeight / 3;
                //set preferred dimensions to aspect-correct dimensions
                graphics.PreferredBackBufferWidth = factor * 4;
                graphics.PreferredBackBufferHeight = factor * 3;
                //applies changes
                graphics.ApplyChanges();
            }
        }

        float scaleX = screenWidth / resolutionWidth;
        float scaleY = screenHeight / resolutionHeight;
        Vector3 scale = new(scaleX, scaleY, 1);

        scaleMatrix = Matrix.CreateScale(scale);

    }

}