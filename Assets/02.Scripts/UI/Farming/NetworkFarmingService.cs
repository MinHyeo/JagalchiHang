using System.Collections.Generic;
using UnityEngine;

public class NetworkFarmingService
{
    private Dictionary<string, FarmingViewModel> _activeFarmingBoxList = new Dictionary<string, FarmingViewModel>();
    private FarmingViewModel _localFarmingViewModel;

    private string _currentActiveBoxUniqueId = string.Empty;
    public string CurrentActiveBoxUniqueId => _currentActiveBoxUniqueId;

    public FarmingViewModel GetFarmingViewModel()
    {
        if (_localFarmingViewModel == null)
        {
            CreateLocalFarmingViewModel();
        }
        return _localFarmingViewModel;
    }

    private FarmingViewModel CreateLocalFarmingViewModel()
    {
        GameDataManager.Instance.LoadData<ItemData>();
        var farmingVm = new FarmingViewModel();
        _localFarmingViewModel = farmingVm;
        farmingVm.AddFarmingSlotViewModel();
        return farmingVm;
    }

    public FarmingViewModel LoadFarmingBox(string boxUniqueId)
    {
        _currentActiveBoxUniqueId = boxUniqueId;

        if (_activeFarmingBoxList.ContainsKey(boxUniqueId))
        {
            return _activeFarmingBoxList[boxUniqueId];
        }

        var newFarmingVm = new FarmingViewModel();

        newFarmingVm.CreateRandomFarmingItemSlot();

        _activeFarmingBoxList.Add(boxUniqueId, newFarmingVm);

        return newFarmingVm;
    }
    
    public void OnExitMap()
    {
        _activeFarmingBoxList.Clear();
    }
}
