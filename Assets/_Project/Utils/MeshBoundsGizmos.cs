using _Project.Utils.MonoBehaviours;
using UnityEngine;

namespace _Project.Utils
{
    [RequireComponent(typeof(MeshFilter))]
    public class MeshBoundsGizmos : RequiredPropertyBehaviour<MeshRenderer>
    {
        private void OnDrawGizmos()
        {
            Gizmos.color = Color.blue;
            var boundsCenter = Component.bounds.center;
            var boundsSize = Component.bounds.size;
            Gizmos.DrawWireCube(boundsCenter, boundsSize);
        }
    }
}