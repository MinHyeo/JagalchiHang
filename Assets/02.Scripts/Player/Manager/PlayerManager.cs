using Cysharp.Threading.Tasks;
using UnityEngine;

public class PlayerManager : ITargetable
{
    private GameObject _player;
    //[나라]TODO 
    private Vector3 _playerSpawnPos = new Vector3(0f, 0f, 0f);

    public async UniTaskVoid SpawnPlayer()
    {
        _player = await GameObjectManager.Instance.CreateObjectAsync("Player_1", "Prefab/Player", _playerSpawnPos);
        if (_player == null) return;

        Debug.Log($"플레이어가 생성됐다!");
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
