using System.Collections.Generic;
using UnityEngine;

public class FarmingViewModel : ViewModelBase
{
    private const int _slotCount = 8; // const를 빼거나 변경 가능

    private Dictionary<int, FarmingSlotViewModel> _farmingSlots = new Dictionary<int, FarmingSlotViewModel>();
    public Dictionary<int, FarmingSlotViewModel> FarmingSlots
    {
        get => _farmingSlots;
        set
        {
            if (_farmingSlots != value)
            {
                _farmingSlots = value;
                OnPropertyChanged(nameof(FarmingSlots));
            }
        }
    }

    public void AddFarmingSlotViewModel()
    {
        _farmingSlots.Clear();

        for (int i = 0; i < _slotCount; i++)
        {
            _farmingSlots.Add(i, new FarmingSlotViewModel());
        }
    }

    public void SwapSlots(int startIdx, int endIdx)
    {
        if (!FarmingSlots.ContainsKey(startIdx) || !FarmingSlots.ContainsKey(endIdx)) return;
        if (startIdx == endIdx) return;

        var startSlot = _farmingSlots[startIdx];
        var endSlot = _farmingSlots[endIdx];

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

    public void CreateRandomFarmingItemSlot()
    {
        AddFarmingSlotViewModel();

        var itemDictionary = GameDataManager.Instance.GetAllData<ItemData>();

        if (itemDictionary == null || itemDictionary.Count == 0)
        {
            GameDataManager.Instance.LoadData<ItemData>();
            itemDictionary = GameDataManager.Instance.GetAllData<ItemData>();
        }

        if (itemDictionary == null || itemDictionary.Count == 0) return;

        List<ItemData> globalPool = new List<ItemData>(itemDictionary);

        int slotsToFill = Random.Range(2, 6); // 무작위 슬롯 수 결정, 수정 가능
        slotsToFill = Mathf.Clamp(slotsToFill, 0, _slotCount);

        List<ItemData> chosenItems = SelectRandomItems(globalPool, slotsToFill);

        for (int i = 0; i < chosenItems.Count; i++)
        {
            var itemData = chosenItems[i];

            int maxAvailableStack = itemData.IsStackable ? itemData.MaxCount : 1;

            int minRange = Mathf.Min(itemData.MinDropCount, maxAvailableStack);
            int maxRange = Mathf.Min(itemData.MaxDropCount, maxAvailableStack);

            int stackCount = Random.Range(minRange, maxRange + 1);

            FarmingSlotViewModel slotVm = FarmingSlots[i];

            slotVm.ItemUniqueId = GameUtil.GenerateUniqueId();
            slotVm.SetItem(itemData.Id, stackCount);
        }
    }

    public List<ItemData> SelectRandomItems(List<ItemData> pool, int countToSelect)
    {
        List<ItemData> result = new List<ItemData>();

        List<ItemData> activePool = new List<ItemData>(pool);
        foreach (var item in pool)
        {
            if (item.DropWeight > 0)
            {
                activePool.Add(item);
            }
        }

        int actualCount = Mathf.Min(countToSelect, activePool.Count);

        for (int i = 0; i < actualCount; i++)
        {
            int totalWeight = 0;
            foreach (var item in activePool)
            {
                totalWeight += item.DropWeight;
            }

            if (totalWeight <= 0) break;

            int randomValue = Random.Range(0, totalWeight);
            int currentSum = 0;
            int selectedIndex = -1;

            for (int j = 0; j < activePool.Count; j++)
            {
                currentSum += activePool[j].DropWeight;
                if (randomValue < currentSum)
                {
                    selectedIndex = j;
                    break;
                }
            }

            if (selectedIndex != -1)
            {
                result.Add(activePool[selectedIndex]);
                activePool.RemoveAt(selectedIndex);
            }
        }

        return result;
    }
}
