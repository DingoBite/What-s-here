using UnityEngine;
using UnityEngine.UI;

namespace _Project.Utils
{
    public class SpriteImageContainer : MonoBehaviour
    {
        [SerializeField] private Image _graphic;
        
        public void SetSprite(Sprite graphic)
        {
            if (_graphic == null || graphic == null)
            {
                gameObject.SetActive(false);
                return;
            }
            
            _graphic.sprite = graphic;
            _graphic.preserveAspect = true;
            gameObject.SetActive(true);
        }
    }
}