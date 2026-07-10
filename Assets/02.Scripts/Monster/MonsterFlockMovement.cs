using UnityEngine;
using System.Collections.Generic;

public class MonsterFlockMovement : MonoBehaviour, IMonsterFlockMovable
{
    [SerializeField] private float _separationWeight = 1.5f;
    [SerializeField] private float _alignmentWeight = 1f;
    [SerializeField] private float _cohesionWeight = 1f;
    [SerializeField] private float _separationDistance = 2f;

    private IMonsterMoveable _moveable;
    private IMonsterFlockperceivable _flockPerceivable;

    private void Awake()
    {
        _moveable = GetComponent<IMonsterMoveable>();
        _flockPerceivable = GetComponent<IMonsterFlockperceivable>();
    }

    public void Tick()
    {
        IReadOnlyList<Transform> neighbors = _flockPerceivable.Neighbors;

        if (neighbors.Count == 0) 
        {
            return; 
        }

        Vector3 separation = CalculateSeparation(neighbors);
        Vector3 alignment = CalculateAlignment(neighbors);
        Vector3 cohesion = CalculateCohesion(neighbors);

        Vector3 combinedDirection = (separation * _separationWeight) + (alignment * _alignmentWeight) + (cohesion * _cohesionWeight);
        _moveable.Move(combinedDirection);
    }

    private Vector3 CalculateSeparation(IReadOnlyList<Transform> neighbors)
    {
        Vector3 result = Vector3.zero;

        foreach (Transform neighbor in neighbors)
        {
            float distance = Vector3.Distance(transform.position, neighbor.position);

            if (distance >= _separationDistance)
            {
                continue;
            }

            if (distance <= 0f)
            {
                continue;
            }

            Vector3 awayDirection = (transform.position - neighbor.position) / distance;
            result += awayDirection;
        }

        return result;
    }

    private Vector3 CalculateAlignment(IReadOnlyList<Transform> neighbors)
    {
        Vector3 sumVelocity = Vector3.zero;

        foreach (Transform neighbor in neighbors)
        {
            IMonsterMoveable neighborMoveable = neighbor.GetComponent<IMonsterMoveable>();

            if (neighborMoveable == null)
            {
                continue;
            }

            sumVelocity += neighborMoveable.Velocity;
        }

        return (sumVelocity / neighbors.Count).normalized;
    }

    private Vector3 CalculateCohesion(IReadOnlyList<Transform> neighbors)
    {
        Vector3 sumPosition = Vector3.zero;

        foreach (Transform neighbor in neighbors)
        {
            sumPosition += neighbor.position;
        }

        Vector3 centerPosition = sumPosition / neighbors.Count;

        return (centerPosition - transform.position).normalized;
    }
}
