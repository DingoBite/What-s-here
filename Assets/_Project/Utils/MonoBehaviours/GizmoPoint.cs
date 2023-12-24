using UnityEngine;

namespace _Project.Utils.MonoBehaviours
{
    public class GizmoPoint : MonoBehaviour
    {
        [SerializeField] private Color _color;
        [SerializeField] private float _radius = 5;

        private void OnDrawGizmos()
        {
            Gizmos.color = _color;
            Gizmos.DrawSphere(transform.position, _radius);
        }
    }
}