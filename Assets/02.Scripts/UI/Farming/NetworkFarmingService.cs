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
        // TODO : 정보 저장용
        // long uniqueId = GameUtil.GenerateUniqueId();

        // TODO: 데이터 드리븐용
        //var newItem = new FarmingSlotViewModel();
        //if (newItem == null) return;

        //var itemData = GameDataManager.Instance.GetData<ItemData>();
        //if (itemData == null) return;

        //newItem.ItemUniqueId = uniqueId;
        //newItem.ItemDataId = itemData.Id;
        

        // TODO : 아이템 랜덤 로직 넣기
        //newItem.ItemStackCount = addItemCount;

        var invenVm = GetFarmingViewModel();
        //invenVm.AddInventorySlotViewModel(newItem);

        // TODO : 저장 기능 구현 후 수정
        // NetworkManager_re.Inst.SaveLoadService.RequestSaveData();
    }
}
