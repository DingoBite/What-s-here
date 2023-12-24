using System;
using System.Collections;
using System.Collections.Generic;
using NaughtyAttributes;
using UnityEngine;
using UnityEngine.XR.ARFoundation;

namespace _Project
{
    public class AppRoot : MonoBehaviour
    {
        [SerializeField] private Camera _camera;
        [SerializeField] private TextRecognizer _textRecognizer;
        [SerializeField] private TransformSwitcher _modelSwitcher;
        [SerializeField] private NameResolver _nameResolver;
        [SerializeField] private ARPatternsManager _arPatternsManager;
        [SerializeField] private int _scoreToStopRecognize;
        
        [Range(0, 1)]
        [SerializeField] private float _angleWeight = 0.5f;
        
        private (int score, string key, ARPlane plane) _record;
        
        private IEnumerator Start()
        {
            yield return new WaitForEndOfFrame();
            _arPatternsManager.Initialize(_camera);
            _textRecognizer.Initialize();
            _textRecognizer.SetCamera(_camera);
            _arPatternsManager.OnTrackingTransformSwitchers += OnTracking;
            EnableRecognition();
        }

        private void OnTracking(IEnumerable<(TrackableState state, TransformSwitcher trackable)> trackables)
        {
            if (_record.key == null)
                return;
            
            foreach (var (state, trackable) in trackables)
            {
                var key = _record.key;
                Debug.Log($"Tracked {key} : {state}");
                switch (state)
                {
                    case TrackableState.Added:
                        trackable.EnableTransform(key);
                        break;
                    case TrackableState.Removed:
                        trackable.DisableAll();
                        break;
                    case TrackableState.Updated:
                        trackable.EnableTransform(key);
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
        }

        [Button]
        public void EnableRecognition()
        {
            _record = (0, null, null);
            DisableRecognition();
            _textRecognizer.OnRecognized += TextRecognized;
        }

        [Button]
        public void DisableRecognition()
        {
            _textRecognizer.OnRecognized -= TextRecognized;
        }
        
        private void TextRecognized(string text)
        {
            var (score, key) = _nameResolver.SearchMaxKey(text, _modelSwitcher.Keys);
            if (key == null)
                return;
            var (angleScoreF, minPlane) = _arPatternsManager.GetPrevPatternAngleDistance();
            var angleScore = (int) (100 * angleScoreF);
            var weightedScore = 0;
            
            if (angleScore >= 0)
                weightedScore = (int)((1 - _angleWeight) * score + angleScore * _angleWeight);
            
            Debug.Log($"Text recognize: Try to add {weightedScore} : {score} : {angleScore}");
            if (weightedScore <= _record.score || weightedScore < 0)
                return;
            
            _record = (weightedScore, key, minPlane);
            var pattern = _textRecognizer.CollectBiggestBoxPattern();
            if (pattern != null)
            {
                pattern.name = key;
                _arPatternsManager.AddNewPattern(pattern);

                if (weightedScore > _scoreToStopRecognize)
                    DisableRecognition();
            }
        }
    }
}
