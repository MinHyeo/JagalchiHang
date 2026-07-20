using UnityEngine;
using System.Collections.Generic;

[RequireComponent(typeof(SphereCollider))]
public class TrailRangeSensor : MonoBehaviour
{
    [SerializeField] private LayerMask _trailLayer;

    private readonly List<Transform> _trailMarkersInRange = new List<Transform>();
    private ITrailmarker _lockedTrailMarker;
    private Transform _lockedTrailTransform;

    public bool HasDetectedTrail { get; private set; }
    public Vector3 TrailPosition { get; private set; }

    private void Awake()
    {
        GetComponent<SphereCollider>().isTrigger = true;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (IsOnLayer(other.gameObject))
        {
            if (!_trailMarkersInRange.Contains(other.transform))
            {
                _trailMarkersInRange.Add(other.transform);
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        _trailMarkersInRange.Remove(other.transform);
    }

    private bool IsOnLayer(GameObject target)
    {
        return (_trailLayer.value & (1 << target.layer)) != 0;
    }

    private void Update()
    {
        if (_lockedTrailTransform != null)
        {
            TrailPosition = _lockedTrailMarker.Position; 
            return;
        }

        if (_trailMarkersInRange.Count == 0)
        {
            HasDetectedTrail = false;
            return;
        }

        ITrailmarker strongestMarker = null;
        Transform strongestTransform = null;
        float highestStrength = 0f;

        foreach (Transform candidate in _trailMarkersInRange)
        {
            ITrailmarker marker = candidate.GetComponent<ITrailmarker>();

            if (marker == null)
            {
                continue;
            }

            if (marker.Strength > highestStrength)
            {
                highestStrength = marker.Strength;
                strongestMarker = marker;
                strongestTransform = candidate;
            }
        }

        if (strongestMarker == null)
        {
            HasDetectedTrail = false;
            return;
        }

        _lockedTrailMarker = strongestMarker;
        _lockedTrailTransform = strongestTransform;
        HasDetectedTrail = true;
        TrailPosition = strongestMarker.Position;
    }

    public void ClearTrail()
    {
        HasDetectedTrail = false;
        _lockedTrailMarker = null;
        _lockedTrailTransform = null;
    }
}
