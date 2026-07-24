using UnityEngine;
using UnityEngine.UI;

public class SettingUI : UIBase
{
    [Header("버튼")]
    [SerializeField] private UIButton _buttonResume;
    [SerializeField] private UIButton _buttonSave;
    [SerializeField] private UIButton _buttonSoundSetting;
    [SerializeField] private UIButton _buttonMainMenu;

    [Header("레이아웃")]
    [SerializeField] private GameObject _layoutButton;
    [SerializeField] private GameObject _layoutSoundSetting;

    [Header("사운드")]
    [SerializeField] private Slider _sliderBGM;
    [SerializeField] private Slider _sliderSFX;
    [SerializeField] private UIButton _buttonClose;

    private void Awake()
    {
        
    }

    private void OnEnable()
    {
        TimeManager.Instance.StopTime();

        _buttonResume.BindOnClickButtonEvent(OnClickResume);
        _buttonSave.BindOnClickButtonEvent(OnClickSave);
        _buttonSoundSetting.BindOnClickButtonEvent(OnClickSoundSetting);
        _buttonMainMenu.BindOnClickButtonEvent(OnClickMainMenu);
        _buttonClose.BindOnClickButtonEvent(OnClickCloseSoundSetting);

        _sliderBGM.onValueChanged.AddListener(OnChangedBGMVolume);
        _sliderSFX.onValueChanged.AddListener(OnChangedSFXVolume);

        CurrentSoundVolume();

        _layoutButton.SetActive(true);
        _layoutSoundSetting.SetActive(false);
    }

    private void OnDisable()
    {
        TimeManager.Instance.RestartTime();
        _sliderBGM.onValueChanged.RemoveListener(OnChangedBGMVolume);
        _sliderSFX.onValueChanged.RemoveListener(OnChangedSFXVolume);
    }

    private void OnClickResume()
    {
        UIManager.Instance.CloseUI(UIRootType.PopupUI, UIType.SettingUI);
    }

    private void OnClickSave()
    {
        // TODO : 세이브 창 열기
        // UIManager.Instance.OpenUI();
    }

    private void OnClickSoundSetting()
    {
        _layoutButton.SetActive(false);
        _layoutSoundSetting.SetActive(true);
    }
    private void OnClickCloseSoundSetting()
    {
        _layoutSoundSetting.SetActive(false);
        _layoutButton.SetActive(true);
    }

    private void OnClickMainMenu()
    {
        UIManager.Instance.OpenUI(UIRootType.MainUI, UIType.LobbyUI);
        UIManager.Instance.CloseUI(UIRootType.PopupUI, UIType.SettingUI);
        GameManager.Instance.ExitInGame();
    }

    private void CurrentSoundVolume()
    {
        if (SoundManager.Instance != null)
        {
            _sliderBGM.value = SoundManager.Instance.BgmVolume;
            _sliderSFX.value = SoundManager.Instance.SfxVolume;
        }
    }

    private void OnChangedBGMVolume(float value)
    {
        SoundManager.Instance.SetBgmVolume(value);
    }

    private void OnChangedSFXVolume(float value)
    {
        SoundManager.Instance.SetSfxVolume(value);
    }
}
