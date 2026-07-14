using System;
using UnityEngine;

public class SampleGameManager : SingletonBase<SampleGameManager>
{
    private LobbyManager _lobbyManager;
    private SampleWorldManager _worldManager;

    public event Action<GameObject> PlayerSpawned;

    private void Start()
    {
        LoadData();

        _lobbyManager = new LobbyManager();
        _worldManager = new SampleWorldManager();

        _worldManager.PlayerSpawned += OnPlayerSpawned;

        _lobbyManager.EnterLobby();
    }

    // TODO : 저장되어 있는 파일 전달해줘야 함
    public void EnterInGame()
    {
        _worldManager.EnterWorld();

        if (_lobbyManager == null)
            return;
        _lobbyManager.ExitLobby();
    }

    private void OnPlayerSpawned(GameObject player)
    {
        PlayerSpawned?.Invoke(player);
    }

    private void LoadData()
    {
        if (GameDataManager.Instance == null)
        {
            Debug.LogError("GameDataManager.Instance가 없습니다.");
            return;
        }

        GameDataManager.Instance.LoadData<PlayerData>();

        Debug.Log("PlayerData 로드 완료");
    }
}
