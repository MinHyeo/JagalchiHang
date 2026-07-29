using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MaterialSlot : MonoBehaviour
{
    [SerializeField] private string _itemId;
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

    public bool CheckRechargeable(InventoryViewModel inventoryVM, GeneratorViewModel generatorVM)
    {
        int count = inventoryVM.GetItemCount(_itemId);
        int maxPower = generatorVM.MaxPower;
        int currentPower = generatorVM.CurrentPower;
        if (count <= 0 || maxPower < currentPower)
        {
            return false;
        }
        return true;
    }

    public void Recharge(InventoryViewModel inventoryVM)
    {
        inventoryVM.RemoveItem(_itemId, 1);
        UpdateItemCount(inventoryVM);
    }

    public bool UpdateItemCount(InventoryViewModel inventoryVM)
    {
        int count = inventoryVM.GetItemCount(_itemId);

        _itemCountText.text = count.ToString();
        return true;
    }
}
