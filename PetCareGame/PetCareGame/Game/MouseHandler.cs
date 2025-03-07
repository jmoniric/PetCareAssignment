using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace PetCareGame;

public class MouseHandler {
    private bool mouseDown = false;

    public MouseHandler() {

    }

    public bool GetMouseDown() {
        return mouseDown;
    }

    public void CheckForRelease() {
        if(GameHandler._mouseState.LeftButton == ButtonState.Released) {
            mouseDown = false;
        }
    }

    public bool CheckLeftInput() {
        return GameHandler._mouseState.LeftButton == ButtonState.Pressed && !mouseDown;
    }

    public void SetMouseDown() {
        mouseDown = true;
    }
}