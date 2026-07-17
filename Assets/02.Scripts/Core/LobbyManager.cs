using UnityEngine;

public class LobbyManager
{
    public void EnterLobby()
    {
        UIManager.Instance.OpenUI(UIRootType.MainUI, UIType.LobbyUI);
    }

    public void ExitLobby()
    {
        UIManager.Instance.CloseUI(UIRootType.MainUI, UIType.LobbyUI);
        UIManager.Instance.CloseUI(UIRootType.ContentUI, UIType.LoadGameUI);
    }
}