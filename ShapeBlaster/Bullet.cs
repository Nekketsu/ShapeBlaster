using Microsoft.Xna.Framework;
using System;

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

        // delete bullets that go off-screen 
        if (!GameRoot.Viewport.Bounds.Contains(Position.ToPoint()))
        {
            IsExpired = true;

            for (var i = 0; i < 30; i++)
            {
                GameRoot.ParticleManager.CreateParticle(Art.LineParticle, Position, Color.LightBlue, 50, 1,
                    new ParticleState() { Velocity = Random.Shared.NextVector2(0, 9), Type = ParticleType.Bullet, LengthMultiplier = 1 });
            }
        }
    }
}