using UnityEngine;
using UnityEngine.AI;

public class RingSpawnAlgorithm : MonoBehaviour, IMonsterSpawnAlgorithm
{
    [SerializeField] private float _minSpawnDistance = 15f;
    [SerializeField] private float _maxSpawnDistance = 25f;
    [SerializeField] private int _maxAttemptCount = 10;
    [SerializeField] private float _navMeshSnapRadius = 2f;

    public bool TryGetSpawnPosition(Transform playerTransform, out Vector3 spawnPosition)
    {
        for (int attempt = 0; attempt < _maxAttemptCount; attempt++)
        {
            Vector2 randomDirection2D = Random.insideUnitCircle.normalized;
            float randomDistance = Random.Range(_minSpawnDistance, _maxSpawnDistance);
            Vector3 candidatePosition = playerTransform.position + (new Vector3(randomDirection2D.x, 0f, randomDirection2D.y) * randomDistance);

            if (!NavMesh.SamplePosition(candidatePosition, out NavMeshHit navMeshHit, _navMeshSnapRadius, NavMesh.AllAreas))
            {
                continue;
            }

            if (IsVisibleToCamera(navMeshHit.position))
            {
                continue;
            }

            spawnPosition = navMeshHit.position;
            return true;
        }

        spawnPosition = Vector3.zero;
        return false;
    }

    private bool IsVisibleToCamera(Vector3 worldPosition)
    {
        Camera playerCamera = Camera.main;

        if (playerCamera == null)
        {
            return false;
        }

        Vector3 viewportPoint = playerCamera.WorldToViewportPoint(worldPosition);
        bool isInFrontOfCamera = viewportPoint.z > 0f;
        bool isWithinViewportBounds = viewportPoint.x >= 0f && viewportPoint.x <= 1f && viewportPoint.y >= 0f && viewportPoint.y <= 1f;
        return isInFrontOfCamera && isWithinViewportBounds;
    }
}
