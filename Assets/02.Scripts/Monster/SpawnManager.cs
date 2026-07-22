using UnityEngine;
using System;
using System.Collections.Generic;

[RequireComponent(typeof(RingSpawnAlgorithm))]
public class SpawnManager : SingletonBase<SpawnManager>
{
    [SerializeField] private int _maxActiveMonsterCount = 10;
    [SerializeField] float _dynamicSpawnInterval = 5f;
    [SerializeField] bool _isSpawningEnabled = true;

    private IMonsterSpawnAlgorithm _spawnAlgorithm;
    private float _dynamicSpawnTimer;
    private ITargetable _playerTarget;

    private readonly List<string> _dynamicMonsterIds = new List<string>();

    public void SetSpawningEnabled(bool isEnabled)
    {
        _isSpawningEnabled = isEnabled;
    }

    public void SetPlayerTarget(ITargetable playerTarget)
    {
        _playerTarget = playerTarget;
    } 

    private void Start()
    {
        _spawnAlgorithm = GetComponent<IMonsterSpawnAlgorithm>();
        _spawnAlgorithm = GetComponent<IMonsterSpawnAlgorithm>();
        LoadDynamicSpawnTable();
        SpawnAllManualSpawnPoints();
    }

    private void Update()
    {
        UpdateDynamicSpawn();
    }

    private void LoadDynamicSpawnTable()
    {
        _dynamicMonsterIds.Clear();

        if (GameDataManager.Instance == null)
        {
            Debug.LogWarning("SpawnManager : GameDataManager.Instance가 null입니다.");
            return;
        }

        GameDataManager.Instance.LoadData<MonsterData>();
        List<string> allIds = GameDataManager.Instance.GetAllDataId<MonsterData>();

        if (allIds == null) 
        {
            Debug.LogWarning("SpawnManager : 스폰 테이블 데이터를 찾을 수 없습니다.");
            return;
        }

        foreach (string id in allIds) 
        {
            MonsterData data = GameDataManager.Instance.GetData<MonsterData>(id);

            if (data == null || string.IsNullOrEmpty(data.PrefabPath))
            {
                continue;
            }

            _dynamicMonsterIds.Add(id);
        }

        Debug.Log($"SpawnManager : 동적 생성 후보 {_dynamicMonsterIds.Count}개 로드 완료");
    }

    private void SpawnAllManualSpawnPoints()
    {
        foreach (MonsterSpawnPoint spawnPoint in SpawnPointRegistry.All)
        {
            if (!spawnPoint.SpawnOnSceneStart)
            {
                continue;
            }

            SpawnMonsterAt(spawnPoint.Position, spawnPoint.MonsterId);
        }
    }

    private bool TryGetPlayerPosition(out Vector3 position)
    {
        if (_playerTarget != null)
        {
            position = _playerTarget.GetPosition();
            return true;
        }

        position = Vector3.zero;
        return false;
    }

    private void UpdateDynamicSpawn()
    {
        if (!_isSpawningEnabled)
        {
            return;
        }

        if (!TryGetPlayerPosition(out Vector3 playerPosition))
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

        if (_dynamicMonsterIds.Count == 0)
        {
            return;
        }

        if (!_spawnAlgorithm.TryGetSpawnPosition(playerPosition, out Vector3 spawnPosition))
        {
            return;
        }

        string randomMonsterId = _dynamicMonsterIds[UnityEngine.Random.Range(0, _dynamicMonsterIds.Count)];
        SpawnMonsterAt(spawnPosition, randomMonsterId);
    }

    private void SpawnMonsterAt(Vector3 position, string monsterId)
    {
        if (string.IsNullOrEmpty(monsterId))
        {
            return;
        }

        if (GameObjectManager.Instance ==  null)
        {
            Debug.LogWarning("SpawnManager : GameObjectManager.Instance가 null입니다. GameObjectManager가 있는지 확인요망.");
            return;
        }

        MonsterData data = GameDataManager.Instance.GetData<MonsterData>(monsterId);

        if (data == null || string.IsNullOrEmpty(data.PrefabPath))
        {
            Debug.LogWarning($"SpawnManager : {monsterId}에 대한 PrefabPath를 찾을 수 없습니다.");
            return;
        }

        if (GameObjectManager.Instance == null)
        {
            Debug.LogWarning("SpawnManager : GameObjectManager.Instance가 null입니다. 씬에 GameObjectManager가 있는지 확인요망.");
            return;
        }

        GameObjectManager.Instance.CreateObject(monsterId, data.PrefabPath, position);
    }

    private int GetActiveMonsterCount()
    {
        return MonsterRegistry.ActiveCount;
    }
}
