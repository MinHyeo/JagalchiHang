using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[RequireComponent(typeof(SphereCollider))]
public class FlockRangeSensor : MonoBehaviour
{
    [SerializeField] private LayerMask _monsterLayer;
    [SerializeField] private float _separationWeight = 1.5f;
    [SerializeField] private float _alignmentWeight = 1f;
    [SerializeField] private float _cohesionWeight = 1f;
    [SerializeField] private float _separationDistance = 2f;
    [SerializeField] private float _obstacleAvoidDistance = 1.5f;
    [SerializeField] private float _obstacleAvoidWeight = 3f;
    [SerializeField] private LayerMask _obstacleLayer;
    [SerializeField] private LayerMask _playerLayer;
    [SerializeField] private float _obstacleCheckInterval = 0.1f;

    private readonly List<Transform> _neighbors = new List<Transform>();
    private Vector3 _cachedObstacleAvoidance;
    private Vector3 _lastMoveDirection;

    public IReadOnlyList<Transform> Neighbors
    {
        get { return _neighbors; }
    }

    private void Awake()
    {
        GetComponent<SphereCollider>().isTrigger = true;
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
        if (!IsOnLayer(other.gameObject))
        {
            return;
        }

        if (other.transform == transform.parent)
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

    private bool IsOnLayer(GameObject target)
    {
        return (_monsterLayer.value & (1 << target.layer)) != 0;
    }

    public Vector3 CalculateMoveDirection()
    {
        if (_neighbors.Count == 0)
        {
            return Vector3.zero;
        }

        Vector3 separation = CalculateSeparation();
        Vector3 alignment = CalculateAlignment();
        Vector3 cohesion = CalculateCohesion();

        Vector3 combinedDirection = (separation * _separationWeight) + (alignment * _alignmentWeight) + (cohesion * _cohesionWeight);

        _lastMoveDirection = combinedDirection;

        combinedDirection += _cachedObstacleAvoidance * _obstacleAvoidWeight;

        return combinedDirection;
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

        LayerMask avoidLayer = _obstacleLayer | _playerLayer;

        if (Physics.Raycast(transform.position, moveDirection.normalized, out RaycastHit hit, _obstacleAvoidDistance, avoidLayer))
        {
            return hit.normal;
        }

        return Vector3.zero;
    }

}
