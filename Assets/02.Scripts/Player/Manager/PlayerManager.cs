using Cysharp.Threading.Tasks;
using System;
using System.Security;
using UnityEngine;

public class PlayerManager : ITargetable
{
    private GameObject _player;

    //[나라]TODO 
    private Vector3 _playerSpawnPos = new Vector3(20f, 1f, -3f);
    private PlayerController _playerController;

    public event Action<Monster> MonsterAttacked;

    // 플레이어 동적 생성
    public async UniTaskVoid SpawnPlayer()
    {
        LoadPlayerData();

        _player = await GameObjectManager.Instance.CreateObjectAsync("Player_1", "Prefab/Player", _playerSpawnPos);
        if (_player == null) return;

        Debug.Log($"플레이어가 생성됐다!");

        _playerController = _player.GetComponent<PlayerController>();
        if (_playerController == null) return;

        _playerController.OnMonsterAttacked += OnMonsterAttacked;

        UpdateCameraTarget();
        BindMainUIView();
    }

    public void TransPlayerPosition(Vector3 transPosition)
    {
        _player.transform.position = transPosition;
    }

    // 생성된 플레이어를 카메라의 추적 대상으로 설정
    private void UpdateCameraTarget()
    {
        CameraController.SetTrackingTarget(_player.transform);
    }

    private void BindMainUIView()
    {
        var vm = NetworkManager.Instance.PlayerService.GetPlayerViewModel();
        if (vm == null) return;

        var mainUI = UIManager.Instance.GetOpenUI(UIRootType.MainUI, UIType.MainUI);
        if (mainUI == null) return;

        var mainUIComponent = mainUI.GetComponent<MainUI>();
        if (mainUIComponent == null) return;

        mainUIComponent.BindViewModel(vm);
    }

    private void OnMonsterAttacked(Monster monster)
    {
        if (monster == null)
        {
            return;
        }

        MonsterAttacked?.Invoke(monster);
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
        var component = _player.GetComponent<PlayerController>();
        if (component == null) return false;

        return component.IsDie == true;
    }

    private void LoadPlayerData()
    {
        if (GameDataManager.Instance == null)
        {
            Debug.LogError("GameDataManager.Instance가 없습니다.");
            return;
        }

        GameDataManager.Instance.LoadData<PlayerData>();

        Debug.Log("PlayerData 로드 완료");
    }

    public void NotifyPlayerAttackedMonster(Monster targetMonster)
    {
        if (targetMonster == null) return;

        NpcManager npcManager = GameUtil.GetNpcManager();
        if (npcManager == null) return;

        npcManager.SetTargetMonster(targetMonster);
    }
}
