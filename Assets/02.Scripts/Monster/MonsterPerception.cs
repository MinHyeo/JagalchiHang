using System;
using UnityEngine;

public class MonsterPerception : MonoBehaviour, IMonsterPerceivable
{
    [SerializeField] private ViewRangeSensor _viewRangeSensor;
    [SerializeField] private TrailRangeSensor _trailRangeSensor;

    public event Action<Vector3> OnPlayerSpotted;

    public bool CanSeePlayer
    {
        get { return _viewRangeSensor != null && _viewRangeSensor.CanSeePlayer; }
    }

    public Vector3 LastKnownPlayerPosition
    {
        get { return _viewRangeSensor != null ? _viewRangeSensor.LastKnownPlayerPosition : Vector3.zero; }
    }

    public bool HasDetectedTrail
    {
        get { return _trailRangeSensor != null && _trailRangeSensor.HasDetectedTrail; }
    }

    public Vector3 TrailPosition
    {
        get { return _trailRangeSensor != null ? _trailRangeSensor.TrailPosition : Vector3.zero; }
    }

    private void OnEnable()
    {
        if (_viewRangeSensor != null)
        {
            _viewRangeSensor.OnPlayerSpotted += HandleViewSensorPlayerSpotted;
        }
    }

    private void OnDisable()
    {
        if (_viewRangeSensor != null)
        {
            _viewRangeSensor.OnPlayerSpotted -= HandleViewSensorPlayerSpotted;
        }
    }

    private void HandleViewSensorPlayerSpotted(Vector3 spottedPosition)
    {
        OnPlayerSpotted?.Invoke(spottedPosition);
    }

    public void ClearTrail()
    {
        if (_trailRangeSensor != null)
        {
            _trailRangeSensor.ClearTrail();
        }
    }
   
}
