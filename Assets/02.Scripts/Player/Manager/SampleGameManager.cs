using UnityEngine;

public class SampleGameManager : SingletonBase<GameManager>
{
    private LobbyManager _lobbyManager;
    private SampleWorldManager _worldManager;

    private void Start()
    {
        _lobbyManager = new LobbyManager();
        _worldManager = new SampleWorldManager();

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
}