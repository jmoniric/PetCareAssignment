using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace PetCareGame;

public class Particle {
    private Point pos;
    private Point target;
    private int width;
    private int height;
    private float velocity;
    private Vector2 direction;
    private Texture2D texture;

    private float rot;

    public Particle(int posX, int posY, int targetX, int targetY, int width, int height, float velocity, Texture2D texture) {
        pos = new Point(posX, posY);
        target = new Point(targetX, targetY);
        this.width = width;
        this.height = height;
        this.velocity = velocity;
        this.texture = texture;
    }

    public void Draw(GameTime gameTime, SpriteBatch spriteBatch, GraphicsDeviceManager _graphics, Color colour)
    {
        Rectangle bounds = new Rectangle(pos, new Point(width, height));
        spriteBatch.Draw(texture, bounds, new Rectangle(0, 0, width, height), colour, rot, Vector2.Zero, SpriteEffects.None, 1f);
    }

    //updates and calculates position change necessary
    public void Update(GameTime gameTime) {
        //calculate rotation so it aims at sprite
        rot = PointAtSprite(pos.X, pos.Y, target.X, target.Y);

        //calculates the velocity direction and normalizes
        direction = new Vector2((float)Math.Cos(rot), (float)Math.Sin(rot));
        direction.Normalize();
        direction = new Vector2(direction.X * velocity, direction.Y * velocity);

        //applies changes to particle position
        pos += new Point((int)direction.X, (int)direction.Y);
    }

    //returns true if the particle's origin is within the margins surrounding the original target coords
    public bool CheckToDestroy(int margin) {
        return (target.X - margin <= pos.X) && (pos.X <= target.X + margin)
        && (target.Y - margin <= pos.Y) && (pos.Y <= target.Y + margin);
    }

    //returns true if the particle's origin is within the margins surrounding the supplied target coords
    public bool CheckToDestroy(int margin, int targetX, int targetY) {
        return (targetX - margin <= pos.X) && (pos.X <= targetX + margin)
        && (targetY - margin <= pos.Y) && (pos.Y <= targetY + margin);
    }

    //used both by Particle and accessible as utility script, makes one sprite point at another
    public static float PointAtSprite(int pointX, int pointY, int targetX, int targetY) {
        return (float)Math.Atan2(targetY - pointY, targetX - pointX);
    }
}