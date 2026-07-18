using System.Collections.Generic;
using UnityEngine;

public class StorageViewModel : ViewModelBase
{
    private const int _slotCount = 72; // const를 빼거나 변경 가능

    private Dictionary<int, StorageSlotViewModel> _storageSlots = new Dictionary<int, StorageSlotViewModel>();
    public Dictionary<int, StorageSlotViewModel> StorageSlots
    {
        get => _storageSlots;
        set
        {
            if (_storageSlots != value)
            {
                _storageSlots = value;
                OnPropertyChanged(nameof(StorageSlots));
            }
        }
    }

    //public void TestStorage()
    //{
    //    AddInventorySlotViewModel();
    //    _inventorySlots[0].SetItem("암", 3);
    //    _inventorySlots[1].SetItem("암", 6);
    //}

    public void AddStorageSlotViewModel()
    {
        _storageSlots.Clear();

        for (int i = 0; i < _slotCount; i++)
        {
            _storageSlots.Add(i, new StorageSlotViewModel());
        }
    }

    public void SwapSlots(int startIdx, int endIdx)
    {
        if (!StorageSlots.ContainsKey(startIdx) || !StorageSlots.ContainsKey(endIdx)) return;
        if (startIdx == endIdx) return;

        var startSlot = _storageSlots[startIdx];
        var endSlot = _storageSlots[endIdx];

        long tempUniqueId = startSlot.ItemUniqueId;
        string tempId = startSlot.ItemDataId;
        int tempCount = startSlot.ItemStackCount;

        startSlot.ItemUniqueId = endSlot.ItemUniqueId;
        startSlot.SetItem(endSlot.ItemDataId, endSlot.ItemStackCount);

        endSlot.ItemUniqueId = tempUniqueId;
        endSlot.SetItem(tempId, tempCount);

        OnPropertyChanged("StorageChanged");
    }
}
