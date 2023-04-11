using UnityEngine;

public static class ColorHelper
{
    /// <summary>
    /// Converts a color to a float array in the format {r, g, b, a}.
    /// </summary>
    /// <param name="color">The color to convert.</param>
    /// <returns>The float array representation of the color.</returns>
    public static float[] ColorToFloat(Color color)
    {
        float[] result = new float[4];

        result[0] = color.r;
        result[1] = color.g;
        result[2] = color.b;
        result[3] = color.a;
        
        return result;
    }
}