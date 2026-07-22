using Cysharp.Threading.Tasks;
using UnityEngine;

public class WorldManager
{
    private PlayerManager _playerManager;
    private NpcManager _npcManager;
    private MonsterManager _monsterManager;
    private FarmManager _farmManager;
    private MapManager _mapManager;

    public async UniTask EnterWorld()
    {
        InputManager.Instance.EnableGamePlayInput(true);
        NetworkManager.Instance.InitNetworkService();
        CreateManager();

        await _mapManager.CreateMap();

        _playerManager.SpawnPlayer().Forget();

        ITargetable target = _playerManager;

        _monsterManager.Init(target);
        _npcManager.Init(target);
    }

    public void TransMap(MapType mapType)
    {
        // 기존 맵 제거 및 새로운 맵 불러오기
        _mapManager.ChangeMap(mapType);

        // 플레이어 위치 받아오기
        Vector3 spawnPosition = _mapManager.GetMapSpawnPosition();

        // 플레이어 위치 적용
        _playerManager.TransPlayerPosition(spawnPosition);

        // Farm 상태 갱신

        // Npc 상태 갱신

        // 몬스터 갱신
        bool isBunker = (mapType == MapType.ParkingGarage) ? true : false;

    }

    public void ExitWorld()
    {
        InputManager.Instance.EnableGamePlayInput(false);

    }

    private void CreateManager()
    {
        _playerManager = new PlayerManager();
        _monsterManager = new MonsterManager();
        _npcManager = new NpcManager();
        _farmManager = new FarmManager();
        _mapManager = new MapManager();
    }

    public void WorldUpdate()
    {
        _npcManager.NpcUpdate();
    }

    public PlayerManager GetPlayerManager()
    {
        return _playerManager;
    }

    public MonsterManager GetMonsterManager()
    {
        return _monsterManager;
    }

    public NpcManager GetNpcManager()
    {
        return _npcManager;
    }

    public FarmManager GetFarmManager()
    {
        return _farmManager;
    }

    public MapManager GetMapManager()
    {
        return _mapManager;
    }
}