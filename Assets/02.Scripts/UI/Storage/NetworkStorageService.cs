using System.Collections.Generic;
using UnityEngine;

public class NetworkStorageService
{
    private StorageViewModel _localStorageViewModel;

    public StorageViewModel GetLocalStorageViewModel()
    {
        if (_localStorageViewModel == null)
        {
            CreateLocalStorageViewModel();
        }

        return _localStorageViewModel;
    }

    private StorageViewModel CreateLocalStorageViewModel()
    {
        GameDataManager.Instance.LoadData<ItemData>();
        var storageVm = new StorageViewModel();
        storageVm.AddStorageSlotViewModel();
        _localStorageViewModel = storageVm;
        return storageVm;
    }

    public List<ItemSaveModel> GetSaveData()
    {
        var saveList = new List<ItemSaveModel>();
        var storageVm = GetLocalStorageViewModel();

        foreach (var pair in storageVm.StorageSlots)
        {
            var slotVm = pair.Value;
            if (!string.IsNullOrEmpty(slotVm.ItemDataId))
            {
                saveList.Add(new ItemSaveModel
                {
                    ItemUniqueId = slotVm.ItemUniqueId,
                    ItemDataId = slotVm.ItemDataId,
                    ItemStackCount = slotVm.ItemStackCount,
                    Location = ItemLocationType.Storage,
                    SlotIndex = pair.Key
                });
            }
        }

        return saveList;
    }

    public void LoadSaveData(List<ItemSaveModel> itemSaveList)
    {
        var storageVm = GetLocalStorageViewModel();
        if (storageVm == null || itemSaveList == null) return;

        foreach (var slot in storageVm.StorageSlots.Values)
        {
            slot.Clear();
        }

        foreach (var itemSave in itemSaveList)
        {
            if (itemSave.Location == ItemLocationType.Storage)
            {
                int slotIdx = itemSave.SlotIndex;

                if (storageVm.StorageSlots.ContainsKey(slotIdx))
                {
                    var slot = storageVm.StorageSlots[slotIdx];
                    slot.ItemUniqueId = itemSave.ItemUniqueId;
                    slot.SetItem(itemSave.ItemDataId, itemSave.ItemStackCount);
                }
            }
        }
    }
}
