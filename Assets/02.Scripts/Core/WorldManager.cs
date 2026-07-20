using UnityEngine;

public class WorldManager
{
    private PlayerManager _playerManager;
    private SaveModel _saveModel;
    //private NpcManager _npcManager;
    //private MonsterManager _monsterManager;
    private FarmManager _farmManager;


    public FarmManager GetFarmManager()
    {
        return _farmManager;
    }


    public void EnterWorld(SaveModel saveModel)
    {

        _playerManager = new PlayerManager();
        //_monsterManager = new MonsterManager();
        //_npcManager = new NpcManager();
        _farmManager = new FarmManager();
        _farmManager.Init();


        _playerManager.SpawnPlayer().Forget();

        //ITargetable target = _playerManager;

        //_monsterManager.Init(target);
        //_npcManager.Init(target);
    }


}