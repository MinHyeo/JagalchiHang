using UnityEngine;

public class SampleWorldManager : SingletonBase<SampleWorldManager>
{
    [SerializeField] private Transform _playerSpawnSpot;

    private void Start()
    {
        SpawnPlayer();
    }

    private void SpawnPlayer()
    {
        GameObjectManager.Instance.CreateObject("Player_1", "Prefab/Player", _playerSpawnSpot.position);
        Debug.Log($"플레이어가 생성됐다!");
    }
}
