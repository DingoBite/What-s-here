using System;
using System.Collections.Generic;
using System.Globalization;
using UnityEngine;

namespace _Project.Utils.Utils
{
    public static class MathUtils
    {
        public static Vector3 GetSphericalCoords(float pitch, float yaw, float radius)
        {
            var pitchRad = Math.PI * pitch / 180;
            var yawRad = Math.PI * yaw / 180;
            var x = radius * Math.Sin(pitchRad) * Math.Cos(yawRad);
            var y = radius * Math.Sin(pitchRad) * Math.Sin(yawRad);
            var z = radius * Math.Cos(pitchRad);
            return new Vector3((float) x, (float) z, (float) y);
        }
        public static Vector2 GetCircleCoords(float angle, float radius)
        {
            var angleRad = Math.PI * angle / 180;
            var x = radius * Math.Cos(angleRad);
            var y = radius * Math.Sin(angleRad);
            return new Vector2((float) x, (float) y);
        }
        
        public static double ConvertDegreeAngleToDouble( double degrees, double minutes, double seconds )
        {
            return degrees + (minutes/60) + (seconds/3600);
        }
        
        public static Vector2D ParseGPS(string latitude, string longitude)
        {
            return new Vector2D(ParseLatitude(latitude), ParseLongitude(longitude));
        }

        public static double ParseLatitude(string latitude)
        {
            latitude = latitude.Replace("\"", "");
            var s1 = latitude.Split('°');
            var degreesStr = s1[0];
            var degrees = double.Parse(degreesStr, NumberStyles.Any);

            var s2 = s1[1].Split('\'');
            var minutesStr = s2[0];
            var minutes = double.Parse(minutesStr, NumberStyles.Any);

            var index = s2[1].Length - 1;
            var secondsStr = s2[1].Substring(0, index);
            var seconds = double.Parse(secondsStr, NumberStyles.Any);

            var sign = s2[1][index];
            
            return ConvertDegreeAngleToDouble(degrees, minutes, seconds);
        }
        
        public static double ParseLongitude(string longitude)
        {
            longitude = longitude.Replace("\"", "");
            var s1 = longitude.Split('°');
            var degreesStr = s1[0];
            var degrees = double.Parse(degreesStr, NumberStyles.Any);

            var s2 = s1[1].Split('\'');
            var minutesStr = s2[0];
            var minutes = double.Parse(minutesStr, NumberStyles.Any);

            var index = s2[1].Length - 1;
            var secondsStr = s2[1].Substring(0, index);
            var seconds = double.Parse(secondsStr, NumberStyles.Any);

            var sign = s2[1][index];
            
            return ConvertDegreeAngleToDouble(degrees, minutes, seconds);
        }
        
        public static Matrix4x4 MatrixLerp(Matrix4x4 from, Matrix4x4 to, float t)
        {
            t = Mathf.Clamp(t, 0.0f, 1.0f);
            var newMatrix = new Matrix4x4();
            newMatrix.SetRow(0, Vector4.Lerp(from.GetRow(0), to.GetRow(0), t));
            newMatrix.SetRow(1, Vector4.Lerp(from.GetRow(1), to.GetRow(1), t));
            newMatrix.SetRow(2, Vector4.Lerp(from.GetRow(2), to.GetRow(2), t));
            newMatrix.SetRow(3, Vector4.Lerp(from.GetRow(3), to.GetRow(3), t));
            return newMatrix;
        }
        
        public static Rect RectFromCollection(IReadOnlyList<float> coords)
        {
            var x = coords[0];
            var y = coords[1];
            var w = coords[2];
            var h = coords[3];
            return new Rect(x, y, w, h);
        } 
        
        public static Rect RectFromCollection(IReadOnlyList<int> coords)
        {
            var x = coords[0];
            var y = coords[1];
            var w = coords[2] - x;
            var h = coords[3] - y;
            return new Rect(x, y, w, h);
        } 
    }
}