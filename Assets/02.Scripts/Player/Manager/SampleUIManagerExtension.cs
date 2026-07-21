using UnityEngine;

public enum SampleUIRootType
{
    None,
    BackgroundUI,
    MainUI,
    ContentUI,
    PopupUI,
    VeryFrontUI,
}

public enum SampleUIType
{
    MainTest,
    PopupTest,
    TestPlayerStatus
}

public static class SampleUIManagerExtension
{
    public static string GetUIPath(this SampleUIManager uiManager, SampleUIRootType uiRootType, SampleUIType uiType)
    {
        string path = string.Empty;

        path = $"UI/{uiRootType}/{uiType}";
        return path;
    }

    public static void ShowStartupUIOnGameStart(this SampleUIManager uiManager)
    {
    }
}