using UnityEngine;

namespace _Project.Utils.Extensions
{
    public static class CameraExtensions
    {
        public static bool IsObjectVisible(this Camera c, Renderer renderer)
        {
            return GeometryUtility.TestPlanesAABB(GeometryUtility.CalculateFrustumPlanes(c), renderer.bounds);
        }
    }
}