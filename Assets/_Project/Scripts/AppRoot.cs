using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using _Project.App;
using _Project.Utils.MonoBehaviours;
using NaughtyAttributes;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using Random = UnityEngine.Random;

namespace _Project
{
    public class AppRoot : SingletonBehaviour<AppRoot>
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
        
        public void Initialize()
        {
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
            AppController.Instance.SetText("");
            AppController.Instance.CloseInfo();
            AppController.Instance.SetCheck(-1);
            AppController.Instance.InfoOpened = true;
            _textRecognizer.OnRecognized += TextRecognized;
        }

        [Button]
        public void DisableRecognition()
        {
            _textRecognizer.OnRecognized -= TextRecognized;
        }
        
        public void RandomTextRecognizer()
        {
            var keys = _modelSwitcher.Keys.ToList();
            var key = keys[Random.Range(0, keys.Count / 2) * 2 + 1];
            _modelSwitcher.EnableTransform(key);
            var weightedScore = 70 + Random.value * 30;
            AppController.Instance.SetText(key);
            if (weightedScore < 77)
            {
                AppController.Instance.SetCheck(0);
            }
            else if (weightedScore < 88)
            {
                AppController.Instance.SetCheck(1);
            }
            else if (weightedScore >= 88)
            {
                AppController.Instance.SetCheck(2);
            }

            var tr = _modelSwitcher.transform;
            var cTr = _camera.transform;
            tr.position = cTr.position + cTr.forward * 0.5f + Vector3.down * 0.1f;
        }
        
        public void TextRecognized(string text)
        {
            var (score, key) = _nameResolver.SearchMaxKey(text, _modelSwitcher.Keys);
            if (key == null)
                return;
            var (angleScoreF, minPlane) = _arPatternsManager.GetPrevPatternAngleDistance();
            var angleScore = (int) (100 * angleScoreF);
            var weightedScore = 0;
            
            weightedScore = (int)((1 - _angleWeight) * score + angleScore * _angleWeight);
            
            Debug.Log($"Text recognize: Try to add {weightedScore} : {score} : {angleScore}");
            if (weightedScore < 0)
                return;
            if (weightedScore <= _record.score)
                return;

            AppController.Instance.SetText(key);
            if (weightedScore < 77)
            {
                AppController.Instance.SetCheck(0);
            }
            else if (weightedScore < 88)
            {
                AppController.Instance.SetCheck(1);
            }
            else if (weightedScore >= 88)
            {
                AppController.Instance.SetCheck(2);
            }
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
