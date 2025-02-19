using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace PetCareGame
{
    public class Button
    {
        Texture2D _staticTexture;
        Texture2D _clickedTexture;

        public string Name{ get; set; }
        public int ID { get; set; }
        public int AnimationTime { get; set; }
        public bool Visible { get; set; }
        public Point Dimensions { get; set; }
        public Vector2 Position { get; set; }
        public Texture2D Texture { get; set; }
        public int CellWidth { get; set; }
        public int CellHeight { get; set; }

        public Button(Texture2D staticImage, Texture2D clickedImage, Point dimensions, Vector2 position, string name, int id, bool visible)
        {
            _staticTexture = staticImage;
            _clickedTexture = clickedImage;

            Texture = _staticTexture;
            CellWidth = dimensions.X;
            CellHeight = dimensions.Y;
            Visible = visible;
            Name = name;
            Position = position;
            Dimensions = new Point(dimensions.X, dimensions.Y);
            ID = id;

            AnimationTime = 0;
        }

        public void Clicked(){
            AnimationTime = 5;
            Texture = _clickedTexture;
        }

        public void UpdateButton(){
            if(AnimationTime > 0){
                AnimationTime--;
            }
            if (AnimationTime == 0)
            {
                Texture = _staticTexture;
            }
        }
    }
}