namespace _Project.Utils.MonoBehaviours
{
    public abstract class SubscribableRequiredPropertyBehaviour<T> : RequiredPropertyBehaviour<T>
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

        private void OnEnable()
        {
            Subscribe();
        }
        
        private void OnDisable()
        {
            Unsubscribe();
        }
    }

    public abstract class SubscribableRequiredPropertyBehaviour<T1, T2> : RequiredPropertyBehaviour<T1, T2>
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

        private void OnEnable()
        {
            Subscribe();
        }
        
        private void OnDisable()
        {
            Unsubscribe();
        }
    }
}