
using UnityEngine;

namespace _Project.Utils.Extensions
{
    public static class ColorExtensions
    {
        public static Color Invert(this Color color)
        {
            return new Color(1.0f-color.r, 1.0f-color.g, 1.0f-color.b);
        }
    }
}