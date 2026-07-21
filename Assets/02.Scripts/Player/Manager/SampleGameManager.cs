using UnityEngine;

public class SampleGameManager : SingletonBase<SampleGameManager>
{
    private LobbyManager _lobbyManager;
    private SampleWorldManager _worldManager;

    private void Start()
    {
        _lobbyManager = new LobbyManager();
        _worldManager = new SampleWorldManager();

        _lobbyManager.EnterLobby();
    }

    // [TODO] 플레이어 위치 잘 받아오는지 테스트용 코드
    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.V))
        {
            UpdatePlayerPosition();
        }
    }

    private void UpdatePlayerPosition()
    {
        Vector3 playerPos = _worldManager.GetPlayerPosition();
        Debug.Log($"플레이어의 현재 위치 -> {playerPos}");
    }

    public Vector3 GetPlayerPosition()
    {
        return _worldManager.GetPlayerPosition();   
    }

    // TODO : 저장되어 있는 파일 전달해줘야 함
    public void EnterInGame()
    {
        _worldManager.EnterWorld();

        if (_lobbyManager == null)
            return;
        _lobbyManager.ExitLobby();
    }
}