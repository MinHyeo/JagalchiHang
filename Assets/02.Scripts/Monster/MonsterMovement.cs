using UnityEngine;
using System;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class MonsterMovement : MonoBehaviour, IMonsterMoveable
{
    [SerializeField] private float _stoppingDistance = 1f;

    private NavMeshAgent _agent;
    private bool _isMoving;

    public bool HasReachedDestination
    {
        get { return !_agent.pathPending && _agent.remainingDistance <= _stoppingDistance; }
    }

    public bool IsMoving
    {
        get { return _isMoving; }
    }

    public Vector3 Velocity
    {
        get { return _agent.velocity; }
    }

    public event Action<bool> OnMovingStateChanged;

    private void Awake()
    {
        _agent = GetComponent<NavMeshAgent>();
    }

    public void ApplySpeed(float speed)
    {
        _agent.speed = speed;
    }

    private void OnEnable()
    {
        _isMoving = false;
        Stop();
    }

    private void Update()
    {
        bool wasMoving = _isMoving;
        bool isCurrentlyMoving = _agent.velocity.sqrMagnitude > 0.01f;

        if ( wasMoving != isCurrentlyMoving)
        {
            _isMoving = isCurrentlyMoving;
            OnMovingStateChanged?.Invoke(_isMoving);
        }
    }

    public void MoveTo(Vector3 destination)
    {
        _agent.SetDestination(destination);
    }

    public void Move(Vector3 direction)
    {
        if (direction == Vector3.zero)
        {
            return;
        }

        _agent.Move(direction.normalized * _agent.speed * Time.deltaTime);
    }

    public void Stop()
    {
        if (_agent == null || !_agent.isOnNavMesh)
        {
            return;
        }

        _agent.ResetPath();
    }
}
