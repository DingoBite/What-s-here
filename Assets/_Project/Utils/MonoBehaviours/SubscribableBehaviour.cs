using UnityEngine;

namespace _Project.Utils.MonoBehaviours
{
    public abstract class SubscribableBehaviour : MonoBehaviour
    {
        private void Subscribe()
        {
            Unsubscribe();
            OnSubscribe();
        }

        protected abstract void OnSubscribe();

        private void Unsubscribe()
        {
            OnUnsubscribe();
        }

        protected abstract void OnUnsubscribe();

        protected virtual void OnEnable()
        {
            Subscribe();
        }

        protected virtual void OnDisable()
        {
            Unsubscribe();
        }
    }
}