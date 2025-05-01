/** 
@author: Nolan
*/
using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace PetCareGame;

public class Particle {
    private Point pos;
    private Point dimensions;
    private float velMult;
    private Texture2D texture;

    public Particle(Point pos, Point dimensions, float velMult, Texture2D texture) {
        this.pos = pos;
        this.dimensions = dimensions;
        this.velMult = velMult;
        this.texture = texture;
    }

    public void Draw(GameTime gameTime, SpriteBatch spriteBatch, GraphicsDeviceManager _graphics, Color colour)
    {
        Rectangle bounds = new Rectangle(pos, dimensions);
        spriteBatch.Draw(texture, bounds, new Rectangle(Point.Zero, dimensions), colour, 0f, Vector2.Zero, SpriteEffects.None, 1f);
    }

    //updates and calculates position change necessary
    public void Update(GameTime gameTime) {
        pos.X += (int)(1 * velMult);
    }

    //returns 1 if particle is out of bounds
    //returns 2 if particle collides with specified target
    //returns 0 if it fails prev conditions
    public int CheckToDestroy(Rectangle target) {
        if(pos.X > 800) {
            return 1;
        } else if(target.Contains(pos)) {
            return 2;
        }
        return 0;
    }
}