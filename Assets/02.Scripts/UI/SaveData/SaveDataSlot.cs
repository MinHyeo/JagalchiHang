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

    public void Init(int index, int day, Action<int> onClickAction)
    {
        _slotIndex = index;
        _onSlotClickAction = onClickAction;

        UpdateSlotText(day);
    }

    private void BindSlotClickEvent()
    {
        _onSlotClickAction?.Invoke(_slotIndex);
    }

    private void UpdateSlotText(int day)
    {
        if (_saveIndexText != null)
        {
            _saveIndexText.text = $"슬롯 {_slotIndex + 1}";
        }

        if (_saveDayText != null)
        {
            if (day == 0)
            {
                _saveDayText.text = "비어 있음";
            }
            else
            {
                _saveDayText.text = $"Day : {day}";
            }
        }
    }
}