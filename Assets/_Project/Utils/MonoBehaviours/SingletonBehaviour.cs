using _Project.Utils.Inspector;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace _Project.Utils.MonoBehaviours
{
    public class SingletonBehaviour<T> : RequiredPropertyBehaviour<T>.Protected where T : SingletonBehaviour<T>
    {
        private static T _instance;
        public static T Instance
        {
            get
            {
                if (!Application.isPlaying)
                {
                    foreach (var rootGameObject in SceneManager.GetActiveScene().GetRootGameObjects())
                    {
                        var component = rootGameObject.transform.FindComponent<T>();
                        if (component != null)
                            return component;
                    }
                }
                return _instance;
            }
            private set => _instance = value;
        }

        private void Awake()
        {
            Setup();
        }

        protected virtual void Setup()
        {
            if (Instance != null)
            {
                Debug.LogWarning("Multiple singletons. Can be incorrect behaviour");
                Destroy(gameObject);
            }
            else
            {
                Instance = Component;
            }
        }
    }
}