using UnityEngine;

namespace _Project.Utils
{
    public class ScreenConfigure : MonoBehaviour
    {
        [SerializeField] private int _frameRate = 90;
        
        private void Start()
        {
            Application.targetFrameRate = _frameRate;
        }
    }
}