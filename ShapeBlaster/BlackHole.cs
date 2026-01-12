using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace ShapeBlaster;

class BlackHole : Entity
{
    private int hitpoints = 10;
    private float sprayAngle = 0;

    public BlackHole(Vector2 position)
    {
        image = Art.BlackHole;
        Position = position;
        Radius = image.Width / 2f;
    }

    public override void Update()
    {
        var entities = EntityManager.GetNearbyEntities(Position, 250);
        foreach (var entity in entities)
        {
            if (entity is Enemy enemy && !enemy.IsActive)
            {
                continue;
            }
            // bullets are repelled by black holes and everything else is attracted 
            if (entity is Bullet)
            {
                entity.Velocity += (entity.Position - Position).ScaleTo(0.3f);
            }
            else
            {
                var dPos = Position - entity.Position;
                var length = dPos.Length();

                entity.Velocity += dPos.ScaleTo(MathHelper.Lerp(2, 0, length / 250f));
            }
        }

        // The black holes spray some orbiting particles. The spray toggles on and off every quarter second. 
        if ((GameRoot.GameTime.TotalGameTime.Milliseconds / 250) % 2 == 0)
        {
            var sprayVel = MathUtil.FromPolar(sprayAngle, Random.Shared.NextFloat(12, 15));
            var color = ColorUtil.HSVToColor(5, 0.5f, 0.8f);  // light purple 
            var pos = Position + 2f * new Vector2(sprayVel.Y, -sprayVel.X) + Random.Shared.NextVector2(4, 8);
            var state = new ParticleState
            {
                Velocity = sprayVel,
                LengthMultiplier = 1,
                Type = ParticleType.Enemy
            };

            GameRoot.ParticleManager.CreateParticle(Art.LineParticle, pos, color, 190, 1.5f, state);
        }

        // rotate the spray direction 
        sprayAngle -= MathHelper.TwoPi / 50f;
    }

    public void WasShot()
    {
        hitpoints--;
        if (hitpoints <= 0)
        {
            IsExpired = true;
        }

        var hue = (float)((3 * GameRoot.GameTime.TotalGameTime.TotalSeconds) % 6);
        var color = ColorUtil.HSVToColor(hue, 0.25f, 1);
        const int numParticles = 150;
        var startOffset = Random.Shared.NextFloat(0, MathHelper.TwoPi / numParticles);

        for (var i = 0; i < numParticles; i++)
        {
            var sprayVel = MathUtil.FromPolar(MathHelper.TwoPi * i / numParticles + startOffset, Random.Shared.NextFloat(8, 16));
            var pos = Position + 2f * sprayVel;
            var state = new ParticleState()
            {
                Velocity = sprayVel,
                LengthMultiplier = 1,
                Type = ParticleType.IgnoreGravity
            };

            GameRoot.ParticleManager.CreateParticle(Art.LineParticle, pos, color, 90, 1.5f, state);
        }

        Sound.Explosion.Play(0.5f, Random.Shared.NextFloat(-0.2f, 0.2f), 0);
    }

    public void Kill()
    {
        hitpoints = 0;
        WasShot();
    }

    public override void Draw(SpriteBatch spriteBatch)
    {
        // make the size of the black hole pulsate 
        var scale = 1 + 0.1f * (float)Math.Sin(10 * GameRoot.GameTime.TotalGameTime.TotalSeconds);
        spriteBatch.Draw(image, Position, null, color, Orientation, Size / 2f, scale, 0, 0);
    }
}

