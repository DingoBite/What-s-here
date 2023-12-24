using System.Collections;
using _Project.Utils.MonoBehaviours;
using UnityEngine;
using UnityEngine.UI;

namespace _Project.Utils
{
    [RequireComponent(typeof(HorizontalOrVerticalLayoutGroup))]
    public class LayoutRebuildProvider : UIRequiredPropertyBehaviour<HorizontalOrVerticalLayoutGroup>
    {
        public void Rebuild()
        {
            if (gameObject.activeSelf)
                LayoutRebuilder.ForceRebuildLayoutImmediate(RectTransform);
        }
        
        public void RebuildOnNextFrame()
        {
            if (gameObject.activeSelf)
                CoroutineParent.Instance.StartCoroutine(Rebuild_C());
        }

        public void RebuildOnNextFrameAndDisable()
        {
            if (gameObject.activeSelf)
                CoroutineParent.Instance.StartCoroutine(RebuildAndDisable_C());
        }
        
        private IEnumerator Rebuild_C()
        {
            yield return new WaitForEndOfFrame();
            Rebuild();
        }
        
        private IEnumerator RebuildAndDisable_C()
        {
            yield return new WaitForEndOfFrame();
            if (TryGetComponent<ContentSizeFitter>(out var sizeFitter))
            {
                sizeFitter.enabled = false;
            }
            yield return new WaitForEndOfFrame();
            Rebuild();
            if (TryGetComponent<HorizontalOrVerticalLayoutGroup>(out var layout))
            {
                layout.enabled = false;
            }
        }
    }
}