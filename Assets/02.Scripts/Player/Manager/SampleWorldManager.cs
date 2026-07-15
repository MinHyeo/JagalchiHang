using System;
using UnityEngine;

public class SampleWorldManager
{
    private PlayerManager _playerManager;

    public void EnterWorld()
    {
        LoadSaveData();
        
        _playerManager = new PlayerManager();
        if (_playerManager == null) return;

        _playerManager.SpawnPlayer().Forget();
    }

    private void LoadSaveData()
    {

    }
}
