using System;
using System.Text.RegularExpressions;
using FuzzySharp;
using NaughtyAttributes;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARFoundation.Samples;
using UnityEngine.XR.ARSubsystems;

namespace _Project
{
    public class RuntimeLibraryTest : MonoBehaviour
    {
        [SerializeField] private Camera _camera;
        [SerializeField] private XRReferenceImageLibrary _referenceImageLibrary;
        [SerializeField] private ARTrackedImageManager _arTrackedImageManager;

        [SerializeField] private RawImage _image;

        [SerializeField] private TextRecognizer _textRecognizer;
        [SerializeField] private TransformSwitcher _transformSwitcher;
        [SerializeField] private int _fuzzyThreshold;
        
        private RuntimeReferenceImageLibrary _runtimeReferenceImageLibrary;
        private RenderTexture _rt;

        private readonly Regex _normalizationPattern = new(@"[а-я\s]+");
        
        private void Start()
        {
            // _runtimeReferenceImageLibrary = _arTrackedImageManager.CreateRuntimeLibrary(_referenceImageLibrary);
            // _arTrackedImageManager.referenceLibrary = _runtimeReferenceImageLibrary;
            // _arTrackedImageManager.trackablesChanged.AddListener(Call);
            _textRecognizer.Initialize();
            AddNewImage();
            _textRecognizer.OnRecognized += TextRecognized;
        }

        private string Normalize(string text)
        {
            text = text.ToLowerInvariant();
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
        
        private void TextRecognized(string text)
        {
            text = Normalize(text);
            if (text == null)
                return;
            
            Debug.Log(text);
            var maxFuzz = int.MinValue;
            var maxKey = "";
            foreach (var key in _transformSwitcher.Keys)
            {
                var keyNormalized = Normalize(key);
                if (keyNormalized == null)
                {
                    Debug.LogError($"Key: {key} cannot be normalized");
                    continue;
                }
                var fuzzValue = Fuzz.Ratio(keyNormalized, text);
                if (fuzzValue > maxFuzz)
                {
                    maxFuzz = fuzzValue;
                    maxKey = key;
                }
            }
            if (maxFuzz < _fuzzyThreshold)
                _transformSwitcher.DisableAll();
            else
                _transformSwitcher.EnableTransform(maxKey);
        }

        // private void Call(ARTrackablesChangedEventArgs<ARTrackedImage> eventArgs)
        // {
        //     foreach (var trackedImage in eventArgs.added)
        //     {
        //         var parent = trackedImage.transform.GetChild(0).gameObject;
        //         if (trackedImage.trackingState != TrackingState.None)
        //         {
        //             parent.SetActive(true);
        //
        //             switch (trackedImage.referenceImage.name)
        //             {
        //                 case "my new image":
        //                     parent.transform.GetChild(1).gameObject.SetActive(true);
        //                     break;
        //             }
        //         }
        //         else
        //         {
        //             parent.SetActive(false);
        //         }
        //     }
        // }

        [Button]
        private void AddNewImage()
        {
            if (_rt == null)
            {
                _rt = new RenderTexture(1920, 1080, 12);
                _textRecognizer.SetCamera(_camera);
            }
            _camera.targetTexture = _rt;
            _camera.Render();
            _image.texture = _rt;
            // var texture = new Texture2D(rt.width, rt.height);
            // RenderTexture.active = rt;
            // texture.ReadPixels(new Rect(0, 0, rt.width, rt.height), 0, 0);
            // texture.Apply();
            // _image.texture = texture;
            //
            // RenderTexture.active = null;
            //
            // if (_arTrackedImageManager.referenceLibrary is MutableRuntimeReferenceImageLibrary mutableLibrary)
            // {
            //     mutableLibrary.ScheduleAddImageWithValidationJob(
            //         texture,
            //         "my new image",
            //         0.5f /* 50 cm */);
            // }

            _camera.targetTexture = null;
        }
    }
}
