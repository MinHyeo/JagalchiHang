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

    public void TestFarming()
    {
        AddFarmingSlotViewModel();
        _farmingSlots[0].SetItem("암", 3);
        _farmingSlots[1].SetItem("암", 6);
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

        string tempId = FarmingSlots[startIdx].ItemDataId;
        int tempCount = FarmingSlots[startIdx].ItemStackCount;

        FarmingSlots[startIdx].SetItem(FarmingSlots[endIdx].ItemDataId, FarmingSlots[endIdx].ItemStackCount);
        FarmingSlots[endIdx].SetItem(tempId, tempCount);
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

        List<ItemData> chosenItems = FarmingLogic.SelectRandomItems(globalPool, slotsToFill);

        for (int i = 0; i < chosenItems.Count; i++)
        {
            var itemData = chosenItems[i];

            int maxAvailableStack = itemData.IsStackable ? itemData.MaxCount : 1;

            int minRange = Mathf.Min(itemData.MinDropCount, maxAvailableStack);
            int maxRange = Mathf.Min(itemData.MaxDropCount, maxAvailableStack);

            int stackCount = Random.Range(minRange, maxRange + 1);

            FarmingSlotViewModel slotVm = FarmingSlots[i];

            slotVm.ItemUniqueId = i + 1;
            slotVm.SetItem(itemData.Id, stackCount);
        }
    }
}
