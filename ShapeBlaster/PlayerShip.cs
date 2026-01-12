using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace ShapeBlaster;

class PlayerShip : Entity
{
    private static PlayerShip instance;
    public static PlayerShip Instance
    {
        get
        {
            instance ??= new PlayerShip();
            return instance;
        }
    }

    const int cooldownFrames = 6;
    int cooldownRemaining = 0;

    int framesUntilRespawn = 0;
    public bool IsDead => framesUntilRespawn > 0;

    private PlayerShip()
    {
        image = Art.Player;
        Position = GameRoot.ScreenSize / 2;
        Radius = 10;
    }

    public override void Update()
    {
        if (IsDead)
        {
            if (--framesUntilRespawn == 0)
            {
                if (PlayerStatus.Lives == 0)
                {
                    PlayerStatus.Reset();
                    Position = GameRoot.ScreenSize / 2;
                }
            }

            return;
        }

        var aim = Input.GetAimDirection();
        if (aim.LengthSquared() > 0 && cooldownRemaining <= 0)
        {
            cooldownRemaining = cooldownFrames;
            var aimAngle = aim.Angle;
            var aimQuat = Quaternion.CreateFromYawPitchRoll(0, 0, aimAngle);

            var randomSpread = Random.Shared.NextFloat(-0.04f, 0.04f) + Random.Shared.NextFloat(-0.04f, 0.04f);
            var vel = MathUtil.FromPolar(aimAngle + randomSpread, 11f);

            var offset = Vector2.Transform(new Vector2(25, -8), aimQuat);
            EntityManager.Add(new Bullet(Position + offset, vel));

            offset = Vector2.Transform(new Vector2(25, 8), aimQuat);
            EntityManager.Add(new Bullet(Position + offset, vel));

            Sound.Shot.Play(0.2f, Random.Shared.NextFloat(-0.2f, 0.2f), 0);
        }

        if (cooldownRemaining > 0)
        {
            cooldownRemaining--;
        }

        const float speed = 8;
        Velocity += speed * Input.GetMovementDirection();
        Position += Velocity;
        Position = Vector2.Clamp(Position, Size / 2, GameRoot.ScreenSize - Size / 2);

        if (Velocity.LengthSquared() > 0)
        {
            Orientation = Velocity.Angle;
        }

        Velocity = Vector2.Zero;
    }

    public override void Draw(SpriteBatch spriteBatch)
    {
        if (!IsDead)
        {
            base.Draw(spriteBatch);
        }
    }

    public void Kill()
    {
        PlayerStatus.RemoveLife();
        framesUntilRespawn = PlayerStatus.IsGameOver ? 300 : 120;
    }
}