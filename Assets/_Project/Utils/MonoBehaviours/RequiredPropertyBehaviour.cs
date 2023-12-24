using UnityEngine;

namespace _Project.Utils.MonoBehaviours
{
    public class RequiredPropertyBehaviour<T> : MonoBehaviour
    {
        public class Protected : MonoBehaviour
        {
            private T _component;
            protected T Component => _component ??= GetComponent<T>();
        }
        
        private T _component;
        public T Component => _component ??= GetComponent<T>();
    }
    
    public class RequiredPropertyBehaviour<T1, T2> : MonoBehaviour
    {
        public class Protected : MonoBehaviour
        {
            private T1 _component1;
            protected T1 Component1 => _component1 ??= GetComponent<T1>();
            
            private T2 _component2;
            protected T2 Component2 => _component2 ??= GetComponent<T2>();
        }
        
        private T1 _component1;
        public T1 Component1 => _component1 ??= GetComponent<T1>();
        
        private T2 _component2;
        public T2 Component2 => _component2 ??= GetComponent<T2>();
    }
}