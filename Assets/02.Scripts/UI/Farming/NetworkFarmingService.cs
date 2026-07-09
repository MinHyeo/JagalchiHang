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

    public void AddItem(string itemDataId, int addItemCount)
    {
        // TODO : 데이터 드리븐용
        // long uniqueId = GameUtil.GenerateUniqueId();

        //var newItem = new InventorySlotViewModel();
        // newItem.ItemUniqueId = uniqueId;
        //newItem.ItemDataId = itemDataId;
        //newItem.ItemStackCount = addItemCount;

        //var invenVm = GetLocalPlayerInventoryViewModel();
        //invenVm.AddInventorySlotViewModel(newItem);

        // NetworkManager_re.Inst.SaveLoadService.RequestSaveData();
    }
}
