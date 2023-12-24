using UnityEngine;

namespace _Project.Utils.MonoBehaviours
{
    [RequireComponent(typeof(RectTransform))]
    public abstract class UIRequiredPropertyBehaviour<T> : RequiredPropertyBehaviour<T>
    {
        private RectTransform _rectTransform;

        public RectTransform RectTransform => _rectTransform == null ? FindRectTransform() : _rectTransform;
        
        private RectTransform FindRectTransform()
        {
            _rectTransform = GetComponent<RectTransform>();
            return _rectTransform;
        }
    }
}