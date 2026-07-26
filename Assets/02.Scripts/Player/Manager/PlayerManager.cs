using Cysharp.Threading.Tasks;
using UnityEngine;

public class PlayerManager : ITargetable
{
    private GameObject _player;

    //[나라]TODO 
    private Vector3 _playerSpawnPos = new Vector3(20f, 1f, -3f);
    public Vector3 PlayerSpawnPos
    {
        get => _playerSpawnPos;
        set => _playerSpawnPos = value;
    }
    private PlayerController _playerController;

    // 플레이어 동적 생성
    public async UniTask SpawnPlayer()
    {
        LoadPlayerData();

        _player = await GameObjectManager.Instance.CreateObjectAsync("Player_1", "Prefab/Player", PlayerSpawnPos);
        if (_player == null) return;

        Debug.Log($"플레이어가 생성됐다!");

        _playerController = _player.GetComponent<PlayerController>();
        if (_playerController == null) return;

        UpdateCameraTarget();
    }

    // 플레이어 부활
    public async UniTaskVoid RespawnPlayer()
    {
        // 현재 실행을 잠시 멈추고 다음 프레임에 이어서 실행
        await UniTask.Yield();
        await UniTask.Delay(3000);

        var mapManager =  GameUtil.GetMapManager();
        if(mapManager == null) return;

        var respawnPos = mapManager.GetMapSpawnPosition();
        if (respawnPos == null) return;

        _player = await GameObjectManager.Instance.CreateObjectAsync("Player_1", "Prefab/Player", respawnPos);
        if(_player == null) return;

        _playerController = _player.GetComponent<PlayerController>();
        if(_playerController == null) return;

        _playerController.ResetPlayerState();
        UpdateCameraTarget();
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
