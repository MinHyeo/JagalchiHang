using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SaveDataSlot : MonoBehaviour
{
    private int _slotIndex;

    [SerializeField] private UIButton _slotButton;

    [SerializeField] private TextMeshProUGUI _saveIndexText;
    [SerializeField] private TextMeshProUGUI _saveDayText;

    private event Action<int> _onSlotClickAction;

    private void OnEnable()
    {
        _slotButton.BindOnClickButtonEvent(BindSlotClickEvent);
    }

    private void OnDisable()
    {
        _slotButton.UnBindOnClickButtonEvent(BindSlotClickEvent);
        _onSlotClickAction = null;
    }

    public void Init(int index, string title, string info, Action<int> onClickAction)
    {
        _slotIndex = index;
        _onSlotClickAction = onClickAction;

        UpdateSlotText(title, info);
    }

    private void BindSlotClickEvent()
    {
        _onSlotClickAction?.Invoke(_slotIndex);
    }

    private void UpdateSlotText(string title, string info)
    {
        _saveIndexText.text = title;
        _saveDayText.text = info;
    }
}