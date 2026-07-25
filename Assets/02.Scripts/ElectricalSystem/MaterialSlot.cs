using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MaterialSlot : MonoBehaviour
{
    [SerializeField] private Outline _outline;
    [SerializeField] private TextMeshProUGUI _itemCountText;
    private UIButton _slotButton;

    [SerializeField] private int _amount = 20;
    public int Amount => _amount;

    public event Action<MaterialSlot> OnSlotClickedEvent;

    private void Awake()
    {
        _outline = GetComponent<Outline>();
        _slotButton = GetComponent<UIButton>();
    }

    private void OnEnable()
    {
        _slotButton.BindOnClickButtonEvent(OnButtonClicked);
    }

    private void OnDisable()
    {
        _slotButton.UnBindOnClickButtonEvent(OnButtonClicked);
    }

    public void SetActiveOutLine(bool isActive)
    {
        _outline.enabled = isActive;
    }

    private void OnButtonClicked()
    {
        OnSlotClickedEvent?.Invoke(this);
    }

    public void UpdateItemCount(int count)
    {
        _itemCountText.text = count.ToString();
    }
}
