using System.Collections.Generic;
using System.Text.RegularExpressions;
using FuzzySharp;
using UnityEngine;

namespace _Project
{
    public class NameResolver : MonoBehaviour
    {
        [SerializeField] private int _lowerScore;
        
        private readonly Regex _normalizationPattern = new(@"[а-я\s]+");

        private string Preprocess(string text)
        {
            return text.ToLowerInvariant();
        }

        private string Normalize(string text)
        {
            var matches = _normalizationPattern.Matches(text);
            if (matches.Count == 0)
                return null;
            if (matches.Count == 1)
                return matches[0].Value;
            var res = "";
            foreach (Match match in matches)
            {
                if (match.Value.Length > 2)
                    res += $"{match.Value} ";
                else
                    res += match.Value;
            }

            return res.TrimEnd();
        }

        public (int score, string key) SearchMaxKey(string origin, IEnumerable<string> keys)
        {
            origin = Preprocess(origin);
            origin = Normalize(origin);
            
            
            var maxFuzz = int.MinValue;
            string maxKey = null;
            if (origin == null)
                return (maxFuzz, maxKey);
            
            foreach (var key in keys)
            {
                var keyNormalized = Preprocess(key);
                keyNormalized = Normalize(keyNormalized);
                if (keyNormalized == null)
                {
                    Debug.LogError($"Key: {key} cannot be normalized");
                    continue;
                }
                var fuzzValue = Fuzz.Ratio(keyNormalized, origin);
                if (fuzzValue > maxFuzz)
                {
                    maxFuzz = fuzzValue;
                    maxKey = key;
                }
            }

            return maxFuzz < _lowerScore ? (int.MinValue, null) : (maxFuzz, maxKey);
        }
    }
}