using UnityEngine;

public class LobbyManager
{
    public void EnterLobby()
    {
        UIManager.Instance.OpenUI(UIRootType.MainUI, UIType.LobbyUI);
        UIManager.Instance.OpenUI(UIRootType.BackgroundUI, UIType.LobbyBackgroundUI);
    }

    public void ExitLobby()
    {
        UIManager.Instance.CloseUI(UIRootType.MainUI, UIType.LobbyUI);
        UIManager.Instance.CloseUI(UIRootType.ContentUI, UIType.LoadGameUI);
        UIManager.Instance.CloseUI(UIRootType.BackgroundUI, UIType.LobbyBackgroundUI);
    }
}