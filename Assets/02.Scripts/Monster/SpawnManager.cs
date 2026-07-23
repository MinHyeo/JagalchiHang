using UnityEngine;
using Cysharp.Threading.Tasks;
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
    private readonly List<GameObject> _dynamicallySpawnedMonsters = new List<GameObject>();

    public void SetSpawningEnabled(bool isEnabled)
    {
        _isSpawningEnabled = isEnabled;
    }

    public void SetPlayerTarget(ITargetable playerTarget)
    {
        _playerTarget = playerTarget;
    }

    public void DespawnAllDynamicMonsters()
    {
        if (GameObjectManager.Instance == null)
        {
            Debug.LogWarning("SpawnManager : GameObjectManager.Instance가 null이라 동적 몬스터 제거를 건너뜁니다.");
            return;
        }

        foreach (GameObject monster in _dynamicallySpawnedMonsters)
        {
            if (monster == null || !monster.activeInHierarchy)
            {
                continue;
            }

            GameObjectManager.Instance.RequestDestroyObject(monster); 
        }

        _dynamicallySpawnedMonsters.Clear();

        Debug.Log("SpawnManager : 동적 생성 몬스터 전부 제거 완료");
    }

    private void Start()
    {
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
        SpawnDynamicMonsterAsync(spawnPosition, randomMonsterId).Forget();
    }

    private async UniTaskVoid SpawnDynamicMonsterAsync(Vector3 position, string monsterId)
    {
        if (!TryGetPrefabPath(monsterId, out string prefabPath))
        {
            return;
        }

        if (GameObjectManager.Instance == null)
        {
            Debug.LogWarning("SpawnManager : GameObjectManager.Instance가 null입니다. 씬에 GameObjectManager가 있는지 확인해주세요.");
            return;
        }

        GameObject spawnedMonster = await GameObjectManager.Instance.CreateObjectAsync(monsterId, prefabPath, position);

        if (spawnedMonster != null) 
        {
            _dynamicallySpawnedMonsters.Add(spawnedMonster);
        }
    }

    private void SpawnMonsterAt(Vector3 position, string monsterId)
    {
        if (!TryGetPrefabPath(monsterId, out string prefabPath))
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

    private bool TryGetPrefabPath(string monsterId, out string prefabPath)
    {
        prefabPath = string.Empty;

        if (string.IsNullOrEmpty(monsterId))
        {
            return false;
        }

        if (GameDataManager.Instance == null)
        {
            Debug.LogWarning("SpawnManager : GameDataManager.Instance가 null이라 프리팹 경로를 조회할 수 없습니다.");
            return false;
        }

        MonsterData data = GameDataManager.Instance.GetData<MonsterData>(monsterId); 

        if (data == null || string.IsNullOrEmpty(data.PrefabPath))
        {
            Debug.LogWarning($"SpawnManager : {monsterId}에 대한 PrefabPath를 찾을 수 없습니다.");
            return false;
        }

        prefabPath = data.PrefabPath;
        return true;
    }

    private int GetActiveMonsterCount()
    {
        return MonsterRegistry.ActiveCount;
    }
}
