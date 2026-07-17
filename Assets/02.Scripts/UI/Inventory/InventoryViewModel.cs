using System.Collections.Generic;
using UnityEngine;

public class InventoryViewModel : ViewModelBase
{
    private const int _slotCount = 36; // const를 빼거나 변경 가능

    private Dictionary<int, InventorySlotViewModel> _inventorySlots = new Dictionary<int, InventorySlotViewModel>();
    public Dictionary<int, InventorySlotViewModel> InventorySlots
    {
        get => _inventorySlots;
        set
        {
            if (_inventorySlots != value)
            {
                _inventorySlots = value;
                OnPropertyChanged(nameof(_inventorySlots));
            }
        }
    }

    public void AddInventorySlotViewModel()
    {
        _inventorySlots.Clear();

        for (int i = 0; i < _slotCount; i++)
        {
            _inventorySlots.Add(i, new InventorySlotViewModel());
        }
    }

    // 유니크 아이디가 생기면 교환 로직 수정
    public void SwapSlots(int startIdx, int endIdx)
    {
        if (!_inventorySlots.ContainsKey(startIdx) || !_inventorySlots.ContainsKey(endIdx)) return;
        if (startIdx == endIdx) return;

        string tempId = InventorySlots[startIdx].ItemDataId;
        int tempCount = InventorySlots[startIdx].ItemStackCount;

        InventorySlots[startIdx].SetItem(InventorySlots[endIdx].ItemDataId, InventorySlots[endIdx].ItemStackCount);
        InventorySlots[endIdx].SetItem(tempId, tempCount);
    }

    public bool AcquireItem(string itemDataId, int count)
    {
        var itemData = GameDataManager.Instance.GetData<ItemData>(itemDataId);
        bool isStackable = itemData.IsStackable;
        int maxCount = itemData.MaxCount;

        if (isStackable)
        {
            for (int i = 0; i < _slotCount; i++)
            {
                if (InventorySlots[i].ItemDataId == itemDataId && _inventorySlots[i].ItemStackCount < maxCount)
                {
                    InventorySlots[i].ItemStackCount += count;
                    return true;
                }
            }
        }

        for (int i = 0; i < _slotCount; i++)
        {
            if (string.IsNullOrEmpty(InventorySlots[i].ItemDataId))
            {
                InventorySlots[i].SetItem(itemDataId, count);
                return true;
            }
        }

        return false;
    }
}
