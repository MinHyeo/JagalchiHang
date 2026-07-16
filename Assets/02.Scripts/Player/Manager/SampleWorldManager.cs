using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.InputSystem;

public class SampleWorldManager
{
    private PlayerManager _playerManager;
    //private NpcManager _npcManager;
    //private MonsterManager _monsterManager;
    //private FarmManager _farmManager;

    public void EnterWorld()
    {
        LoadSaveData();

        _playerManager = new PlayerManager();
        //_monsterManager = new MonsterManager();
        //_npcManager = new NpcManager();
        //_farmManager = new FarmManager();

        SampleUIManager.Instance.CreateUI(SampleUIRootType.MainUI, SampleUIType.TestPlayerStatus);
        _playerManager.SpawnPlayer().Forget();

        //ITargetable target = _playerManager;

        //_monsterManager.Init(target);
        //_npcManager.Init(target);
    }

    private void LoadSaveData()
    {

    }
}