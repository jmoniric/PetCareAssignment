using System.Net.Http.Headers;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

// This class should make sure that it doesn't register mmultiple buttons clicks
namespace PetCareGame
{
    public class OneShotMouseButtons
    {
        static MouseState currentMouseState;
        static MouseState previousMouseState;

        public OneShotMouseButtons()
        {

        }

        public static MouseState GetState()
        {
            previousMouseState = currentMouseState;
            currentMouseState = Mouse.GetState();
            return currentMouseState;
        }

        public static bool IsPressed(bool left)
        {
            if (left)
            {
                return currentMouseState.LeftButton == ButtonState.Pressed;
            }
            else
            {
                return currentMouseState.RightButton == ButtonState.Pressed;
            }
        }

        public static bool HasNotBeenPressed(bool left)
        {
            if (left)
            {
                return currentMouseState.LeftButton == ButtonState.Pressed && !(previousMouseState.LeftButton == ButtonState.Pressed);
            }
            else
            {
                return currentMouseState.RightButton == ButtonState.Pressed && !(previousMouseState.LeftButton == ButtonState.Pressed);
            }
        }
    }
}