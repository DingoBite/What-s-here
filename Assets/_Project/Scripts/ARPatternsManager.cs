using System;
using System.Collections;
using System.Collections.Generic;
using NaughtyAttributes;
using UnityEditor;
using UnityEditor.XR.ARSubsystems;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

namespace _Project
{
    public enum TrackableState
    {
        Added,
        Removed,
        Updated,
    }
    
    
    public class ARPatternsManager : MonoBehaviour
    {
        public event Action<IEnumerable<(TrackableState state, TransformSwitcher trackable)>> OnTrackingTransformSwitchers;

        [SerializeField] private GameObject _arTrackedImageManagerGO;
        [SerializeField] private ARTrackedImageManager _arTrackedImageManager;
        [SerializeField] private GameObject _trackingPrefab;
        
        [SerializeField] private ARPlaneManager _arPlaneManager;
        [SerializeField] private float _maxAngle;
        [SerializeField] private float _distanceInfluenceThreshold = Vector3.kEpsilon;
        [SerializeField] private Texture2D _defaultTexture;
        [SerializeField] private XRReferenceImageLibrary _xrReferenceImageLibrary;
        
        private int _id;
        private RuntimeReferenceImageLibrary _runtimeReferenceImageLibrary;
        
        private Plane _patternPlane;
        private Camera _camera;

        private RawImage _cachedGameObject;
        private Texture2D _pattern;
        private AddReferenceImageJobState? _task;

        public (float, ARPlane) GetPrevPatternAngleDistance()
        {
            var minAngle = float.MaxValue;
            var minDist = float.MaxValue;
            ARPlane minPlane = null;
            var cameraTransform = _camera.transform;
            var position = cameraTransform.position;
            var forward = cameraTransform.forward;
            
            foreach (var plane in _arPlaneManager.trackables)
            {
                var angle = Vector3.Angle(-plane.normal, forward);
                var distance = plane.infinitePlane.GetDistanceToPoint(position);
                if (angle < minAngle || Math.Abs(angle - minAngle) < _distanceInfluenceThreshold && distance < minDist)
                {
                    minAngle = angle;
                    minDist = distance;
                    minPlane = plane;
                }
            }

            if (minAngle > _maxAngle)
                return (-1, minPlane);
            
            return (1 - minAngle / 180f, minPlane);
        }
        
        public void AddNewPattern(Texture2D pattern)
        {
            if (_cachedGameObject == null)
            {
                var go = new GameObject();
                go.transform.SetParent(transform);
                _cachedGameObject = go.AddComponent<RawImage>();
            }
            Debug.Log("Add new pattern");
            InitializeNewLibrary();
            if (_arTrackedImageManager.referenceLibrary is MutableRuntimeReferenceImageLibrary mutableLibrary)
            {
                if (_pattern != null)
                    Destroy(_pattern);
                
                var n = $"{pattern.name}__{_id}";
                _pattern = pattern;
                _cachedGameObject.texture = pattern;
                _cachedGameObject.SetNativeSize();
                _cachedGameObject.name = pattern.name;
                _task = mutableLibrary.ScheduleAddImageWithValidationJob(pattern, n, 0.3f);
                _id++;
            }
        }

        [Button]
        public void ResetTexture()
        {
            if (_arTrackedImageManager.referenceLibrary.count <= 0)
                return;
            var texture = _arTrackedImageManager.referenceLibrary[0].texture;
            texture.Reinitialize(_defaultTexture.width, _defaultTexture.height, _defaultTexture.format, false);
            texture.SetPixels(_defaultTexture.GetPixels());
            texture.Apply();
        }
        
        public void Initialize(Camera c)
        {
            // ResetTexture();
            _camera = c;
            _arTrackedImageManager = _arTrackedImageManagerGO.AddComponent<ARTrackedImageManager>();
            InitializeNewLibrary();
            _arTrackedImageManager.trackedImagePrefab = _trackingPrefab;
            _arTrackedImageManager.requestedMaxNumberOfMovingImages = 5;
            _arTrackedImageManager.enabled = true;
            _arTrackedImageManager.trackedImagesChanged -= TrackablesChanges;
            _arTrackedImageManager.trackedImagesChanged += TrackablesChanges;
        }

        private void InitializeNewLibrary()
        {
            _runtimeReferenceImageLibrary = _arTrackedImageManager.CreateRuntimeLibrary();
            _arTrackedImageManager.referenceLibrary = _runtimeReferenceImageLibrary;
            Debug.Log($"Is support mutable library {_arTrackedImageManager.descriptor.supportsMutableLibrary}");
        }

        private void TrackablesChanges(ARTrackedImagesChangedEventArgs changedEvent)
        {
            OnTrackingTransformSwitchers?.Invoke(GetTransformSwitchersFromTrackables(changedEvent));
        }

        private IEnumerable<(TrackableState state, TransformSwitcher trackable)> GetTransformSwitchersFromTrackables(ARTrackedImagesChangedEventArgs changedEvent)
        {
            foreach (var trackable in changedEvent.added)
            {
                var sw = trackable.GetComponentInChildren<TransformSwitcher>();
                if (sw != null)
                    yield return (TrackableState.Added, sw);
            }

            foreach (var trackable in changedEvent.removed)
            {
                var sw = trackable.GetComponentInChildren<TransformSwitcher>();
                if (sw != null)
                    yield return (TrackableState.Removed, sw);
            }
            
            foreach (var trackable in changedEvent.updated)
            {
                var sw = trackable.GetComponentInChildren<TransformSwitcher>();
                if (sw != null)
                    yield return (TrackableState.Updated, sw);
            }
        }

        private void Update()
        {
            if (_task != null)
            {
                print(_task.Value.status);
            }
        }
    }
}
