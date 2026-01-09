using Microsoft.Xna.Framework;

namespace ShapeBlaster;

public static class MathUtil
{
    public static Vector2 FromPolar(float angle, float magnitude) =>
        magnitude * new Vector2(float.Cos(angle), float.Sin(angle));
}
