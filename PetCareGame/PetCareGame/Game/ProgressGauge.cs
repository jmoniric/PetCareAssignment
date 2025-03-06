using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace PetCareGame;

public class ProgressGauge {
    private Texture2D atlasTexture;
    private Rectangle bounds;
    private Rectangle sourceGauge = new Rectangle(0,16,64,16);
    private Rectangle sourceMarker = new Rectangle(48,0,13,12);
    private Texture2D gauge;
    private Texture2D marker;
    private Point markerPos;

    private int minValue, maxValue, currentValue;

    public ProgressGauge(Rectangle bounds, int minValue, int maxValue, int currentValue) {
        this.bounds = bounds;
        this.minValue = minValue;
        this.maxValue = maxValue;
        this.currentValue = currentValue;

        markerPos = new Point(999, bounds.Y + bounds.Height/2);
    }
    
    public void Draw(GameTime gameTime, SpriteBatch spriteBatch) {
        spriteBatch.Draw(GameHandler.coreTextureAtlas, bounds, sourceGauge, Color.White);
    }

    public void LoadContent(ContentManager _manager) {

    }
}