using System;
using UnityEngine;

namespace _Project.Utils
{

    [Serializable]
    public struct Vector2D
    {
        private const double ReverseHalf = 1d / 180;
        private const double ReversePI = 1 / Math.PI;
        
        public double x;
        public double y;
        
        public Vector2D(double x, double y)
        {
            this.x = x;
            this.y = y;
        }

        public static Vector2D operator -(Vector2D v1, Vector2D v2)
        {
            var result = new Vector2D(v1.x - v2.x, v1.y - v2.y);
            return result;
        }
        
        public static Vector2D operator +(Vector2D v1, Vector2D v2)
        {
            var result = new Vector2D(v1.x + v2.x, v1.y + v2.y);
            return result;
        }
        
        public static Vector2D operator *(Vector2D v1, float scalar)
        {
            var result = new Vector2D(v1.x * scalar, v1.y * scalar);
            return result;
        }
        
        public static Vector2D operator *(Vector2D v1, double scalar)
        {
            var result = new Vector2D(v1.x * scalar, v1.y * scalar);
            return result;
        }
        
        public static Vector2D operator /(Vector2D v1, float scalar)
        {
            var result = new Vector2D(v1.x / scalar, v1.y / scalar);
            return result;
        }
        
        public static Vector2D operator /(Vector2D v1, double scalar)
        {
            var result = new Vector2D(v1.x / scalar, v1.y / scalar);
            return result;
        }

        public void ToRad()
        {
            x = (float)(x * ReverseHalf * Math.PI);
            y = (float)(y * ReverseHalf * Math.PI);
        }

        public void FromRad()
        {
            x = (float)(x * 180 * ReversePI);
            y = (float)(y * 180 * ReversePI);
        }

        public static Vector2D Zero => new(0, 0);
        
        public static implicit operator Vector2(Vector2D v) => new((float)v.x, (float)v.y);
        public static explicit operator Vector2D(Vector2 v) => new(v.x, v.y);
        
        public void Deconstruct(out double longitude, out double latitude)
        {
            longitude = y;
            latitude = x;
        }

        public override string ToString()
        {
            return $"{x}, {y}";
        }
    }
}