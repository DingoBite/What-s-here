using System;
using NaughtyAttributes;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace _Project
{
    public class TextRecognizer : MonoBehaviour
    {
        public event Action<string> OnRecognized;

        [SerializeField] private bool _isFrameBaseRecognize;
        [ShowIf(nameof(_isFrameBaseRecognize))]
        [SerializeField] private int _framesForRecognize;

        [HideIf(nameof(_isFrameBaseRecognize))] 
        [SerializeField] private float _secondsForRecognize;

        [SerializeField] private RawImage _debugTexture;
        [SerializeField] private RenderTexture _renderTexture;
        [SerializeField] private LayerMask _layerMask;

        [SerializeField] private bool _drawOutline;
        
        private TesseractDriver _tesseractDriver;

        private int _frames;
        private float _seconds;
        
        private bool _initialized;
        private Camera _camera;

        public void Initialize()
        {
            _tesseractDriver = new TesseractDriver();
            _tesseractDriver.Setup(() => _initialized = true);
        }

        public void SetCamera(Camera c)
        {
            _camera = c;
            if (_debugTexture != null)
                _debugTexture.texture = _renderTexture;
        }
        
        public string ForceRecognizeWithoutInvoke(Texture2D texture2D)
        {
            return _tesseractDriver.Recognize(texture2D);
        }

        public Texture2D CollectBiggestBoxPattern()
        {
            return _tesseractDriver.CollectBiggestBoxPattern();
        }
        
        private void Recognize()
        {
            var prevCullingMask = _camera.cullingMask;
            _camera.cullingMask = _layerMask;
            _camera.targetTexture = _renderTexture;
            _camera.Render();
            var res = _tesseractDriver.Recognize(_renderTexture, _drawOutline);
            OnRecognized?.Invoke(res);
            if (_debugTexture != null)
                _debugTexture.texture = _tesseractDriver.GetHighlightedTexture();
            _camera.targetTexture = null;
            _camera.cullingMask = prevCullingMask;
        }
        
        private void Update()
        {
            if (_renderTexture == null || !_initialized || 
                _isFrameBaseRecognize && _framesForRecognize <= 0 ||
                !_isFrameBaseRecognize && _secondsForRecognize <= 0)
                return;
            if (_isFrameBaseRecognize)
                FrameBaseRecognize();
            else 
                SecondsBaseRecognize();
        }

        private void FrameBaseRecognize()
        {
            if (_frames > 0)
            {
                _frames--;
            }
            else
            {
                _frames = _framesForRecognize;
                Recognize();
            }
        }

        private void SecondsBaseRecognize()
        {
            if (_seconds > 0)
            {
                _seconds -= Time.deltaTime;
            }
            else
            {
                _seconds = _secondsForRecognize;
                Recognize();
            }
        }
    }
}
