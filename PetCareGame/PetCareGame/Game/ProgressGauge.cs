using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace PetCareGame;

public class ProgressGauge {
    private Rectangle bounds;
    private Rectangle markerBounds;
    private Rectangle sourceGauge = new Rectangle(0,0,64,16);
    private Rectangle sourceMarker = new Rectangle(48,48,13,12);
    private Point markerPos;

    private int minValue, maxValue, currentValue;

    public ProgressGauge(Rectangle bounds, int minValue, int maxValue, int currentValue) {
        this.bounds = bounds;
        this.minValue = minValue;
        this.maxValue = maxValue;
        this.currentValue = currentValue;

        markerPos = new Point(CalculatePosition(), bounds.Y + bounds.Height/2);
        markerBounds = new Rectangle(markerPos.X, markerPos.Y, bounds.Width/4, bounds.Height);
    }
    
    public void Draw(GameTime gameTime, SpriteBatch spriteBatch) {
        spriteBatch.Draw(GameHandler.gaugeTextureAtlas, bounds, sourceGauge, Color.White);
        spriteBatch.Draw(GameHandler.gaugeTextureAtlas, markerBounds, sourceMarker, Color.White, 0f, new Vector2(6,0), SpriteEffects.None, 1f);
    }

    public void LoadContent(ContentManager _manager) {

    }

    private int CalculatePosition() {
        return (int)((double)bounds.X + (double)((currentValue - minValue)/(double)(maxValue - minValue)) * (double)bounds.Width);
    }

    public void Increment(int times) {
        currentValue += times;
    }

    public void Increment() {
        currentValue += 1;
    }

    public void Decrement(int times) {
        currentValue -= times;
    }

    public void Decrement() {
        currentValue -= 1;
    }

    public int GetValue() {
        return currentValue;
    }

    public void Update(GameTime gameTime) {
        markerPos = new Point(CalculatePosition(), markerPos.Y);
        markerBounds = new Rectangle(markerPos.X, markerPos.Y, markerBounds.Width, markerBounds.Height);
    }
}