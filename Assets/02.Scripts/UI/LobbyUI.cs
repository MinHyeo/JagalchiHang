using UnityEngine;

public class LobbyUI : UIBase
{
    [SerializeField] private UIButton _newStartButton;
    [SerializeField] private UIButton _loadStartButton;
    [SerializeField] private UIButton _settingButton;
    [SerializeField] private UIButton _exitButton;

    private void OnEnable()
    {
        _newStartButton.BindOnClickButtonEvent(OnClickNewStart);
        _loadStartButton.BindOnClickButtonEvent(OnClickLoadStart);
        _settingButton.BindOnClickButtonEvent(OnClickSettingButton);
        _exitButton.BindOnClickButtonEvent(OnClickExitButton);
    }

    private void OnDisable()
    {
        _newStartButton.UnBindOnClickButtonEvent(OnClickNewStart);
        _loadStartButton.UnBindOnClickButtonEvent(OnClickLoadStart);
        _settingButton.UnBindOnClickButtonEvent(OnClickSettingButton);
        _exitButton.UnBindOnClickButtonEvent(OnClickExitButton);
    }

    private void OnClickNewStart()
    {
        //UIManager.Instance.OpenUI(UIRootType.ContentUI, UIType.LoadGameUI);
        UIManager.Instance.OpenLoadGameUI(LoadGameUIType.NewGame);
    }

    private void OnClickLoadStart()
    {
        //UIManager.Instance.OpenUI(UIRootType.ContentUI, UIType.LoadGameUI);
        UIManager.Instance.OpenLoadGameUI(LoadGameUIType.LoadGame);
    }

    private void OnClickSettingButton()
    {

    }

    private void OnClickExitButton()
    {
        Application.Quit();
    }
}