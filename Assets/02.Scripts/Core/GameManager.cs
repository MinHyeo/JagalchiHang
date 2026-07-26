using Cysharp.Threading.Tasks;
using UnityEngine;

public class GameManager : SingletonBase<GameManager>
{
    private LobbyManager _lobbyManager;
    private WorldManager _worldManager;

    private void Start()
    {
        _lobbyManager = new LobbyManager();
        _worldManager = new WorldManager();

        _lobbyManager.EnterLobby();
        //_worldManager.EnterWorld(); //테스트용

        NetworkManager.Instance.InitNetworkService();
    }

    // TODO : 저장되어 있는 파일 전달해줘야 함
    public void EnterInGame(SaveModel saveModel, int slotIndex)
    {
        UIManager.Instance.OpenUI(UIRootType.VeryFrontUI, UIType.LoadingUI);
        _worldManager.EnterWorld(saveModel, slotIndex).Forget();

        if (_lobbyManager == null)
            return;
        _lobbyManager.ExitLobby();
    }

    public void ExitInGame()
    {

    }

    public LobbyManager GetLobbyManager()
    {
        return _lobbyManager;
    }

    public WorldManager GetWorldManager()
    {
        return _worldManager;
    }

    private void Update()
    {
        _worldManager.WorldUpdate();
    }
}