using System;
using UnityEngine;

public class MonsterPerception : MonoBehaviour, IMonsterPerceivable
{
    [SerializeField] private float _viewRadius = 10f;
    [SerializeField] private float _viewAngle = 90f;
    [SerializeField] private LayerMask _playerLayer;
    [SerializeField] private LayerMask _obstacleLayer;
    [SerializeField] private float _trailScanRadius = 5f;
    [SerializeField] private LayerMask _trailLayer;

    private bool _canSeePlayer;
    private Vector3? _lastKnowPlayerPosition;
    private bool _hasDetectedTrail;
    private Vector3? _trailPosition;
    private ITrailmarker _lockedTrailMarker;
    private Transform _lockedTrailTransform;

    public bool CanSeePlayer
    {
        get { return _canSeePlayer; }
    }

    public event Action<Vector3> OnPlayerSpotted;
   
    public Vector3? LastKnownPlayerPosition
    {
        get { return _lastKnowPlayerPosition; }
    }

    public bool HasDetectedTrail
    { 
        get { return _hasDetectedTrail; }
    }

    public Vector3? TrailPosition
    {
        get { return _trailPosition; }
    }

    private void Update()
    {
        UpdatePlayerDetection();
        UpdateTrailDetection();
    }

    private void UpdatePlayerDetection()
    {
        bool wasSeeingPlayer = _canSeePlayer;

        Collider[] hits = Physics.OverlapSphere(transform.position, _viewRadius, _playerLayer);

        // Debug.Log($"{name} : 플레이어 스캔 결과 {hits.Length}개 발견 (반경 {_viewRadius}, 레이어 {_playerLayer.value})");

        if (hits.Length == 0)
        {
            _canSeePlayer = false;
            return;
        }

        Transform target = hits[0].transform;

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

        float distanceToTarget = Vector3.Distance(transform.position, target.position);
        bool isBlocked = Physics.Raycast(transform.position, directionToTarget, distanceToTarget, _obstacleLayer);

        if (isBlocked)
        {
           // Debug.Log($"{name} : 장애물에 막혀서 실패 (거리 {distanceToTarget:F1})");
            _canSeePlayer = false;
            return;
        }

        _canSeePlayer = true;
        _lastKnowPlayerPosition = target.position;

        if (!wasSeeingPlayer)
        {
           // Debug.Log($"{name} : 플레이어 발견!");
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

        Collider[] hits = Physics.OverlapSphere(transform.position, _trailScanRadius, _trailLayer);

        // Debug.Log($"{name} : 흔적 스캔 결과 {hits.Length}개 발견 (반경 {_trailScanRadius})");

        if (hits.Length == 0)
        {
            _hasDetectedTrail = false;
            _trailPosition = null;
            return;
        }

        ITrailmarker strongestMarker = null;
        Transform strongestTransform = null;

        float highestStrength = 0f;

        foreach (Collider hit in hits)
        {
            ITrailmarker marker = hit.GetComponent<ITrailmarker>();

            if (marker == null)
            {
               // Debug.Log($"{name} : 스캔된 오브젝트 {hit.name}에 ITrailmarker가 없음");

                continue;
            }

            if (marker.Strength > highestStrength)
            {
                highestStrength = marker.Strength;
                strongestMarker = marker;
                strongestTransform = hit.transform;
            }
        }

        if (strongestMarker ==  null)
        {
            _hasDetectedTrail = false;
            _trailPosition = null;
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
        _trailPosition = null;
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
