using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIButton : MonoBehaviour
{
    [SerializeField] private Button Button_Base;
    [SerializeField] private TextMeshPro Text_Base;
    [SerializeField] private Image Image_Base;
    [SerializeField] private Image Image_Select;

    private bool _isManualUnbindEvent;

    private void Awake()
    {
        InitUIButton();
        SetDefalultUI();
    }

    private void OnDisable()
    {
        if (_isManualUnbindEvent == false)
        {
            Button_Base.onClick.RemoveAllListeners();
        }
    }

    private void SetDefalultUI()
    {
        if (Image_Select != null)
        {
            Image_Select.gameObject.SetActive(false);
        }
    }

    private void InitUIButton()
    {
        if (Button_Base != null)
        {
            return;
        }

        var button = this.gameObject.GetComponentInChildren<Button>();
        if (button != null)
        {
            this.Button_Base = button;
        }
    }

    public void BindOnClickButtonEvent(Action onClickCallback, bool isManualUnbindEvent = false)
    {
        if (Button_Base == null) return;

        Button_Base.onClick.AddListener(onClickCallback.Invoke);
        _isManualUnbindEvent = isManualUnbindEvent;
    }

    public void UnBindAllOnClickButtonEvent()
    {
        if (Button_Base == null) return;

        Button_Base.onClick.RemoveAllListeners();
    }

    public void ChangeButtonText(string buttonStr)
    {
        if (Text_Base == null) return;

        Text_Base.text = buttonStr;
    }
}
