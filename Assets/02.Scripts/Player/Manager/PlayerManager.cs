using Cysharp.Threading.Tasks;
using System;
using UnityEngine;

public class PlayerManager : ITargetable
{
    private GameObject _player;

    //[나라]TODO 
    private Vector3 _playerSpawnPos = Vector3.zero;

    //[나라]TODO : MainUI 생성되면 UIManager 통해 불러오기
    public event Action<GameObject> PlayerSpawned;

    public async UniTaskVoid SpawnPlayer()
    {
        _player = await GameObjectManager.Instance.CreateObjectAsync("Player_1", "Prefab/Player", _playerSpawnPos);
        if (_player == null) return;

        Debug.Log($"플레이어가 생성됐다!");

        PlayerSpawned?.Invoke(_player);
    }

    public Vector3 GetPosition()
    {
        if(_player == null)
        {
            return Vector3.zero;
        }

        return _player.transform.position;
    }

    public bool IsDead()
    {
        // [나라]TODO 
        return true;
    }
}
