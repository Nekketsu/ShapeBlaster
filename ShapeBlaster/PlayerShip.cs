using Microsoft.Xna.Framework;
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

    private PlayerShip()
    {
        image = Art.Player;
        Position = GameRoot.ScreenSize / 2;
        Radius = 10;
    }

    public override void Update()
    {
        const float speed = 8;
        Velocity = speed * Input.GetMovementDirection();
        Position += Velocity;
        Position = Vector2.Clamp(Position, Size / 2, GameRoot.ScreenSize - Size / 2);

        if (Velocity.LengthSquared() > 0)
        {
            Orientation = Velocity.Angle;
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
        }

        if (cooldownRemaining > 0)
        {
            cooldownRemaining--;
        }
    }
}