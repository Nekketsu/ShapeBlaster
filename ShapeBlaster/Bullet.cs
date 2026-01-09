using Microsoft.Xna.Framework;

namespace ShapeBlaster;

class Bullet : Entity
{
    public Bullet(Vector2 position, Vector2 velocity)
    {
        image = Art.Bullet;
        Position = position;
        Velocity = velocity;
        Orientation = Velocity.Angle;
        Radius = 8;
    }

    public override void Update()
    {
        if (Velocity.LengthSquared() > 0)
        {
            Orientation = Velocity.Angle;
        }

        Position += Velocity;

        // delete bullets that go off-screen 
        if (!GameRoot.Viewport.Bounds.Contains(Position.ToPoint()))
        {
            IsExpired = true;
        }
    }
}