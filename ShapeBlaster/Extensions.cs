using Microsoft.Xna.Framework;
using System;

namespace ShapeBlaster;

public static class Extensions
{
    extension(Vector2 vector)
    {
        public float Angle =>
            float.Atan2(vector.Y, vector.X);
    }

    extension(Random random)
    {
        public float NextFloat(float minValue, float maxValue) =>
            (float)(random.NextDouble() * (maxValue - minValue) + minValue);
    }
}
