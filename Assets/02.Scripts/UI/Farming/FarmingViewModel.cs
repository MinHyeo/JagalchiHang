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

    // TODO : 자동 아이템 생성 로직 만들어야 함
    public void CreateRandomFarmingItemSlot()
    {

    }
}
