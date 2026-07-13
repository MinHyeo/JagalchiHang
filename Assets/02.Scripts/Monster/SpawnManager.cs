using UnityEngine;
using System;
using System.Collections.Generic;

[Serializable]
public class MonsterSpawnEntry
{
    public string PrefabPath;
    public string MonsterId;
}

public class SpawnManager : SingletonBase<SpawnManager>
{
    [SerializeField] private Transform _playerTransform;
    [SerializeField] private List<MonsterSpawnEntry> _dynamicMonsterEntries;
    [SerializeField] private int _maxActiveMonsterCount = 10;
    [SerializeField] float _dynamicSpawnInterval = 5f;
    [SerializeField] bool _isSpawningEnabled = true;

    private IMonsterSpawnAlgorithm _spawnAlgorithm;
    private float _dynamicSpawnTimer;

    public void SetSpawningEnabled(bool isEnabled)
    {
        _isSpawningEnabled = isEnabled;
    }

    private void Awake()
    {
        _spawnAlgorithm = GetComponent<IMonsterSpawnAlgorithm>();
    }

    private void Start()
    {
        SpawnAllManualSpawnPoints();
    }

    private void Update()
    {
        UpdateDynamicSpawn();
    }

    private void SpawnAllManualSpawnPoints()
    {
        foreach (MonsterSpawnPoint spawnPoint in SpawnPointRegistry.All)
        {
            if (!spawnPoint.SpawnOnSceneStart)
            {
                continue;
            }

            SpawnMonsterAt(spawnPoint.Position, spawnPoint.MonsterPrefabPath, spawnPoint.MonsterId);
        }
    }

    private Transform GetPlayerTransform()
    {
        return _playerTransform;

        // TODO: GameManager 완성되면  수정용
        // if (GameManager.Instance == null || GameManager.Instance.Player == null)
        // {
        //     return null;
        // }
        //
        // return GameManager.Instance.Player.transform;
    }

    private void UpdateDynamicSpawn()
    {
        if (!_isSpawningEnabled)
        {
            return;
        }

        Transform playerTransform = GetPlayerTransform();

        if (_playerTransform == null)
        {
            return;
        }

        _dynamicSpawnTimer += Time.deltaTime;

        if (_dynamicSpawnTimer < _dynamicSpawnInterval)
        {
            return;
        }

        _dynamicSpawnTimer = 0f;

        if (GetActiveMonsterCount() >= _maxActiveMonsterCount)
        {
            return;
        }

        if (_dynamicMonsterEntries == null || _dynamicMonsterEntries.Count == 0)
        {
            return;
        }

        if (!_spawnAlgorithm.TryGetSpawnPosition(_playerTransform, out Vector3 spawnPosition))
        {
            return;
        }

        MonsterSpawnEntry randomEntry = _dynamicMonsterEntries[UnityEngine.Random.Range(0, _dynamicMonsterEntries.Count)];
        SpawnMonsterAt(spawnPosition, randomEntry.PrefabPath, randomEntry.MonsterId);
    }

    private void SpawnMonsterAt(Vector3 position, string prefabPath, string monsterId)
    {
        if (string.IsNullOrEmpty(prefabPath))
        {
            return;
        }

        if (GameObjectManager.Instance ==  null)
        {
            Debug.LogWarning("SpawnManager : GameObjectManager.Instance가 null입니다. GameObjectManager가 있는지 확인요망.");
            return;
        }

        GameObjectManager.Instance.CreateObject(monsterId, prefabPath, position);
    }

    private int GetActiveMonsterCount()
    {
        return MonsterRegistry.ActiveCount;
    }
}
