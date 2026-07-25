using UnityEngine;

public class NetworkSettingService
{
    public void BindSettingInputEvent()
    {
        InputManager.Instance.OnClickSettingUI += OnOpenSettingUI;
    }

    public void UnBindSettingInputEvent()
    {
        InputManager.Instance.OnClickSettingUI -= OnOpenSettingUI;
    }

    private void OnOpenSettingUI()
    {
        if (UIManager.Instance.IsOpenUI(UIType.SettingUI))
        {
            UIManager.Instance.CloseUI(UIRootType.PopupUI, UIType.SettingUI);
        }
        else
        {
            UIManager.Instance.OpenUI(UIRootType.PopupUI, UIType.SettingUI);
        }
    }
}
