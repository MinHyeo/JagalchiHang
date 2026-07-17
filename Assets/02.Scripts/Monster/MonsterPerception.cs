using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterPerception : MonoBehaviour, IMonsterPerceivable
{
    [SerializeField] private float _viewRadius = 10f;
    [SerializeField] private float _viewAngle = 90f;
    [SerializeField] private LayerMask _playerLayer;
    [SerializeField] private LayerMask _obstacleLayer;
    [SerializeField] private float _trailScanRadius = 5f;
    [SerializeField] private LayerMask _trailLayer;
    [SerializeField] private float _obstacleCheckInterval = 0.1f;

    private bool _canSeePlayer;
    private Vector3 _lastKnowPlayerPosition;
    private bool _hasDetectedTrail;
    private Vector3 _trailPosition;
    private ITrailmarker _lockedTrailMarker;
    private Transform _lockedTrailTransform;
    private SphereCollider _viewRangeCollider;
    private SphereCollider _trailRangeCollider;
    private readonly HashSet<Transform> _playersInRange = new HashSet<Transform>();
    private readonly List<Transform> _trailMarkersInRange = new List<Transform>();
    private bool _cachedIsBlockedByObstacle;

    public bool CanSeePlayer
    {
        get { return _canSeePlayer; }
    }

    public event Action<Vector3> OnPlayerSpotted;

    public Vector3 LastKnownPlayerPosition
    {
        get { return _lastKnowPlayerPosition; }
    }

    public bool HasDetectedTrail
    { 
        get { return _hasDetectedTrail; }
    }

    public Vector3 TrailPosition
    {
        get { return _trailPosition; }
    }

    private void Awake()
    {
        _viewRangeCollider = gameObject.AddComponent<SphereCollider>();
        _viewRangeCollider.isTrigger = true;
        _viewRangeCollider.radius = _viewRadius;

        _trailRangeCollider = gameObject.AddComponent<SphereCollider>();
        _trailRangeCollider.isTrigger = true;
        _trailRangeCollider.radius = _trailScanRadius;
    }

    private void OnEnable()
    {
        StartCoroutine(ObstacleCheckRoutine());
    }

    private void OnDisable()
    {
        StopAllCoroutines();
    }

    private IEnumerator ObstacleCheckRoutine() 
    {
        WaitForSeconds wait = new WaitForSeconds(_obstacleCheckInterval);

        while (true)
        {
            UpdateObstacleCheck();

            yield return wait;
        }
    }

    private void UpdateObstacleCheck() 
    {
        if (_playersInRange.Count == 0)
        {
            _cachedIsBlockedByObstacle = false;
            return;
        }

        Transform target = GetFirstPlayerCandidate();

        if (target == null)
        {
            _cachedIsBlockedByObstacle = false;
            return;
        }

        Vector3 directionToTarget = (target.position - transform.position).normalized;
        float distanceToTarget = Vector3.Distance(transform.position, target.position);

        _cachedIsBlockedByObstacle = Physics.Raycast(transform.position, directionToTarget, distanceToTarget, _obstacleLayer);
    }

    private Transform GetFirstPlayerCandidate()
    {
        foreach (Transform candidate in _playersInRange)
        {
            return candidate;
        }

        return null;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (IsOnLayer(other.gameObject, _playerLayer))
        {
            _playersInRange.Add(other.transform);
        }

        if (IsOnLayer(other.gameObject, _trailLayer))
        {
            if (!_trailMarkersInRange.Contains(other.transform))
            {
                _trailMarkersInRange.Add(other.transform);
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        _playersInRange.Remove(other.transform);
        _trailMarkersInRange.Remove(other.transform);
    }

    private bool IsOnLayer(GameObject target, LayerMask layerMask)
    {
        return (layerMask.value & (1 << target.layer)) != 0;
    }

    private void Update()
    {
        UpdatePlayerDetection();
        UpdateTrailDetection();
    }

    private void UpdatePlayerDetection()
    {
        bool wasSeeingPlayer = _canSeePlayer;

       if (_playersInRange.Count == 0)
        {
            _canSeePlayer = false;
            return;
        }

        Transform target = GetFirstPlayerCandidate();

        if (target == null)
        {
            _canSeePlayer = false;
            return;
        }

        Vector3 directionToTarget = (target.position - transform.position).normalized;

        Vector3 flatForward = transform.forward;
        flatForward.y = 0f;

        Vector3 flatDirectionToTarget = directionToTarget;
        flatDirectionToTarget.y = 0f;

        float angleToTarget = Vector3.Angle(flatForward, flatDirectionToTarget);

        if (angleToTarget > _viewAngle * 0.5f)
        {
            _canSeePlayer = false;
            return;
        }

        if (_cachedIsBlockedByObstacle)
        {
            _canSeePlayer = false;
            return;
        }

        _canSeePlayer = true;
        _lastKnowPlayerPosition = target.position;

        if (!wasSeeingPlayer)
        {
            OnPlayerSpotted?.Invoke(target.position);
        }
    }

    private void UpdateTrailDetection()
    {
        if (_lockedTrailTransform != null)
        {
            _trailPosition = _lockedTrailMarker.Position;
            return;
        }

        if (_trailMarkersInRange.Count == 0)
        {
            _hasDetectedTrail = false;
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
               // Debug.Log($"{name} : 스캔된 오브젝트 {hit.name}에 ITrailmarker가 없음");

                continue;
            }

            if (marker.Strength > highestStrength)
            {
                highestStrength = marker.Strength;
                strongestMarker = marker;
                strongestTransform = candidate;
            }
        }

        if (strongestMarker ==  null)
        {
            _hasDetectedTrail = false;
            return;
        }

        _lockedTrailMarker = strongestMarker;
        _lockedTrailTransform = strongestTransform;
        _hasDetectedTrail = true;
        _trailPosition = strongestMarker.Position;
    }

    public void ClearTrail()
    {
        _hasDetectedTrail = false;
        _lockedTrailMarker = null;
        _lockedTrailTransform = null;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = new Color(1f, 0.5f, 0f);
        Gizmos.DrawWireSphere(transform.position, _viewRadius);

        Vector3 leftBoundary = DirectionFromAngle(-_viewAngle * 0.5f);
        Vector3 rightBoundary = DirectionFromAngle(_viewAngle * 0.5f);

        Gizmos.DrawLine(transform.position, transform.position + (leftBoundary * _viewRadius));
        Gizmos.DrawLine(transform.position, transform.position + (rightBoundary * _viewRadius));

        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, _trailScanRadius);
    }

    private Vector3 DirectionFromAngle(float angleOffestDegrees)
    {
        float angleInRadians = (transform.eulerAngles.y + angleOffestDegrees) * Mathf.Deg2Rad;
        return new Vector3(Mathf.Sin(angleInRadians), 0f, Mathf.Cos(angleInRadians));
    }
}
