using UnityEngine;

public interface IMonsterSpawnAlgorithm
{
    bool TryGetSpawnPosition(Transform playerTransform, out Vector3 spawnPosition);
}
