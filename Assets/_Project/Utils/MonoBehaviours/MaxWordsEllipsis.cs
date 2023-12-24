using System.Text;
using TMPro;
using UnityEngine;

namespace _Project.Utils.MonoBehaviours
{
    public class MaxWordsEllipsis : MonoBehaviour
    {
        [SerializeField] public int TakeSymbolsCount;
        [SerializeField] private TMP_Text _text;

        private readonly StringBuilder _stringBuilder = new();
        
        public void SetText(string text)
        {
            gameObject.SetActive(text != null);
            if (text == null)
                return;
            if (text.Length < TakeSymbolsCount)
            {
                _text.text = text;
                return;
            }

            _stringBuilder.Clear();
            var i = 0;
            foreach (var word in text.Split(" "))
            {
                i += word.Length + 1;
                _stringBuilder.Append(word);
                if (i > TakeSymbolsCount)
                    break;
                _stringBuilder.Append(" ");
            }
            _text.text = _stringBuilder + "...";
        }
    }
}