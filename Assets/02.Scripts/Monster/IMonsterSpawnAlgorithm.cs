using UnityEngine;

public interface IMonsterSpawnAlgorithm
{
    bool TryGetSpawnPosition(Vector3 playerPosition, out Vector3 spawnPosition);
}
