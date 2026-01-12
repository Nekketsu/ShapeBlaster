using Microsoft.Xna.Framework;
using System;

namespace ShapeBlaster;

static class ColorUtil
{
    public static Color HSVToColor(float h, float s, float v)
    {
        if (h == 0 && s == 0)
        {
            return new Color(v, v, v);
        }

        var c = s * v;
        var x = c * (1 - Math.Abs(h % 2 - 1));
        var m = v - c;

        return h switch
        {
            < 1 => new Color(c + m, x + m, m),
            < 2 => new Color(x + m, c + m, m),
            < 3 => new Color(m, c + m, x + m),
            < 4 => new Color(m, x + m, c + m),
            < 5 => new Color(x + m, m, c + m),
            _ => new Color(c + m, m, x + m)
        };
    }
}