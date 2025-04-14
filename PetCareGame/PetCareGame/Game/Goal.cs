using System;
using System.Runtime.Intrinsics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace PetCareGame;

public class Goal {
    private int targetValue;
    private int currentValue = 0;
    private bool isComplete = false;

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
            isComplete = true;
            return true;
        }
        return false;
    }

    public void ResetGoal() {
        currentValue = 0;
        isComplete = false;
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

    public bool GetCompletion() {
        return isComplete;
    }

    public void SetCompletion(bool state) {
        isComplete = state;
    }
}