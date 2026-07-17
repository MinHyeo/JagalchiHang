using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.InputSystem;

public class WorldManager
{
    private PlayerManager _playerManager;
    //private NpcManager _npcManager;
    //private MonsterManager _monsterManager;
    //private FarmManager _farmManager;

    public void EnterWorld(SaveModel saveModel)
    {
        LoadSaveData();

        _playerManager = new PlayerManager();
        //_monsterManager = new MonsterManager();
        //_npcManager = new NpcManager();
        //_farmManager = new FarmManager();

        _playerManager.SpawnPlayer().Forget();

        //ITargetable target = _playerManager;

        //_monsterManager.Init(target);
        //_npcManager.Init(target);
    }

    private void LoadSaveData()
    {

    }
}