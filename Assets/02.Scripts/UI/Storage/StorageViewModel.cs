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

    public void AddInventorySlotViewModel()
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

        string tempId = StorageSlots[startIdx].ItemDataId;
        int tempCount = StorageSlots[startIdx].ItemStackCount;

        StorageSlots[startIdx].SetItem(StorageSlots[endIdx].ItemDataId, StorageSlots[endIdx].ItemStackCount);
        StorageSlots[endIdx].SetItem(tempId, tempCount);
    }
}
