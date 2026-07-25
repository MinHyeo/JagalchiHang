using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class NetworkInventoryService
{
    private InventoryViewModel _localInventoryViewModel;

    public InventoryViewModel GetLocalInventoryViewModel()
    {
        if (_localInventoryViewModel == null)
        {
            CreateLocalInventoryViewModel();
        }

        return _localInventoryViewModel;
    }

    private InventoryViewModel CreateLocalInventoryViewModel()
    {
        GameDataManager.Instance.LoadData<ItemData>();
        var inventoryVm = new InventoryViewModel();
        inventoryVm.AddInventorySlotViewModel();
        _localInventoryViewModel = inventoryVm;
        return inventoryVm;
    }

    public void AddItem(string itemDataId, int addItemCount)
    {
        var invenVm = GetLocalInventoryViewModel();
        invenVm.AcquireItem(itemDataId, addItemCount);

        // TODO : 저장 기능 구현 후 연동
        // NetworkManager.Instance.SaveLoadService.RequestSaveData();
    }

    public bool RequestUseItem(long requestUseTargetItemUniqeuId)
    {
        var invenVm = GetLocalInventoryViewModel();
        invenVm.RequestUseItem(requestUseTargetItemUniqeuId);

        return true;
    }

    public void RequestRemoveItem(string removeTargetDataId, int reduceCount)
    {
        var invenVm = GetLocalInventoryViewModel();
        invenVm.RemoveItem(removeTargetDataId, reduceCount);

        // TODO : 세이브 필요
    }

    public void BindInventoryInputEvent()
    {
        InputManager.Instance.OnClickInventory += OnOpenInventoryUI;
    }

    public void UnBindInventoryInputEvent()
    {
        InputManager.Instance.OnClickInventory -= OnOpenInventoryUI;
    }

    private void OnOpenInventoryUI()
    {
        if (UIManager.Instance.IsOpenUI(UIType.InventoryUI))
        {
            UIManager.Instance.CloseUI(UIRootType.PopupUI, UIType.InventoryUI);
        }
        else
        {
            UIManager.Instance.OpenUI(UIRootType.PopupUI, UIType.InventoryUI);
        }
    }

    public void TestItem()
    {
        var invenVem = GetLocalInventoryViewModel();
        invenVem.TestAddItem();
    }

    public List<ItemSaveModel> GetSaveData()
    {
        var saveList = new List<ItemSaveModel>();
        var invenVm = GetLocalInventoryViewModel();

        foreach (var pair in invenVm.InventorySlots)
        {
            var slotVm = pair.Value;
            if (!string.IsNullOrEmpty(slotVm.ItemDataId))
            {
                saveList.Add(new ItemSaveModel
                {
                    ItemUniqueId = slotVm.ItemUniqueId,
                    ItemDataId = slotVm.ItemDataId,
                    ItemStackCount = slotVm.ItemStackCount,
                    Location = ItemLocationType.Inventory,
                    SlotIndex = pair.Key
                });
            }
        }

        return saveList;
    }

    public void LoadSaveData(List<ItemSaveModel> itemSaveList)
    {
        var invenVm = GetLocalInventoryViewModel();
        if (invenVm == null || itemSaveList == null) return;

        foreach (var slot in invenVm.InventorySlots.Values)
        {
            slot.Clear();
        }

        foreach (var itemSave in itemSaveList)
        {
            if (itemSave.Location == ItemLocationType.Inventory)
            {
                int slotIdx = itemSave.SlotIndex;

                if (invenVm.InventorySlots.ContainsKey(slotIdx))
                {
                    var slot = invenVm.InventorySlots[slotIdx];
                    slot.ItemUniqueId = itemSave.ItemUniqueId;
                    slot.SetItem(itemSave.ItemDataId, itemSave.ItemStackCount);
                }
            }
        }

        invenVm.NotifySlotCountChanged();
    }
}
