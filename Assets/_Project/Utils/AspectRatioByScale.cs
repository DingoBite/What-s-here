using _Project.Utils.MonoBehaviours;
using UnityEngine;

namespace _Project.Utils
{
    public class AspectRatioByScale : UIBehaviour
    {
        [SerializeField] private int _size;

        public void Update()
        {
            var w = UnityEngine.Screen.height;
            var scaleFactor = (float) w / _size;
            print(w);
            print(scaleFactor);
            var newScale = RectTransform.localScale;
            newScale = scaleFactor * Vector3.one;
            RectTransform.localScale = newScale;
        }
    }
}