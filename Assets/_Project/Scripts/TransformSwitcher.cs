using System.Collections.Generic;
using AYellowpaper.SerializedCollections;
using UnityEngine;

namespace _Project
{
    public class TransformSwitcher : MonoBehaviour
    {
        [SerializeField] private SerializedDictionary<string, Transform> _transforms;

        public IEnumerable<string> Keys => _transforms.Keys;

        public void DisableAll()
        {
            foreach (var value in _transforms.Values)
            {
                value.gameObject.SetActive(false);
            }
        }
        
        public void EnableTransform(string key)
        {
            DisableAll();
            
            if (!_transforms.TryGetValue(key, out var tr))
            {
                Debug.LogError($"Cannot find transform with name {key}");
                return;
            }

            tr.gameObject.SetActive(true);
        }
    }
}
