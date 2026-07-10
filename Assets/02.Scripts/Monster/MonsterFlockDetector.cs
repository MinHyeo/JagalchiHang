using UnityEngine;
using System.Collections.Generic;

public class MonsterFlockDetector : MonoBehaviour, IMonsterFlockperceivable
{
    [SerializeField] private float _flockRadius = 6f;
    [SerializeField] private LayerMask _monsterLayer;

    private readonly List<Transform> _neighbors = new List<Transform>();

    public IReadOnlyList<Transform> Neighbors
    {
        get { return _neighbors; }
    }

    private void Update()
    {
        UpdateNeighbors();
    }

    private void UpdateNeighbors()
    {
        _neighbors.Clear();

        Collider[] hits = Physics.OverlapSphere(transform.position, _flockRadius, _monsterLayer);

        foreach (Collider hit in hits)
        {
            if (hit.gameObject == gameObject)
            {
                continue;
            }

            _neighbors.Add(hit.transform);
        }
    }
}
