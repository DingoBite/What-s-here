using System;
using System.Collections;
using System.Collections.Generic;
using _Project.Utils.MonoBehaviours;
using AYellowpaper.SerializedCollections;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace _Project.App
{
    public class AppController : SingletonBehaviour<AppController>
    {
        [SerializeField] private CanvasGroup _splash;
        [SerializeField] private CanvasGroup _playScreen;
        [SerializeField] private CanvasGroup _gameScreen;

        [SerializeField] private Button _randomRecognize;
        [SerializeField] private GameObject _modelSwitcher;
        
        [SerializeField] private GameObject _mainGameWindow;

        [SerializeField] private RectTransform _infoParent;
        [SerializeField] private TMP_Text _info;

        [SerializeField] private SerializedDictionary<string, string> _infos;
        [SerializeField] private List<Color> _colors;
        [SerializeField] private List<Image> _checks;
        [SerializeField] private Button _infoButton;
        
        [SerializeField] private TMP_Text _text;
        
        public bool InfoOpened;
        private float _progress;
        private bool _isXR;

        IEnumerator Start()
        {
            _infoParent.gameObject.SetActive(false);
            _gameScreen.gameObject.SetActive(false);
            _infoButton.gameObject.SetActive(false);
            _splash.gameObject.SetActive(true);
            _mainGameWindow.gameObject.SetActive(false);
            yield return new WaitForSeconds(3f);
            _progress = 0;
            _playScreen.gameObject.SetActive(true);
            DOTween.To(
                () => _progress,
                v =>
                {
                    _progress = v;
                    print(_progress);
                    _splash.alpha = 1 - v;
                    _splash.transform.localScale = Vector3.one * (1 + 0.5f * v);
                },
                1,
                0.8f
            ).SetEase(Ease.OutExpo).OnComplete(() =>
            {
                _splash.GetComponent<Canvas>().enabled = false;
                _splash.gameObject.SetActive(false);
            }).Play();
        }
            
        public void RevealMainGameScreen()
        {
#if !UNITY_ANDROID
            
            try
            {
                AppRoot.Instance.Initialize();
                AppRoot.Instance.DisableRecognition();
                _isXR = true;
            }
            catch (Exception e)
            {
                Debug.LogError(e);
                _isXR = false;
            }
#else
            _isXR = false;
#endif
            _randomRecognize.gameObject.SetActive(!_isXR);
            _modelSwitcher.gameObject.SetActive(!_isXR);
            SetCheck(-1);
            _gameScreen.gameObject.SetActive(true);
            _mainGameWindow.gameObject.SetActive(true);
            SetText("");
            _playScreen.GetComponent<GraphicRaycaster>().enabled = false;
            _progress = 0;
            DOTween.To(
                () => _progress,
                v =>
                {
                    _progress = v;
                    _playScreen.alpha = 1 - v;
                    _playScreen.transform.localScale = Vector3.one * (1 + 0.5f * v);
                },
                1,
                0.8f
            ).SetEase(Ease.OutExpo).OnComplete(() =>
            {
                _playScreen.GetComponent<Canvas>().enabled = false;
                _playScreen.gameObject.SetActive(false);
                if (_isXR)
                    AppRoot.Instance.EnableRecognition();
            }).Play();
        }

        public void SetCheck(int check)
        {
            _checks[0].transform.parent.parent.gameObject.SetActive(check >= 0);
            if (check < 0)
            {
                foreach (var image in _checks)
                {
                    image.gameObject.SetActive(false);
                }
                return;
            }
            
            var color = _colors[check];
            for (var i = 0; i <= check; i++)
            {
                var c = _checks[i];
                c.color = color;
                c.gameObject.SetActive(true);
            }

            for (var i = check + 1; i < _checks.Count; i++)
            {
                var c = _checks[i];
                c.color = color;
                c.gameObject.SetActive(false);
            }
        }

        public void SetText(string text)
        {
            if (string.IsNullOrWhiteSpace(text))
            {
                _text.transform.parent.gameObject.SetActive(false);
                _infoButton.gameObject.SetActive(false);
                _text.text = text;
                return;
            }

            _text.transform.parent.gameObject.SetActive(true);
            _text.text = text;
            _infoButton.gameObject.SetActive(true);
            _info.text = _infos[_text.text];
        }

        public void ToggleInfo()
        {
            InfoOpened = !InfoOpened;
            if (InfoOpened)
                CloseInfo();
            else
                OpenInfo();
        }
        
        public void OpenInfo()
        {
            _infoParent.gameObject.SetActive(true);
            _info.text = _infos[_text.text];
        }

        public void CloseInfo()
        {
            _infoButton.gameObject.SetActive(!string.IsNullOrWhiteSpace(_text.text));
            _infoParent.gameObject.SetActive(false);
        }
    }
}
