using System;
using System.Collections.Generic;
using UnityEngine;

public class InventoryViewModel : ViewModelBase
{
    private int _slotCount = 36; // const를 빼거나 변경 가능

    public int SlotCount
    {
        get => _slotCount;
        set
        {
            if(_slotCount != value)
            {
                _slotCount = value;
                OnPropertyChanged(nameof(SlotCount));

            }
        }
        
    }
    
    private Dictionary<int, InventorySlotViewModel> _inventorySlots = new Dictionary<int, InventorySlotViewModel>();
    public Dictionary<int, InventorySlotViewModel> InventorySlots
    {
        get => _inventorySlots;
        set
        {
            if (_inventorySlots != value)
            {
                _inventorySlots = value;
                OnPropertyChanged(nameof(InventorySlots));
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

    public void SwapSlots(int startIdx, int endIdx)
    {
        if (!_inventorySlots.ContainsKey(startIdx) || !_inventorySlots.ContainsKey(endIdx)) return;
        if (startIdx == endIdx) return;

        var startSlot = _inventorySlots[startIdx];
        var endSlot = _inventorySlots[endIdx];

        if (!string.IsNullOrEmpty(startSlot.ItemDataId) &&
        startSlot.ItemDataId == endSlot.ItemDataId &&
        startSlot.IsStackable)
        {
            int maxCount = endSlot.MaxCount;
            int spaceLeft = maxCount - endSlot.ItemStackCount;

            if (spaceLeft > 0)
            {
                int addAmount = Mathf.Min(spaceLeft, startSlot.ItemStackCount);
                endSlot.ItemStackCount += addAmount;
                startSlot.ItemStackCount -= addAmount;

                if (startSlot.ItemStackCount <= 0)
                {
                    startSlot.Clear();
                }
                return;
            }
        }

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

    public void TestAddItem()
    {
        NetworkManager.Instance.AddItemToInventory("Item_Drop_09", 5);
        NetworkManager.Instance.AddItemToInventory("Item_Drop_10", 7);
        NetworkManager.Instance.AddItemToInventory("Crop_Carrot", 10);
        NetworkManager.Instance.AddItemToInventory("Crop_Corn", 10);
        NetworkManager.Instance.AddItemToInventory("Crop_Onion", 10);
        NetworkManager.Instance.AddItemToInventory("Crop_Pea", 10);
        NetworkManager.Instance.AddItemToInventory("Crop_Potato", 10);
        NetworkManager.Instance.AddItemToInventory("Crop_Pumpkin", 10);
        NetworkManager.Instance.AddItemToInventory("Crop_Tomato", 10);
        NetworkManager.Instance.AddItemToInventory("Crop_Wheat", 10);
        NetworkManager.Instance.AddItemToInventory("Item_Seed_Carrot", 7);
        NetworkManager.Instance.AddItemToInventory("Item_Seed_Corn", 7);
        NetworkManager.Instance.AddItemToInventory("Item_Seed_Onion", 7);
        NetworkManager.Instance.AddItemToInventory("Item_Seed_Pea", 7);
        NetworkManager.Instance.AddItemToInventory("Item_Seed_Potato", 7);
        NetworkManager.Instance.AddItemToInventory("Item_Seed_Pumpkin", 7);
        NetworkManager.Instance.AddItemToInventory("Item_Seed_Tomato", 7);
        NetworkManager.Instance.AddItemToInventory("Item_Seed_Wheat", 7);
        NetworkManager.Instance.AddItemToInventory("Item_Drop_01", 10);
        NetworkManager.Instance.AddItemToInventory("Item_Drop_02", 10);
        NetworkManager.Instance.AddItemToInventory("Item_Drop_03", 10);
        NetworkManager.Instance.AddItemToInventory("Item_Drop_04", 10);
        NetworkManager.Instance.AddItemToInventory("Item_Drop_05", 10);
    }

    public bool RequestUseItem(long requestUseTargetItemUniqeuId)
    {
        InventorySlotViewModel targetSlot = null;

        foreach (var slot in InventorySlots.Values)
        {
            if (slot.ItemUniqueId == requestUseTargetItemUniqeuId)
            {
                targetSlot = slot;
                break;
            }
        }

        if (targetSlot == null || !targetSlot.IsUsable) return false;

        var itemData = GameDataManager.Instance.GetData<ItemData>(targetSlot.ItemDataId);
        if (itemData == null) return false;

        if (!string.IsNullOrEmpty(itemData.UseItemType))
        {
            UseItemFunction(itemData.UseItemType, itemData.UseItemEffect);
        }

        targetSlot.ConsumeItem();

        OnPropertyChanged("ItemUsed");
        return true;
    }

    private void UseItemFunction(string itemUseType, int useItemEffect) 
    { 
        if (useItemEffect == 0) return;

        var playerVm = NetworkManager.Instance.PlayerService.GetPlayerViewModel();
        if (itemUseType == "Hunger")
        {
            playerVm.CurrentHunger = Math.Min(playerVm.CurrentHunger + useItemEffect, playerVm.MaxHunger);
            Debug.Log($"플레이어의 허기가 {useItemEffect}만큼 증가했다.     허기: {playerVm.CurrentHunger}");
        }
        else if (itemUseType == "Thirsty")
        {
            playerVm.CurrentThirst = Math.Min(playerVm.CurrentThirst + useItemEffect, playerVm.MaxThirst);
            Debug.Log($"플레이어의 목마름이 {useItemEffect}만큼 증가했다.     목마름: {playerVm.CurrentThirst}");
        }
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
