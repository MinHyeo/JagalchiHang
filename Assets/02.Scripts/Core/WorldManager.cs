using UnityEngine;

public class WorldManager
{
    private PlayerManager _playerManager;
    private NpcManager _npcManager;
    private MonsterManager _monsterManager;
    private FarmManager _farmManager;
    private MapManager _mapManager;

    public void EnterWorld()
    {
        NetworkManager.Instance.InitNetworkService();
        CreateManager();

        _mapManager.CreateMap();

        _playerManager.SpawnPlayer().Forget();

        ITargetable target = _playerManager;

        _monsterManager.Init(target);
        //_npcManager.Init(target);
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

    }
}