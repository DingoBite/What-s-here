using UnityEngine;

namespace _Project.Utils
{
    public class AlignToCameraDirection : MonoBehaviour
    {
        private void Update()
        {
            transform.rotation = Quaternion.LookRotation(Camera.main.transform.forward);
        }
    }
}