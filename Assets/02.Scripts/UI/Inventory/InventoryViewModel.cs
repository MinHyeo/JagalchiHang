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

        var startSlot = _inventorySlots[startIdx];
        var endSlot = _inventorySlots[endIdx];

        long tempUniqueId = startSlot.ItemUniqueId;
        string tempId = startSlot.ItemDataId;
        int tempCount = startSlot.ItemStackCount;

        startSlot.ItemUniqueId = endSlot.ItemUniqueId;
        startSlot.SetItem(endSlot.ItemDataId, endSlot.ItemStackCount);

        endSlot.ItemUniqueId = tempUniqueId;
        endSlot.SetItem(tempId, tempCount);
    }

    public bool AcquireItem(string itemDataId, int count)
    {
        var itemData = GameDataManager.Instance.GetData<ItemData>(itemDataId);
        if (itemData == null) return false;

        bool isStackable = itemData.IsStackable;
        int maxCount = itemData.MaxCount;

        if (isStackable)
        {
            for (int i = 0; i < _slotCount; i++)
            {
                if (InventorySlots[i].ItemDataId == itemDataId && _inventorySlots[i].ItemStackCount < maxCount)
                {
                    int fullCount = maxCount - _inventorySlots[i].ItemStackCount;
                    int addAmount = Mathf.Min(fullCount, count);

                    _inventorySlots[i].ItemStackCount += addAmount;
                    count -= addAmount;

                    if (count <= 0)
                    {
                        OnPropertyChanged("ItemListAdded");
                        return true;
                    }
                }
            }
        }

        while (count > 0)
        {
            int emptySlotIdx = GetEmptySlotIndex();
            if (emptySlotIdx == -1)
            {
                OnPropertyChanged("ItemListAdded");
                return false;
            }

            int insertCount = isStackable ? Mathf.Min(maxCount, count) : 1;

            long newUniqueId = GameUtil.GenerateUniqueId();
            _inventorySlots[emptySlotIdx].ItemUniqueId = newUniqueId;
            _inventorySlots[emptySlotIdx].SetItem(itemDataId, insertCount);

            count -= insertCount;
        }

        OnPropertyChanged("ItemListAdded");
        return true;
    }

    private int GetEmptySlotIndex()
    {
        for (int i = 0; i < _slotCount; i++)
        {
            if (string.IsNullOrEmpty(_inventorySlots[i].ItemDataId))
            {
                return i;
            }
        }
        return -1;
    }

    public void RemoveItemSlotViewModel(long uniqueId)
    {
        foreach (var slot in _inventorySlots.Values)
        {
            if (slot.ItemUniqueId == uniqueId) 
            {
                slot.Clear();
                break;
            }
        }

        OnPropertyChanged("ItemListRemoved");
    }
}
