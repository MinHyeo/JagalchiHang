using UnityEngine;

public enum UIRootType 
{
    None,
    BackgroundUI,
    MainUI,
    ContentUI,
    PopupUI,
    VeryFrontUI,
}

public enum UIType
{
    LobbyUI,
    LoadGameUI,
    MainTest,
    PopupTest,
}

public static class UIManagerExtension
{
    public static string GetUIPath(this UIManager uiManager, UIRootType uiRootType, UIType uiType)
    {
        string path = string.Empty;

        path = $"UI/{uiRootType}/{uiType}";
        return path;
    }

    public static void ShowStartupUIOnGameStart(this UIManager uiManager)
    {
    }

    public static void OpenLoadGameUI(this UIManager uiManager, LoadGameUIType loadGameType)
    {
        UIBase uiBase = uiManager.OpenUI(UIRootType.ContentUI, UIType.LoadGameUI);
        if (uiBase == null)
            return;

        if(uiBase is LoadGameUI loadGameUI)
        {
            loadGameUI.Init(loadGameType);
        }
    }
}