using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace ShapeBlaster;

public static class Extensions
{
    extension(SpriteBatch spriteBatch)
    {
        public void DrawLine(Vector2 start, Vector2 end, Color color, float thickness = 2f)
        {
            Vector2 delta = end - start;
            spriteBatch.Draw(Art.Pixel, start, null, color, delta.Angle, new Vector2(0, 0.5f), new Vector2(delta.Length(), thickness), SpriteEffects.None, 0f);
        }
    }

    extension(Vector2 vector)
    {
        public float Angle =>
            float.Atan2(vector.Y, vector.X);

        public Vector2 ScaleTo(float length) =>
            vector * (length / vector.Length());
    }

    extension(Random random)
    {
        public float NextFloat(float minValue, float maxValue) =>
            (float)(random.NextDouble() * (maxValue - minValue) + minValue);

        public Vector2 NextVector2(float minLength, float maxLength)
        {
            double theta = random.NextDouble() * 2 * Math.PI;
            float length = random.NextFloat(minLength, maxLength);
            return new Vector2(length * (float)Math.Cos(theta), length * (float)Math.Sin(theta));
        }
    }
}