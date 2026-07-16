using UnityEngine;
using System.Collections.Generic;

public static class SpawnPointRegistry
{
    private static readonly List<MonsterSpawnPoint> _spawnPoints = new List<MonsterSpawnPoint>();

    public static IReadOnlyList<MonsterSpawnPoint> All
    {
        get { return _spawnPoints; }
    }

    public static void Register(MonsterSpawnPoint spawnPoint)
    {
        _spawnPoints.Add(spawnPoint);
    }

    public static void Unregister(MonsterSpawnPoint spawnPoint)
    {
        _spawnPoints.Remove(spawnPoint);
    }
}
