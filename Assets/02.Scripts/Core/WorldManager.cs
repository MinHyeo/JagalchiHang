using Cysharp.Threading.Tasks;
using Cysharp.Threading.Tasks.Triggers;
using UnityEngine;

public class WorldManager
{
    private PlayerManager _playerManager;
    private NpcManager _npcManager;
    private MonsterManager _monsterManager;
    private FarmManager _farmManager;
    private MapManager _mapManager;

    public async UniTask EnterWorld(SaveModel saveModel, int slotIndex)
    {
        InputManager.Instance.EnableGamePlayInput(true);
        NetworkManager.Instance.InitNetworkService();

        _playerManager = new PlayerManager();
        _npcManager = new NpcManager();
        _mapManager = new MapManager();
        _farmManager = new FarmManager();
        _monsterManager = new MonsterManager();

        await _playerManager.SpawnPlayer(saveModel);
        await _mapManager.CreateMap(saveModel.MapType);
        ITargetable target = _playerManager;

        _npcManager.Init(target);

        NetworkManager.Instance.RequestLoadGame(saveModel);

        _farmManager.Init();

        _monsterManager.Init(target);

        bool isInBunker = IsBunkerMap(saveModel.MapType);
        _npcManager.OnBunkerData(isInBunker);

        NetworkManager.Instance.InventoryService.TestItem();

        SoundManager.Instance.StopBGM();
        UIManager.Instance.OpenUI(UIRootType.MainUI, UIType.MainUI);
        SoundManager.Instance.PlayBGM("Sounds/InGame");
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
        _farmManager.OnMapChanged();

        // Npc 상태 갱신
        bool isinBunker = (mapType == MapType.ParkingGarage) ? true : false;
        _npcManager.OnBunkerData(isinBunker);


        // 몬스터 갱신
        bool isBunker = (mapType == MapType.ParkingGarage) ? true : false;

        NetworkManager.Instance.FarmingService.OnExitMap();
    }

    private bool IsBunkerMap(MapType mapType)
    {
        return mapType == MapType.ParkingGarage;
    }

    public void ExitWorld()
    {
        CloseAllUI();
        UIManager.Instance.OpenUI(UIRootType.VeryFrontUI, UIType.LoadingUI);
        InputManager.Instance.EnableGamePlayInput(false);
        GameObjectManager.Instance.RemoveAllObject();
    }

    public void WorldUpdate()
    {
        if (_npcManager != null)
        {
            _npcManager.NpcUpdate();
        }
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

    private void CloseAllUI()
    {
        UIManager.Instance.CloseUI(UIRootType.MainUI, UIType.MainUI);
        UIManager.Instance.RemoveAllSlotHudInteraction();
        UIManager.Instance.CloseUI(UIRootType.MainUI, UIType.HudMainUI);
        UIManager.Instance.CloseUI(UIRootType.PopupUI, UIType.InventoryUI);
        UIManager.Instance.CloseUI(UIRootType.PopupUI, UIType.FarmingUI);
        UIManager.Instance.CloseUI(UIRootType.PopupUI, UIType.NpcUI);
        UIManager.Instance.CloseUI(UIRootType.PopupUI, UIType.FarmSeedSelectUI);
        UIManager.Instance.CloseUI(UIRootType.PopupUI, UIType.CraftUI);
        UIManager.Instance.CloseUI(UIRootType.PopupUI, UIType.SettingUI);
        UIManager.Instance.CloseUI(UIRootType.PopupUI, UIType.FarmPlotStatusUI);
        UIManager.Instance.CloseUI(UIRootType.PopupUI, UIType.GeneratorUI);
    }
}