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
    }

    // TODO : 저장되어 있는 파일 전달해줘야 함
    public void EnterInGame()
    {
        _worldManager.EnterWorld();

        if (_lobbyManager == null)
            return;
        _lobbyManager.ExitLobby();
    }

    public void ExitInGame()
    {

    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.I))
        {
            if(UIManager.Instance.IsOpenUI(UIType.InventoryUI)) 
            {
                UIManager.Instance.CloseUI(UIRootType.PopupUI, UIType.InventoryUI);
            }
            else
            {
                UIManager.Instance.OpenUI(UIRootType.PopupUI, UIType.InventoryUI);
            }
        }

        _worldManager.WorldUpdate();
    }
}