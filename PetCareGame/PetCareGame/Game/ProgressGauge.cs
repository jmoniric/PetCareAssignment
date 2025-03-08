using System;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace PetCareGame;

public class ProgressGauge {
    private Rectangle bounds;
    private Rectangle markerBounds;
    //atlas source for Good/Bad type gauge
    private Rectangle sourceGaugeGB = new Rectangle(0,0,64,16);
    //atlas source for Hit In Range type gauge
    private Rectangle sourceGaugeHIR = new Rectangle(0,16,64,16);
    private Rectangle sourceMarker = new Rectangle(48,48,13,12);
    private Rectangle targetRange;
    private Point markerPos;
    private bool ascending = true;
    private bool isVisible;

    private Random rand = new Random();

    private int minValue, maxValue, currentValue;

    private GaugeType gaugeType;

    public enum GaugeType {
        GoodBad,
        HitInRange
    }

    public ProgressGauge(Rectangle bounds, int minValue, int maxValue, int currentValue, GaugeType gaugeType, bool isVisible) {
        this.bounds = bounds;
        this.minValue = minValue;
        this.maxValue = maxValue;
        this.currentValue = currentValue;
        this.isVisible = isVisible;

        this.gaugeType = gaugeType;

        markerPos = new Point(CalculatePosition(), bounds.Y + bounds.Height/2);
        markerBounds = new Rectangle(markerPos.X, markerPos.Y, bounds.Width/4, bounds.Height);

        if(gaugeType == GaugeType.HitInRange) {
            CalculateTargetRange();
        }
    }
    
    public void Draw(GameTime gameTime, SpriteBatch spriteBatch) {
        if(isVisible) {
            if(gaugeType == GaugeType.GoodBad) {
                spriteBatch.Draw(GameHandler.gaugeTextureAtlas, bounds, sourceGaugeGB, Color.White);
            } else if(gaugeType == GaugeType.HitInRange) {
                spriteBatch.Draw(GameHandler.gaugeTextureAtlas, bounds, sourceGaugeHIR, Color.White);
                spriteBatch.Draw(GameHandler.gaugeTextureAtlas, targetRange, new Rectangle(32, 48, 16, 16), Color.SeaGreen);
            }
            spriteBatch.Draw(GameHandler.gaugeTextureAtlas, markerBounds, sourceMarker, Color.White, 0f, new Vector2(6,0), SpriteEffects.None, 1f);
        }
    }

    public void LoadContent(ContentManager _manager) {

    }

    private int CalculatePosition() {
        return (int)((double)bounds.X + (double)((currentValue - minValue)/(double)(maxValue - minValue)) * (double)bounds.Width);
    }

    private void CalculateTargetRange() {
        int x = (int)rand.NextInt64(bounds.X+1, bounds.X + (bounds.Width / 2));
        int width = (int)rand.NextInt64(bounds.Width/6, bounds.Width/2);
        targetRange = new Rectangle(x, bounds.Y, width, bounds.Height);
    }

    public bool Increment(int times) {
        int i = 0;
        while (i < times) {
            //still within bounds
            if(currentValue < maxValue) {
                currentValue += 1;
                i++;
            } else { //can't add because it will go out of bounds
                return false;
            }
        }
        //all operations completed without going out of bounds
        return true;
    }

    public bool Increment() {
        if(currentValue < maxValue) {
            currentValue += 1;
            return true;
        }
        return false;
    }

    public bool Decrement(int times) {
        int i = 0;
        while (i < times) {
            //still within bounds
            if(currentValue > minValue) {
                currentValue -= 1;
                i++;
            } else { //can't subtract because it will go out of bounds
                return false;
            }
        }
        //all operations completed without going out of bounds
        return true;
    }

    public bool Decrement() {
        if(currentValue > minValue) {
            currentValue -= 1;
            return true;
        }
        return false;
    }

    public int GetValue() {
        return currentValue;
    }

    public void SetCurrentValue(int value) {
        currentValue = value;
    }
    
    public void SetVisibility(bool isVisible) {
        this.isVisible = isVisible;
    }

    public void Update(GameTime gameTime) {
        if(isVisible) {
            if(gaugeType == GaugeType.GoodBad) {
                markerPos = new Point(CalculatePosition(), markerPos.Y);
                markerBounds = new Rectangle(markerPos.X, markerPos.Y, markerBounds.Width, markerBounds.Height);

            } else if(gaugeType == GaugeType.HitInRange) {
                if(ascending) {
                    if(currentValue < maxValue) {
                        currentValue++;
                    } else if(currentValue == maxValue) {
                        ascending = false;
                        currentValue--;
                    }
                } else {
                    if(currentValue > minValue) {
                        currentValue--;
                    } else if(currentValue == minValue) {
                        ascending = true;
                        currentValue++;
                    }
                }

                markerPos = new Point(CalculatePosition(), markerPos.Y);
                markerBounds = new Rectangle(markerPos.X, markerPos.Y, markerBounds.Width, markerBounds.Height);
            }
        }
    }

    public void HandleInput() {
        
    }

    public int CheckForSuccess() {
        int returnState = 0;
        if(isVisible) {
            if(targetRange.X <= markerPos.X && markerPos.X <= (targetRange.X +  targetRange.Width)) {
                GameHandler.successSfx.Play();
                returnState = 1;
                CalculateTargetRange();
            } else {
                GameHandler.failSfx.Play();
                returnState = -1;
                CalculateTargetRange();
            }
        }
        return returnState;
    }
}