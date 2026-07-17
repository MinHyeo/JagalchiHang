using UnityEngine;

public class NetworkFarmingService
{
    private FarmingViewModel _localFarmingViewModel;

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
        var farmingVm = new FarmingViewModel();
        farmingVm.AddFarmingSlotViewModel();
        _localFarmingViewModel = farmingVm;
        return farmingVm;
    }
}
