using System;
using UnityEngine;

public class SampleWorldManager
{
    private PlayerManager _playerManager;

    public event Action<GameObject> PlayerSpawned;

    public void EnterWorld()
    {
        LoadSaveData();
        
        _playerManager = new PlayerManager();
        if (_playerManager == null) return;

        _playerManager.PlayerSpawned += OnPlayerSpawned;
        _playerManager.SpawnPlayer().Forget();
    }

    private void OnPlayerSpawned(GameObject player)
    {
        PlayerSpawned?.Invoke(player);
    }

    private void LoadSaveData()
    {

    }
}
