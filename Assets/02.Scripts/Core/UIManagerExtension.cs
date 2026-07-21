using System;
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
    HudMainUI,
    InventoryUI,
    FarmingUI,
    StorageUI,
    NpcUI,
    TestPlayerStatus,
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

    public static void AddSlotHpHud(this UIManager uIManager, int instanceId, Transform targetTransform)
    {
        var uiBase = uIManager.GetOpenUI(UIRootType.MainUI, UIType.HudMainUI);
        if (uiBase == null) return;

        if (uiBase is HudMainUI hudMainUI)
        {
            hudMainUI.AddSlotHudHp(instanceId, targetTransform);
        }
    }

    public static void RemoveSlotHpHud(this UIManager uIManager, int instanceId)
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

    public static void AddSlotHudInteraction(this UIManager uIManager, int instanceId, string interactionTitle, string interactionKey,
        Transform targetTransform, Action<string> onClickCallback = null)
    {
        var uiBase = uIManager.GetOpenUI(UIRootType.MainUI, UIType.HudMainUI);
        if (uiBase == null) return;

        if (uiBase is HudMainUI hudMainUI)
        {
            hudMainUI.AddInteractionSlot(instanceId, interactionTitle, interactionKey, targetTransform, onClickCallback);
        }
    }

    public static void RemoveSlotHudInteraction(this UIManager uIManager, int instanceId)
    {
        var uiBase = uIManager.GetOpenUI(UIRootType.MainUI, UIType.HudMainUI);
        if (uiBase == null) return;

        if (uiBase is HudMainUI hudMainUI)
        {
            hudMainUI.RemoveInteractionSlot(instanceId);
        }
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

    public static void OpenFarmingUI(this UIManager uiManager, string boxUniqueId)
    {
        UIBase uiBase = uiManager.OpenUI(UIRootType.PopupUI, UIType.FarmingUI);
        if (uiBase == null) return;

        if (uiBase is FarmingUI farmingUI)
        {
            farmingUI.Init(boxUniqueId);
        }
    }
}