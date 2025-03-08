using System;
using System.Runtime.Intrinsics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace PetCareGame;

public class Goal {
    private int targetValue;
    private int currentValue = 0;

    public Goal(int targetValue) {
        this.targetValue = targetValue;
    }

    public bool Increment() {
        currentValue++;
        if(currentValue >= targetValue) {
            return true;
        }
        return false;
    }

    public bool Increment(int value) {
        currentValue += value;
        if(currentValue >= targetValue) {
            return true;
        }
        return false;
    }

    public void ResetGoal() {
        currentValue = 0;
    }

    public int GetCurrentValue() {
        return currentValue;
    }

    public void DrawOutput(SpriteBatch spriteBatch, SpriteFont font, Vector2 pos, Color colour, string label) {
        spriteBatch.DrawString(
            font,
            label + ": " + currentValue + "/" + targetValue,
            pos,
            colour
        );
    }
}