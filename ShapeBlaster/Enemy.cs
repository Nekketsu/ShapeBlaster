using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;

namespace ShapeBlaster;

class Enemy : Entity
{
    private List<IEnumerator<int>> behaviours = [];
    private int timeUntilStart = 60;
    public bool IsActive => timeUntilStart <= 0;
    public int PointValue { get; private set; }

    public Enemy(Texture2D image, Vector2 position)
    {
        this.image = image;
        Position = position;
        Radius = image.Width / 2f;
        color = Color.Transparent;
        PointValue = 1;
    }

    public static Enemy CreateSeeker(Vector2 position)
    {
        var enemy = new Enemy(Art.Seeker, position);
        enemy.AddBehaviour(enemy.FollowPlayer());
        enemy.PointValue = 2;

        return enemy;
    }

    public static Enemy CreateWanderer(Vector2 position)
    {
        var enemy = new Enemy(Art.Wanderer, position);
        enemy.AddBehaviour(enemy.MoveRandomly());
        return enemy;
    }

    public override void Update()
    {
        if (timeUntilStart <= 0)
        {
            ApplyBehaviours();
        }
        else
        {
            timeUntilStart--;
            color = Color.White * (1 - timeUntilStart / 60f);
        }

        Position += Velocity;
        Position = Vector2.Clamp(Position, Size / 2, GameRoot.ScreenSize - Size / 2);
        Velocity *= 0.8f;
    }

    private void AddBehaviour(IEnumerable<int> behaviour)
    {
        behaviours.Add(behaviour.GetEnumerator());
    }

    private void ApplyBehaviours()
    {
        for (var i = 0; i < behaviours.Count; i++)
        {
            if (!behaviours[i].MoveNext())
            {
                behaviours.RemoveAt(i--);
            }
        }
    }

    public void HandleCollision(Enemy other)
    {
        var d = Position - other.Position;
        Velocity += 10 * d / (d.LengthSquared() + 1);
    }

    public void WasShot()
    {
        IsExpired = true;
        PlayerStatus.AddPoints(PointValue);
        PlayerStatus.IncreaseMultiplier();

        var hue1 = Random.Shared.NextFloat(0, 6);
        var hue2 = (hue1 + Random.Shared.NextFloat(0, 2)) % 6f;
        var color1 = ColorUtil.HSVToColor(hue1, 0.5f, 1);
        var color2 = ColorUtil.HSVToColor(hue2, 0.5f, 1);

        for (var i = 0; i < 120; i++)
        {
            var speed = 18f * (1f - 1 / Random.Shared.NextFloat(1f, 10f));
            var state = new ParticleState()
            {
                Velocity = Random.Shared.NextVector2(speed, speed),
                Type = ParticleType.Enemy,
                LengthMultiplier = 1f
            };

            var color = Color.Lerp(color1, color2, Random.Shared.NextFloat(0, 1));
            GameRoot.ParticleManager.CreateParticle(Art.LineParticle, Position, color, 190, 1.5f, state);
        }

        Sound.Explosion.Play(0.5f, Random.Shared.NextFloat(-0.2f, 0.2f), 0);
    }

    IEnumerable<int> FollowPlayer(float acceleration = 1f)
    {
        while (true)
        {
            Velocity += (PlayerShip.Instance.Position - Position).ScaleTo(acceleration);
            if (Velocity != Vector2.Zero)
            {
                Orientation = Velocity.Angle;
            }

            yield return 0;
        }
    }

    IEnumerable<int> MoveInASquare()
    {
        const int framesPerSide = 30;
        while (true)
        {
            // move right for 30 frames 
            for (int i = 0; i < framesPerSide; i++)
            {
                Velocity = Vector2.UnitX;
                yield return 0;
            }

            // move down 
            for (int i = 0; i < framesPerSide; i++)
            {
                Velocity = Vector2.UnitY;
                yield return 0;
            }

            // move left 
            for (int i = 0; i < framesPerSide; i++)
            {
                Velocity = -Vector2.UnitX;
                yield return 0;
            }

            // move up 
            for (int i = 0; i < framesPerSide; i++)
            {
                Velocity = -Vector2.UnitY;
                yield return 0;
            }
        }
    }

    IEnumerable<int> MoveRandomly()
    {
        var direction = Random.Shared.NextFloat(0, MathHelper.TwoPi);

        while (true)
        {
            direction += Random.Shared.NextFloat(-0.1f, 0.1f);
            direction = MathHelper.WrapAngle(direction);

            for (var i = 0; i < 6; i++)
            {
                Velocity += MathUtil.FromPolar(direction, 0.4f);
                Orientation -= 0.05f;

                var bounds = GameRoot.Viewport.Bounds;
                bounds.Inflate(-image.Width, -image.Height);

                // if the enemy is outside the bounds, make it move away from the edge 
                if (!bounds.Contains(Position.ToPoint()))
                {
                    direction = (GameRoot.ScreenSize / 2 - Position).Angle + Random.Shared.NextFloat(-MathHelper.PiOver2, MathHelper.PiOver2);
                }

                yield return 0;
            }
        }
    }
}