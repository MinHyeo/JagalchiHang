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
        InputManager.Instance.EnableGamePlayInput(true);
        NetworkManager.Instance.InitNetworkService();
        CreateManager();

        _mapManager.CreateMap();

        _playerManager.SpawnPlayer().Forget();

        ITargetable target = _playerManager;

        _monsterManager.Init(target);
        //_npcManager.Init(target);
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

    }
}