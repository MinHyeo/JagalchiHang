using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class MonsterGroup : MonoBehaviour, IMonsterGroupBehavior
{
    [Header("경보")] 
    [SerializeField] private float _alertRadius = 15f;
    [SerializeField] private float _alertDuration = 8f;

    [Header("무리 이동")]
    [SerializeField] private float _flockRadius = 6f;
    [SerializeField] private float _separationWeight = 1.5f;
    [SerializeField] private float _alignmentWeight = 1f;
    [SerializeField] private float _cohesionWeight = 1f;
    [SerializeField] private float _separationDistance = 2f;
    [SerializeField] private float _obstacleAvoidDistance = 1.5f;
    [SerializeField] private float _obstacleAvoidWeight = 3f;
    [SerializeField] private float _obstacleCheckInterval = 0.1f;

    [Header("공용")]
    [SerializeField] private LayerMask _monsterLayer;
    [SerializeField] private LayerMask _obstacleLayer;

    private IMonsterPerceivable _perceivable;
    private IMonsterMoveable _moveable;
    private SphereCollider _detectionCollider;
    private readonly List<Transform> _neighbors = new List<Transform>();
    private bool _isAlerted;
    private Vector3 _alertPosition;
    private float _alertElapsedTime;
    private Vector3 _cachedObstacleAvoidance;
    private Vector3 _lastMoveDirection;

    public bool IsAlerted
    {
        get { return _isAlerted; }
    }

    public Vector3 AlertPosition
    {
        get { return _alertPosition; }
    }

    public IReadOnlyList<Transform> Neighbors
    {
        get { return _neighbors; }
    }

    private void Awake()
    {
        _perceivable = GetComponent<IMonsterPerceivable>();
        _moveable = GetComponent<IMonsterMoveable>();

        _detectionCollider = gameObject.AddComponent<SphereCollider>();
        _detectionCollider.isTrigger = true;
        _detectionCollider.radius = _flockRadius;

        _perceivable.OnPlayerSpotted += HandlePlayerSpotted;
    }

    private void OnDestroy()
    {
        if (_perceivable is UnityEngine.Object perceivableObject && perceivableObject != null)
        {
            _perceivable.OnPlayerSpotted -= HandlePlayerSpotted;
        }
    }

    private void OnEnable()
    {
        StartCoroutine(ObstacleAvoidanceRoutine());
    }

    private void OnDisable()
    {
        StopAllCoroutines();
    }

    private IEnumerator ObstacleAvoidanceRoutine()
    {
        WaitForSeconds wait = new WaitForSeconds(_obstacleCheckInterval);

        while (true)
        {
            _cachedObstacleAvoidance = CalculateObstacleAvoidance(_lastMoveDirection);

            yield return wait;
        }
    }

    private void OnTriggerEnter(Collider other) 
    {
        if (!IsOnMonsterLayer(other.gameObject))
        {
            return;
        }

        if (other.gameObject == gameObject)
        {
            return;
        }

        if (!_neighbors.Contains(other.transform))
        {
            _neighbors.Add(other.transform);
        }
    }

    private void OnTriggerExit(Collider other) 
    {
        _neighbors.Remove(other.transform);
    }

    private bool IsOnMonsterLayer(GameObject target)
    {
        return (_monsterLayer.value & (1 << target.layer)) != 0;
    }

    private void Update() 
    {
        if (!_isAlerted)
        {
            return;
        }

        _alertElapsedTime += Time.deltaTime;

        if (_alertElapsedTime >= _alertDuration)
        {
            _isAlerted = false;
        }
    }

    private void HandlePlayerSpotted(Vector3 spottedPosition)
    {
        BroadcastAlert(spottedPosition);
    }

    private void BroadcastAlert(Vector3 alertPosition) 
    {
        Collider[] hits = Physics.OverlapSphere(transform.position, _alertRadius, _monsterLayer);

        foreach (Collider hit in hits)
        {
            if (hit.gameObject == gameObject)
            {
                continue;
            }

            IMonsterGroupBehavior groupBehavior = hit.GetComponent<IMonsterGroupBehavior>();

            if (groupBehavior == null)
            {
                continue;
            }

            groupBehavior.ReceiveAlert(alertPosition);
        }
    }

    public void ReceiveAlert(Vector3 alertPosition)
    {
        _isAlerted = true;
        _alertPosition = alertPosition;
        _alertElapsedTime = 0f;

        Debug.Log($"{name} : 경보 수신, 목표 위치 {alertPosition}");
    }

    public void ClearAlert()
    {
        _isAlerted = false;

        Debug.Log($"{name} : 경보 해제");
    }

    public void Tick()
    {
        if (_neighbors.Count == 0)
        {
            return;
        }

        Vector3 separation = CalculateSeparation();
        Vector3 alignment = CalculateAlignment();
        Vector3 cohesion = CalculateCohesion();

        Vector3 combinedDirection = (separation * _separationWeight) + (alignment * _alignmentWeight) + (cohesion * _cohesionWeight);
        _lastMoveDirection = combinedDirection;

        combinedDirection += _cachedObstacleAvoidance * _obstacleAvoidWeight;

        _moveable.Move(combinedDirection);
    }

    private Vector3 CalculateSeparation()
    {
        Vector3 result = Vector3.zero;

        foreach (Transform neighbor in _neighbors)
        {
            float distance = Vector3.Distance(transform.position, neighbor.position);

            if (distance >= _separationDistance || distance <= 0f)
            {
                continue;
            }

            result += (transform.position - neighbor.position) / distance;
        }

        return result;
    }

    private Vector3 CalculateAlignment()
    {
        Vector3 sumVelocity = Vector3.zero;

        foreach (Transform neighbor in _neighbors)
        {
            IMonsterMoveable neighborMoveable = neighbor.GetComponent<IMonsterMoveable>();

            if (neighborMoveable == null)
            {
                continue;
            }

            sumVelocity += neighborMoveable.Velocity;
        }

        return (sumVelocity / _neighbors.Count).normalized;
    }

    private Vector3 CalculateCohesion()
    {
        Vector3 sumPosition = Vector3.zero;

        foreach (Transform neighbor in _neighbors)
        {
            sumPosition += neighbor.position;
        }

        Vector3 centerPosition = sumPosition / _neighbors.Count;

        return (centerPosition - transform.position).normalized;
    }

    private Vector3 CalculateObstacleAvoidance(Vector3 moveDirection)
    {
        if (moveDirection == Vector3.zero)
        {
            return Vector3.zero;
        }

        if (Physics.Raycast(transform.position, moveDirection.normalized, out RaycastHit hit, _obstacleAvoidDistance, _obstacleLayer))
        {
            return hit.normal;
        }

        return Vector3.zero;
    }
}
