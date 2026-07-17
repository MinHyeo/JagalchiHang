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
    MainTest,
    PopupTest,
    HudMainUI
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

    public static void AddHudSlot(this UIManager uIManager, int instanceId, Transform targetTransform)
    {
        var uiBase = uIManager.GetOpenUI(UIRootType.MainUI, UIType.HudMainUI);
        if (uiBase == null) return;

        if (uiBase is HudMainUI hudMainUI)
        {
            hudMainUI.AddSlotHudHp(instanceId, targetTransform);
        }
    }

    public static void RemoveHudSlot(this UIManager uIManager, int instanceId)
    {
        var uiBase = uIManager.GetOpenUI(UIRootType.MainUI, UIType.HudMainUI);
        if (uiBase == null) return;

        if (uiBase is HudMainUI hudMainUI)
        {
            hudMainUI.RemoveSlotHudHp(instanceId);
        }
    }

    public static void RemoverAllSlotHudHp(this UIManager uIManager)
    {
        var uiBase = uIManager.GetOpenUI(UIRootType.MainUI, UIType.HudMainUI);
        if (uiBase == null) return;

        if (uiBase is HudMainUI hudMainUI)
        {
            hudMainUI.RemoveAllSlotHudHp();
        }
    }
}