using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

[RequireComponent(typeof(SphereCollider))]
public class ViewRangeSensor : MonoBehaviour
{
    [SerializeField] private LayerMask _playerLayer;
    [SerializeField] private float _viewAngle = 90f;
    [SerializeField] private LayerMask _obstacleLayer;
    [SerializeField] private float _obstacleCheckInterval = 0.1f;

    private readonly HashSet<Transform> _playersInRange = new HashSet<Transform>();
    private bool _cachedIsBlockedByObstacle;

    public event Action<Vector3> OnPlayerSpotted;

    public bool CanSeePlayer { get; private set; }
    public Vector3 LastKnownPlayerPosition { get; private set; }

    private void Awake()
    {
        GetComponent<SphereCollider>().isTrigger = true;
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
        Transform target = GetFirstCandidate();

        if (target == null)
        {
            _cachedIsBlockedByObstacle = false;
            return;
        }

        Vector3 directionToTarget = (target.position - transform.position).normalized;
        float distanceToTarget = Vector3.Distance(transform.position, target.position);

        _cachedIsBlockedByObstacle = Physics.Raycast(transform.position, directionToTarget, distanceToTarget, _obstacleLayer);
    }

    private Transform GetFirstCandidate()
    {
        foreach (Transform candidate in _playersInRange)
        {
            return candidate;
        }

        return null;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (IsOnLayer(other.gameObject))
        {
            _playersInRange.Add(other.transform);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        _playersInRange.Remove(other.transform);
    }

    private bool IsOnLayer(GameObject target)
    {
        return (_playerLayer.value & (1 << target.layer)) != 0;
    }

    private void Update()
    {
        bool wasSeeingPlayer = CanSeePlayer;

        Transform target = GetFirstCandidate();

        if (target == null)
        {
            CanSeePlayer = false;
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
            CanSeePlayer = false;
            return;
        }

        if (_cachedIsBlockedByObstacle)
        {
            CanSeePlayer = false;
            return;
        }

        CanSeePlayer = true;
        LastKnownPlayerPosition = target.position;

        if (!wasSeeingPlayer)
        {
            OnPlayerSpotted?.Invoke(target.position);
        }
    }

    private void OnDrawGizmosSelected()
    {
        SphereCollider collider = GetComponent<SphereCollider>();

        if (collider == null)
        {
            return;
        }

        float radius = collider.radius;

        Gizmos.color = new Color(1f, 0.5f, 0f);
        Gizmos.DrawWireSphere(transform.position, radius);

        Vector3 leftBoundary = DirectionFromAngle(-_viewAngle * 0.5f);
        Vector3 rightBoundary = DirectionFromAngle(_viewAngle * 0.5f);

        Gizmos.DrawLine(transform.position, transform.position + (leftBoundary * radius));
        Gizmos.DrawLine(transform.position, transform.position + (rightBoundary * radius));
    }

    private Vector3 DirectionFromAngle(float angleOffestDegrees)
    {
        float angleInRadians = (transform.eulerAngles.y + angleOffestDegrees) * Mathf.Deg2Rad;
        return new Vector3(Mathf.Sin(angleInRadians), 0f, Mathf.Cos(angleInRadians));
    }
}
